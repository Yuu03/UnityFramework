﻿
using UnityEngine;
using UnityEditor;
using UnityEditor.Animations;
using System;
using System.IO;
using System.Linq;
using UniRx;
using Extensions;
using Extensions.Devkit;
using Modules.Devkit.AssetTuning;

namespace Modules.Devkit.Animator
{
	public class EmptyAnimatorAssetTuner : IAssetTuner
    {
        //----- params -----

        //----- field -----

        //----- property -----

        public int Priority { get { return 50; } }

        //----- method -----

        [InitializeOnLoadMethod]
        private static void InitializeOnLoadMethod()
        {
            AssetTuner.Instance.Register<EmptyAnimatorAssetTuner>();
        }

        public bool Validate(string path)
        {
            if (Path.GetExtension(path) != ".controller") { return false; }

            var asset = AssetDatabase.LoadMainAssetAtPath(path);

            var controller = asset as AnimatorController;

            if (controller == null) { return false; }

            if (controller.parameters.Any()) { return false; }

            foreach (var layer in controller.layers)
            {
                var stateMachine = layer.stateMachine;

                if (stateMachine.states.Any()) { return false; }                
                if (stateMachine.anyStateTransitions.Any()) { return false; }
                if (stateMachine.entryTransitions.Any()) { return false; }                
                if (stateMachine.stateMachines.Any()) { return false; }
                if (stateMachine.behaviours.Any()) { return false; }
            }

            return true;
        }

        public void OnAssetCreate(string path)
        {
            RegisterTuneAssetCallback(path);
        }

        public void OnAssetImport(string path) { }

        public void OnAssetDelete(string path){ }

        public void OnAssetMove(string path, string from) { }

        private void RegisterTuneAssetCallback(string path)
        {
            // ※ Importされた時点ではAnimatorの初期化が終わっていないのでImport完了後に処理を実行する.

            EditorApplication.CallbackFunction tuningCallbackFunction = null;

            tuningCallbackFunction = () =>
            {
                TuneAsset(path);

                if (tuningCallbackFunction != null)
                {
                    EditorApplication.delayCall -= tuningCallbackFunction;
                }
            };

            EditorApplication.delayCall += tuningCallbackFunction;
        }

        protected static void TuneAsset(string path)
        {
            var asset = AssetDatabase.LoadMainAssetAtPath(path);

            var controller = asset as AnimatorController;

            if (controller == null) { return; }

            var rootStateMachine = controller.layers[0].stateMachine;

            // Add DefaultState.

            var defaultState = rootStateMachine.AddState("Default");

            var state = rootStateMachine.states.FirstOrDefault(x => x.state == defaultState);

            state.position = new Vector2(50f, 50f);

            // Set EntryTransition.

            rootStateMachine.AddEntryTransition(defaultState);

            // Set Position.

            rootStateMachine.entryPosition = new Vector2(0f, 0f);
            rootStateMachine.anyStatePosition = new Vector2(0f, 50f);
            rootStateMachine.exitPosition = new Vector2(0f, 100f);

            UnityEditorUtility.SaveAsset(controller);

            Debug.Log("TuneAsset!");
        }
    }
}
