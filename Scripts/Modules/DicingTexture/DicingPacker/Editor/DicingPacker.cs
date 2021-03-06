﻿
using UnityEngine;
using UnityEditor;
using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using Extensions;
using Extensions.Devkit;
using Modules.Devkit.Generators;
using Modules.Devkit.Prefs;

namespace Modules.Dicing
{
	public partial class DicingPacker : SingletonEditorWindow<DicingPacker>
	{
        //----- params -----

        private static class Prefs
        {
            public static string exportPath
            {
                get { return ProjectPrefs.GetString("DicingPackerPrefs-exportPath", null); }
                set { ProjectPrefs.SetString("DicingPackerPrefs-exportPath", value); }
            }
        }

        private readonly Vector2 WindowSize = new Vector2(400f, 350f);

        private readonly int[] BlockSizes = new int[] { 16, 32, 64, 128 };

        private const int DefaultBlockSize = 32;
        private const int DefaultPadding = 1;

        private const int MB = 1024 * 1024;

        private enum TextureStatus
        {
            None = 0,

            // 既存.
            Exist, 
            // 新規.
            Add,
            // 更新.
            Update,
            // 行方不明.
            Missing,
        }

        private class TextureInfo
        {
            public TextureStatus status;
            public Texture2D texture;
        }

        //----- field -----

        private DicingTexture selectDicingTexture = null;
        private DicingTextureGenerator generator = null;

        private int blockSize = DefaultBlockSize;
        private int padding = DefaultPadding;
        private bool hasAlphaMap = false;
        private FilterMode filterMode = FilterMode.Bilinear;
        private string selectionTextureName = null;
        private TextureInfo[] textureInfos = null;
        private List<string> deleteNames = new List<string>();
        private Vector2 scrollPosition = Vector2.zero;

        private bool calcPerformance = false;
        private float totalMemSize = 0f;
        private float totalAtlasMemSize = 0f;
        private float totalFileSize = 0f;
        private float atlasFileSize = 0f;
        private float infoFileSize = 0f;

        private bool initialized = false;

        public static DicingPacker instance = null;

        //----- property -----

        //----- method -----
        
        public static void Open()
        {
            if(!IsExsist)
            {
                Instance.Initialize();
                instance.Show();
            }
        }

        private void Initialize()
        {
            if(initialized) { return; }

            titleContent = new GUIContent("DicingPacker");
            minSize = WindowSize;

            generator = new DicingTextureGenerator();

            initialized = true;
        }

        void OnEnable()
        {
            instance = this;
            Selection.selectionChanged += OnSelectionUpdate;
        }

        void OnDisable()
        {
            Selection.selectionChanged -= OnSelectionUpdate;
            instance = null;
        }

        private static void OnSelectionUpdate()
        {
            if(instance == null) { return; }

            instance.BuildTextureInfos();
        }

        void OnGUI()
        {
            var selectionTextures = Selection.objects != null ? Selection.objects.OfType<Texture2D>().ToArray() : null;

            GUILayout.Space(2f);

            EditorGUI.BeginChangeCheck();

            selectDicingTexture = EditorLayoutTools.ObjectField(selectDicingTexture, false);

            if (EditorGUI.EndChangeCheck())
            {
                if(selectDicingTexture != null)
                {
                    blockSize = selectDicingTexture.BlockSize;
                    padding = selectDicingTexture.Padding;

                    BuildTextureInfos();
                }
                else
                {
                    blockSize = DefaultBlockSize;
                    padding = DefaultPadding;
                }

                deleteNames.Clear();
            }

            GUILayout.Space(5f);

            DrawSettingsGUI();

            GUILayout.Space(5f);

            if (selectDicingTexture == null)
            {
                DrawCreateGUI(selectionTextures);
            }
            else
            {
                DrawUpdateGUI(selectDicingTexture);
            }
        }

