﻿﻿
using UnityEngine;
using System;
using System.Linq;
using System.Collections.Generic;
using Extensions;
using UniRx;

namespace Modules.UI
{
    public abstract class VirtualScrollItem<T> : MonoBehaviour
    {
        //----- params -----

        //----- field -----

        private RectTransform rectTransform = null;

        //----- property -----

        public int Index { get; private set; }

        public RectTransform RectTransform
        {
            get { return rectTransform ?? (rectTransform = UnityUtility.GetComponent<RectTransform>(gameObject)); }
        }

        //----- method -----

        public virtual IObservable<Unit> Initialize() { return Observable.ReturnUnit(); }

        public void UpdateItem(int index, T[] contents)
        {
            Index = index;

            if (contents != null)
            {
                if (0 <= index && index < contents.Length)
                {
                    UpdateContents(contents[index]);
                }
            }
        }

        protected abstract void UpdateContents(T content);
    }
}