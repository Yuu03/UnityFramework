﻿﻿﻿﻿
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
using System;
using System.Linq;
using System.Collections.Generic;
using UniRx;
using Extensions;
using Extensions.Devkit;

namespace Modules.Devkit.CleanComponent
{
    public static class ComponentCleaner
	{
        public static void Execute()
        {
            SceneTextComponentCleaner.Clean();
            SceneImageComponentCleaner.Clean();

            InternalEditorUtility.RepaintAllViews();

            Debug.Log("Finish clean component.");
        }
	}
}
