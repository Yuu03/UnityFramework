﻿﻿﻿
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
using Unity.Linq;
using System;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Globalization;
using System.Linq;
using System.Collections.Generic;
using Extensions;
using Modules.Devkit;
using Modules.Devkit.Generators;
using Modules.Devkit.Spreadsheet;

namespace Modules.GameText.Editor
{
    public class GameTextScriptGenerator
    {
        //----- params -----

        private const string SheetIdTableScriptFileName = @"GameText.category.cs";

        private const string EnumContentsTemplate = @"{0} = {1},";

        private const string SheetIdTableScriptTemplate =
@"
// Generated by GameTextScriptGenerator.cs

using System;
using System.Collections.Generic;
using Extensions;

namespace Modules.GameText
{
    public enum GameTextCategory
    {
        [Label(""未使用"")]
        None = -1,

#ENUMS#
    }

    public partial class GameText
    {
        private readonly Dictionary<Type, GameTextCategory> CategoryTable = new Dictionary<Type, GameTextCategory>
        {
#TABLES#
        };
    }
}
";
        private const string CategoryLabelTemplate = @"[Label(""{0}"")]";
        private const string SheetIdTableContentsTemplate = @"{{ typeof({0}), GameTextCategory.{1} }},";

        private const string EnumScriptTemplate =
@"
// Generated by GameTextScriptGenerator.cs

using System.Collections.Generic;

namespace Modules.GameText
{
    public partial class GameText
	{
        public enum #ENUMNAME#
        {
#ENUMS#
        }
    }
}
";

        private const string SummaryTemplate = @"/// <summary> {0} </summary>";

        //----- method -----

        public static void Generate(SheetEntity[] spreadsheets, GameTextConfig config, int textColumn)
        {
            // EnumとシートIDを結び付けるファイル生成.
            GenerateCategoryFile(spreadsheets, config);

            // Enumファイルを生成.
            var generatedScripts = GenerateEnumFiles(spreadsheets, config, textColumn);

            // 不要になったEnumファイルがある場合に削除.
            DeleteUnusedEnumFiles(generatedScripts, config);
        }

        private static void GenerateCategoryFile(SheetEntity[] spreadsheets, GameTextConfig config)
        {
            var exportPath = config.TableScriptFolderPath;
            
            var sheetDefinitionRow = config.SheetDefinitionRow.GetValueOrDefault();
            var sheetIdColumn = config.SheetIdColumn.GetValueOrDefault();
            var ignoreSheets = config.IgnoreSheets;

            var enums = new StringBuilder();
            var contents = new StringBuilder();

            var targetSheets = spreadsheets.Where(x => !ignoreSheets.Contains(x.Title)).ToArray();

            for (var i = 0; i < targetSheets.Length ; ++i)
            {
                var spreadsheet = targetSheets[i];

                var enumName = GetSheetEnumName(spreadsheet, config);
                var sheetId = spreadsheet.GetValue(sheetIdColumn, sheetDefinitionRow);

                enums.Append("\t\t").AppendFormat(CategoryLabelTemplate, spreadsheet.Title).AppendLine();
                enums.Append("\t\t").AppendFormat(EnumContentsTemplate, enumName, sheetId);

                contents.Append("\t\t\t").AppendFormat(SheetIdTableContentsTemplate, enumName, enumName);

                // 最終行は改行しない.
                if (i < targetSheets.Length - 1)
                {
                    enums.AppendLine();
                    enums.AppendLine();

                    contents.AppendLine();
                }
            }

            var script = SheetIdTableScriptTemplate;
            script = Regex.Replace(script, "#ENUMS#", enums.ToString());
            script = Regex.Replace(script, "#TABLES#", contents.ToString());

            script = script.FixLineEnd();

            ScriptGenerateUtility.GenerateScript(exportPath, SheetIdTableScriptFileName, script);
        }

        private static string[] GenerateEnumFiles(SheetEntity[] spreadsheets, GameTextConfig config, int textColumn)
        {
            var exportPath = config.EnumScriptFolderPath;
            
            var startRow = config.DefinitionStartRow.GetValueOrDefault();
            var idColumn = config.IdColumn.GetValueOrDefault();
            var enumColumn = config.EnumColumn.GetValueOrDefault();
            var ignoreSheets = config.IgnoreSheets;

            var generatedScripts = new List<string>();

            var targetSheets = spreadsheets.Where(x => !ignoreSheets.Contains(x.Title)).ToArray();

            foreach (var spreadsheet in targetSheets)
            {
                var enums = new StringBuilder();

                for (var i = startRow; i < spreadsheet.Rows.Length; ++i)
                {
                    var idValue = 0;
                    var id = spreadsheet.GetValue(idColumn, i);

                    var name = ScriptGenerateUtility.RemoveInvalidChars(spreadsheet.GetValue(enumColumn, i));

                    // 空文字、数値に変換出来ない値の場合はEnumを追加しない.
                    if (string.IsNullOrEmpty(name) || !int.TryParse(id, out idValue)) { continue; }

                    var text = spreadsheet.GetValue(textColumn, i);

                    var summary = string.Format(SummaryTemplate, string.IsNullOrEmpty(text) ? string.Empty : text.Replace("\r\n", "").Replace("\n", ""));

                    enums.Append("\t\t\t").AppendLine(summary);
                    enums.Append("\t\t\t").AppendFormat(EnumContentsTemplate, name, idValue);

                    // 最終行は改行しない.
                    if (i < spreadsheet.Rows.Length - 1)
                    {
                        enums.AppendLine();
                        enums.AppendLine();
                    }
                }

                var enumName = GetSheetEnumName(spreadsheet, config);

                var script = EnumScriptTemplate;

                script = Regex.Replace(script, "#ENUMNAME#", enumName);
                script = Regex.Replace(script, "#ENUMS#", enums.ToString());

                script = script.FixLineEnd();

                var fileName = string.Format(@"{0}.cs", enumName);

                if (ScriptGenerateUtility.GenerateScript(exportPath, fileName, script))
                {
                    generatedScripts.Add(fileName);
                }
            }

            return generatedScripts.ToArray();
        }

        private static void DeleteUnusedEnumFiles(string[] generatedScripts, GameTextConfig config)
        {
            var exportPath = config.EnumScriptFolderPath;
            var exportFullPath = PathUtility.Combine(UnityPathUtility.GetProjectFolderPath(), exportPath);

            if (AssetDatabase.IsValidFolder(exportPath))
            {
                var files = Directory.GetFiles(exportFullPath, "*", SearchOption.TopDirectoryOnly);

                var deleteTargets = files
                    .Where(x => Path.GetFileName(x) != SheetIdTableScriptFileName)
                    .Where(x => Path.GetExtension(x) != ".meta" && !generatedScripts.Contains(Path.GetFileName(x)))
                    .Select(x => UnityPathUtility.ConvertFullPathToAssetPath(x));

                foreach (var target in deleteTargets)
                {
                    if (Path.GetExtension(target) == ".cs")
                    {
                        AssetDatabase.DeleteAsset(target);
                        UnityConsole.Info("File Delete : {0}", target);
                    }
                }
            }
        }

        private static string GetSheetEnumName(SheetEntity spreadsheet, GameTextConfig config)
        {
            var sheetDefinitionRow = config.SheetDefinitionRow.GetValueOrDefault();
            var sheetNameColumn = config.SheetNameColumn.GetValueOrDefault();

            var sheetEnumName = spreadsheet.GetValue(sheetNameColumn, sheetDefinitionRow);
            var enumName = ScriptGenerateUtility.RemoveInvalidChars(sheetEnumName);

            return enumName;
        }
    }
}