        public void BuildTextureInfos()
        {
            // 選択中テクスチャ.
            var selectionTextures = Selection.objects != null ? 
                Selection.objects.OfType<Texture2D>().ToArray() : 
                new Texture2D[0];

            // パック済みのテクスチャは除外.
            if(selectDicingTexture != null)
            {
                selectionTextures = selectionTextures
                    .Where(x => x != selectDicingTexture.Texture)
                    .ToArray();
            }

            var allDicingSource = selectDicingTexture != null ? 
                selectDicingTexture.GetAllDicingSource() :
                new DicingSourceData[0];

            var textureInfoByGuid = new Dictionary<string, TextureInfo>();

            foreach (var item in allDicingSource)
            {
                if(string.IsNullOrEmpty(item.guid)) { continue; }

                var assetPath = AssetDatabase.GUIDToAssetPath(item.guid);
                var texture = AssetDatabase.LoadMainAssetAtPath(assetPath) as Texture2D;

                var info = new TextureInfo();

                info.status = texture != null ? TextureStatus.Exist : TextureStatus.Missing;

                if (info.status == TextureStatus.Exist)
                {
                    var fullPath = UnityPathUtility.ConvertAssetPathToFullPath(assetPath);
                    var lastUpdate = File.GetLastWriteTime(fullPath).ToUnixTime();

                    if (item.lastUpdate != lastUpdate)
                    {
                        info.status = TextureStatus.Update;
                    }

                    info.texture = texture;
                }

                textureInfoByGuid.Add(item.guid, info);
            }

            foreach (var texture in selectionTextures)
            {
                var assetPath = AssetDatabase.GetAssetPath(texture);
                var guid = AssetDatabase.AssetPathToGUID(assetPath);

                var info = textureInfoByGuid.GetValueOrDefault(guid);

                if(info == null)
                {
                    info = new TextureInfo()
                    {
                        status = TextureStatus.Add,
                        texture = texture,
                    };

                    textureInfoByGuid.Add(guid, info);
                }
                else
                {
                    textureInfoByGuid[guid].status = TextureStatus.Update;
                }
            }

            textureInfos = textureInfoByGuid.Values.ToArray();

            CalcPerformance(selectDicingTexture);

            Repaint();
        }

        private void DrawSettingsGUI()
        {
            var labelWidth = 80f;

            EditorLayoutTools.DrawLabelWithBackground("Settings", EditorLayoutTools.BackgroundColor, EditorLayoutTools.LabelColor);

            using (new EditorGUILayout.HorizontalScope())
            {
                var labels = BlockSizes.Select(x => x.ToString()).ToArray();

                GUILayout.Label("BlockSize", GUILayout.Width(labelWidth));
                blockSize = EditorGUILayout.IntPopup(blockSize, labels, BlockSizes, GUILayout.Width(75f));
            }

            using (new EditorGUILayout.HorizontalScope())
            {
                GUILayout.Label("Padding", GUILayout.Width(labelWidth));
                padding = Mathf.Clamp(EditorGUILayout.IntField(padding, GUILayout.Width(75f)), 1, 5);
            }

            using (new EditorGUILayout.HorizontalScope())
            {
                GUILayout.Label("AlphaMap", GUILayout.Width(labelWidth));
                hasAlphaMap = EditorGUILayout.Toggle(hasAlphaMap, GUILayout.Width(75f));
            }

            using (new EditorGUILayout.HorizontalScope())
            {
                GUILayout.Label("FilterMode", GUILayout.Width(labelWidth));
                filterMode = (FilterMode)EditorGUILayout.EnumPopup(filterMode, GUILayout.Width(75f));
            }
        }

        private void DrawCreateGUI(Texture2D[] selectionTextures)
        {
            var defaultBackgroundColor = GUI.backgroundColor;

            EditorGUILayout.HelpBox("対象のテクスチャを選択してください。", MessageType.Info);

            using (new DisableScope(selectionTextures.IsEmpty()))
            {
                using (new EditorGUILayout.HorizontalScope())
                {
                    GUILayout.Space(20f);

                    GUI.backgroundColor = selectionTextures.Any() ? Color.yellow : Color.white;

                    if (GUILayout.Button("Create"))
                    {
                        GenerateDicingTexture(null);
                    }

                    GUI.backgroundColor = defaultBackgroundColor;

                    GUILayout.Space(20f);
                }
            }
        }

