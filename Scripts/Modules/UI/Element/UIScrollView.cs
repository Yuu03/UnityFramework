﻿﻿﻿﻿
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Linq;
using System.Collections.Generic;
using UniRx;
using Extensions;

namespace Modules.UI.Element
{
    [ExecuteInEditMode]
    [RequireComponent(typeof(ScrollRect))]
    public abstract class UIScrollView : UIElement<ScrollRect>
    {
        //----- params -----

        //----- field -----

        private Vector2? position = null;
        private IDisposable lockPositionDisposable = null;

        //----- property -----

        public ScrollRect ScrollRect { get { return component; } }

        //----- method -----

        public override void Modify()
        {

        }

        public void LockPosition()
        {
            if (position.HasValue) { return; }

            position = ScrollRect.normalizedPosition;

            lockPositionDisposable = Observable.EveryUpdate().Subscribe(_ => ScrollRect.normalizedPosition = position.Value).AddTo(this);
        }

        public void UnLockPosition()
        {
            if (position.HasValue)
            {
                ScrollRect.normalizedPosition = position.Value;
                position = null;
            }

            if (lockPositionDisposable != null)
            {
                lockPositionDisposable.Dispose();
                lockPositionDisposable = null;
            }
        }

        public void ResetPosition()
        {
            if (ScrollRect.horizontal)
            {
                ScrollRect.horizontalNormalizedPosition = 0f;
            }

            if (ScrollRect.vertical)
            {
                ScrollRect.verticalNormalizedPosition = 1f;
            }
        }
    }
}