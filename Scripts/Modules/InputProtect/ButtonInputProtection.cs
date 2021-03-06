﻿
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Linq;
using System.Collections.Generic;
using UniRx;
using Extensions;
using Modules.UI;
using Unity.Linq;

namespace Modules.InputProtection.Components
{
    [RequireComponent(typeof(Button))]
    public class ButtonInputProtection : InputProtection
    {
        //----- params -----

        //----- field -----

        private Button target = null;

        private List<Graphic> raycastGraphics = null;

        //----- property -----

        //----- method -----

        protected override void UpdateProtect(bool isProtect)
        {
            if (raycastGraphics == null)
            {
                raycastGraphics = new List<Graphic>();
            }

            if (target == null)
            {
                target = UnityUtility.GetComponent<Button>(gameObject);
            }

            // interactiveの状態でTintColorが変わってしまうのを防ぐ.
            if (target.targetGraphic != null)
            {
                if (isProtect)
                {
                    var graphics = target.targetGraphic.gameObject
                        .DescendantsAndSelf()
                        .OfComponent<Graphic>()
                        .ToArray();

                    raycastGraphics.Clear();

                    foreach (var graphic in graphics)
                    {
                        if (graphic.raycastTarget)
                        {
                            raycastGraphics.Add(graphic);
                        }

                        graphic.raycastTarget = false;
                    }
                }
                else
                {
                    foreach (var raycastGraphic in raycastGraphics)
                    {
                        raycastGraphic.raycastTarget = true;
                    }
                }
            }
        }
    }
}