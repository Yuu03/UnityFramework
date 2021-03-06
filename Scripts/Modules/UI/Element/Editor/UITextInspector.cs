﻿﻿﻿﻿﻿
using UnityEngine;
using UnityEditor;
using System;
using System.Linq;
using System.Collections.Generic;
using UniRx;
using Extensions;
using Extensions.Devkit;
using Extensions.Serialize;

namespace Modules.UI.Element
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(UIText), true)]
    public class UITextInspector : UnityEditor.Editor
    {
        //----- params -----

        //----- field -----

        //----- property -----

        //----- method -----

        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            var instance = target as UIText;

            var selection = Reflection.GetPrivateField<UIText, IntNullable>(instance, "selection");
            var colorInfos = Reflection.GetPrivateProperty<UIText, UIText.TextColor[]>(instance, "ColorInfos");

            var infos = new UIText.TextColor[] { new UIText.TextColor() }.Concat(colorInfos).ToArray();
            var select = selection.HasValue ? infos.IndexOf(x => x.Type == selection.Value) : 0;

            var current = infos[select];

            using (new EditorGUILayout.HorizontalScope())
            {
                var labels = infos.Select(x => x.LabelName).ToArray();

                EditorGUI.BeginChangeCheck();

                select = EditorGUILayout.Popup("Text Color", select, labels);

                if (EditorGUI.EndChangeCheck())
                {
                    UnityEditorUtility.RegisterUndo("TextColorSetterInspector-Undo", instance);

                    current = infos[select];
                    instance.SetColor(current.Type);
                }
            }

            if (current != null)
            {
                if (current.ShadowColor.HasValue)
                {
                    EditorGUILayout.HelpBox("Shadow color exists", MessageType.Info);
                }

                if (current.OutlineColor.HasValue)
                {
                    EditorGUILayout.HelpBox("Outline color exists", MessageType.Info);
                }
            }
        }
    }
}