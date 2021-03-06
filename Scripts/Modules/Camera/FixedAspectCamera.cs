﻿﻿﻿
using UnityEngine;
using System;
using System.Collections;

namespace Modules.FixedAspectCamera
{
    [ExecuteInEditMode]
    [RequireComponent(typeof(Camera))]
    public abstract class FixedAspectCamera : MonoBehaviour
    {
        //----- params -----

        //----- field -----

        private float fixedWidth = 0f;
        private float fixedHeight = 0f;

        private Camera target = null;
        private float aspectRate = 0f;

        //----- property -----

        public abstract float FixedWidth { get; }
        public abstract float FixedHeight { get; }

        //----- method -----

        void Awake()
        {
            aspectRate = fixedWidth / fixedHeight;

            target = gameObject.GetComponent<Camera>();

            UpdateScreenRate();
        }

        void Update()
        {
            if (IsChangeAspect()) { return; }

            UpdateScreenRate();
        }

        private void UpdateScreenRate()
        {
            float nowAspect = (float)Screen.width / (float)Screen.height;
            float changeAspect;

            if (aspectRate > nowAspect)
            {
                changeAspect = nowAspect / aspectRate;
                target.rect = new Rect((1 - changeAspect) * 0.5f, 0, changeAspect, 1);
            }
            else
            {
                changeAspect = aspectRate / nowAspect;
                target.rect = new Rect(0, (1 - changeAspect) * 0.5f, 1, changeAspect);
            }

            target.ResetAspect();
        }

        private bool IsChangeAspect()
        {
            return target.aspect == aspectRate;
        }
    }
}
