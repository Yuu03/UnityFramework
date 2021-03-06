﻿#if ENABLE_CRIWARE
﻿﻿
using UnityEngine;
using System;
using System.Linq;
using System.Collections.Generic;
using UniRx;
using Extensions;
using CriMana;

namespace Modules.MovieManagement
{
	public class MovieElement
	{
        //----- params -----

        //----- field -----

        private Player manaPlayer = null;
        private Subject<Unit> onFinish = null;

        //----- property -----

        public string MoviePath { get; private set; }
        public Player.Status? Status { get; private set; }

        //----- method -----

        public MovieElement(Player manaPlayer, string moviePath)
        {
            this.manaPlayer = manaPlayer;

            MoviePath = moviePath;
            Status = manaPlayer.status;
        }

        public void Update()
        {
            if (onFinish != null)
            {
                if (Status != manaPlayer.status && manaPlayer.status == Player.Status.PlayEnd)
                {
                    onFinish.OnNext(Unit.Default);
                }
            }

            Status = manaPlayer.status;
        }
        
        public Player GetPlayer()
        {
            return manaPlayer;
        }

        public IObservable<Unit> OnFinishAsObservable()
        {
            return onFinish ?? (onFinish = new Subject<Unit>());
        }
    }
}

#endif