        private void DrawUpdateGUI(DicingTexture dicingTexture)
        {
            var labelStyle = new GUIStyle(EditorStyles.label);

            var defaultColor = labelStyle.normal.textColor;
            var defaultBackgroundColor = GUI.backgroundColor;

            var delete = false;

            if (textureInfos.Any())
            {
                EditorLayoutTools.DrawLabelWithBackground("Sprites", EditorLayoutTools.BackgroundColor, EditorLayoutTools.LabelColor);

                EditorGUILayout.Separator();

                using (new EditorGUILayout.VerticalScope())
                {
                    using (var scrollViewScope = new EditorGUILayout.ScrollViewScope(scrollPosition))
                    {
                        int index = 0;

                        for (var i = 0; i < textureInfos.Length; i++)
                        {
                            ++index;

                            GUILayout.Space(-1f);

                            var textureName = textureInfos[i].texture != null ? textureInfos[i].texture.name : null;

                            var highlight = selectionTextureName == textureName;

                            GUI.backgroundColor = highlight ? Color.white : new Color(0.8f, 0.8f, 0.8f);

                            using (new EditorGUILayout.HorizontalScope(EditorLayoutTools.TextAreaStyle, GUILayout.MinHeight(20f)))
                            {
                                GUI.backgroundColor = Color.white;
                                GUILayout.Label(index.ToString(), GUILayout.Width(24f));

                                if (GUILayout.Button(textureName, EditorStyles.label, GUILayout.Height(20f)))
                                {
                                    selectionTextureName = textureName;
                                }

                                switch (textureInfos[i].status)
                                {
                                    case TextureStatus.Add:
                                        labelStyle.normal.textColor = Color.green;
                                        GUILayout.Label("Add", labelStyle, GUILayout.Width(27f));
                                        break;  
                                        
                                    case TextureStatus.Update:
                                        labelStyle.normal.textColor = Color.cyan;
                                        GUILayout.Label("Update", labelStyle, GUILayout.Width(45f));
                                        break; 
                                    case TextureStatus.Missing:
                                        labelStyle.normal.textColor = Color.yellow;
                                        GUILayout.Label("Missing", labelStyle, GUILayout.Width(45f));
                                        break;
                                }

                                labelStyle.normal.textColor = defaultColor;

                                if (deleteNames.Contains(textureName))
                                {
                                    GUI.backgroundColor = Color.red;

                                    if (GUILayout.Button("Delete", GUILayout.Width(60f)))
                                    {
                                        delete = true;
                                    }

                                    GUI.backgroundColor = Color.green;

                                    if (GUILayout.Button("X", GUILayout.Width(22f)))
                                    {
                                        deleteNames.Remove(textureName);
                                    }
                                }
                                else
                                {
                                    if (GUILayout.Button("X", GUILayout.Width(22f)))
                                    {
                                        if (!deleteNames.Contains(textureName))
                                        {
                                            deleteNames.Add(textureName);
                                        }
                                    }
                                }

                                GUILayout.Space(5f);
                            }
                        }

                        scrollPosition = scrollViewScope.scrollPosition;
                    }
                }
            }
            
            if(calcPerformance)
            {
                GUILayout.Space(5f);

                EditorLayoutTools.DrawLabelWithBackground("Result", EditorLayoutTools.BackgroundColor, EditorLayoutTools.LabelColor);

                using (new EditorGUILayout.HorizontalScope())
                {
                    GUILayout.Label("MemorySize", GUILayout.Width(75f));

                    var dicingMemSize = infoFileSize + totalAtlasMemSize;

                    labelStyle.normal.textColor = totalMemSize < dicingMemSize ? Color.red : defaultColor;
                    GUILayout.Label(string.Format("{0:F1} MB >>> {1:F1} MB : {2:F1}% ", totalMemSize, dicingMemSize, 100.0f * dicingMemSize / totalMemSize), labelStyle);
                    labelStyle.normal.textColor = defaultColor;
                }

                using (new EditorGUILayout.HorizontalScope())
                {
                    GUILayout.Label("FileSize", GUILayout.Width(75f));

                    var dicingFileSize = infoFileSize + atlasFileSize;

                    labelStyle.normal.textColor = totalFileSize < atlasFileSize ? Color.red : defaultColor;
                    GUILayout.Label(string.Format("{0:F1} MB >>> {1:F1} MB : {2:F1}% ", totalFileSize, dicingFileSize, 100.0f * dicingFileSize / totalFileSize), labelStyle);
                    labelStyle.normal.textColor = defaultColor;
                }
            }

            GUILayout.Space(15f);

            using (new EditorGUILayout.HorizontalScope())
            {
                GUI.backgroundColor = Color.cyan;

                GUILayout.FlexibleSpace();

                if (GUILayout.Button("Apply", GUILayout.Width(150f)))
                {
                    GenerateDicingTexture(dicingTexture);
                }

                GUI.backgroundColor = defaultBackgroundColor;

                GUILayout.Space(25f);

                if (GUILayout.Button("View Textures", GUILayout.Width(150f)))
                {
                    Action<string> onSelection = x =>
                    {
                        selectionTextureName = x;
                        Repaint();
                    };

                    DicingSpriteSelector.Show(dicingTexture, selectionTextureName, onSelection, null);
                }

                GUILayout.FlexibleSpace();
            }

            if(delete)
            {
                GenerateDicingTexture(dicingTexture);
            }

            GUILayout.Space(5f);
        }

        private void CalcPerformance(DicingTexture dicingTexture)
        {
            calcPerformance = false;

            if (dicingTexture != null)
            {
                var assetPath = AssetDatabase.GetAssetPath(dicingTexture);
                var fullPath = UnityPathUtility.ConvertAssetPathToFullPath(assetPath);

                var fileInfo = new FileInfo(fullPath);

                infoFileSize = (float)fileInfo.Length / MB;
            }
            else
            {
                return;
            }

            var textures = dicingTexture.GetAllDicingSource()
                .Select(x => AssetDatabase.GUIDToAssetPath(x.guid))
                .Select(x => AssetDatabase.LoadMainAssetAtPath(x) as Texture2D)
                .Where(x => x != null)
                .ToArray();

            // 消費メモリサイズを計測.
            totalMemSize = 0;
            textures.ForEach(x =>
                {
                    var mem = Mathf.NextPowerOfTwo(x.width) * Mathf.NextPowerOfTwo(x.height);
                    mem *= !x.alphaIsTransparency ? 3 : 4;
                    totalMemSize += mem;
                });
            totalMemSize /= MB;

            if (dicingTexture.Texture != null)
            {
                var mem = Mathf.NextPowerOfTwo(dicingTexture.Texture.width) * Mathf.NextPowerOfTwo(dicingTexture.Texture.height);
                mem *= !dicingTexture.Texture.alphaIsTransparency ? 3 : 4;
                totalAtlasMemSize = (float)mem / MB;
            }

            // ファイルサイズ.
            totalFileSize = 0f;
            textures.Select(x => AssetDatabase.GetAssetPath(x))
                .Select(x => UnityPathUtility.ConvertAssetPathToFullPath(x))
                .Select(x => new FileInfo(x))
                .ForEach(x => totalFileSize += (float)x.Length / MB);

            if (dicingTexture.Texture != null)
            {
                var assetPath = AssetDatabase.GetAssetPath(dicingTexture.Texture);
                var fullPath = UnityPathUtility.ConvertAssetPathToFullPath(assetPath);

                var fileInfo = new FileInfo(fullPath);

                atlasFileSize = (float)fileInfo.Length / MB;
            }

            calcPerformance = true;
        }

        private void GenerateDicingTexture(DicingTexture target)
        {
            var exportPath = string.Empty;

            var textures = textureInfos
                .Where(x => x.status == TextureStatus.Exist ||
                            x.status == TextureStatus.Add ||
                            x.status == TextureStatus.Update)
                .Where(x => !deleteNames.Contains(x.texture.name))
                .Select(x => x.texture)
                .ToArray();

            if (target == null)
            {
                var path = string.Empty;

                if (string.IsNullOrEmpty(Prefs.exportPath) || !Directory.Exists(path))
                {
                    path = UnityPathUtility.AssetsFolder;
                }
                else
                {
                    path = Prefs.exportPath;
                }

                exportPath = EditorUtility.SaveFilePanelInProject("Save As", "New DicingTexture.asset", "asset", "Save as...", path);
            }
            else
            {
                exportPath = AssetDatabase.GetAssetPath(target);
            }

            if (!string.IsNullOrEmpty(exportPath))
            {
                var dicingData = generator.Generate(exportPath, blockSize, padding, textures, hasAlphaMap);

                target = ScriptableObjectGenerator.Generate<DicingTexture>(exportPath);

                target.Set(dicingData.Texture, blockSize, padding, dicingData.SourceTextures, dicingData.DicingBlocks, hasAlphaMap);
                target.Texture.filterMode = filterMode;

                UnityEditorUtility.SaveAsset(target);

                Prefs.exportPath = exportPath;

                selectDicingTexture = target;

                BuildTextureInfos();

                deleteNames.Clear();

                Repaint();
            }
        }
    }
}
