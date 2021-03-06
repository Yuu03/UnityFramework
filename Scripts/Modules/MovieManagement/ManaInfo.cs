﻿﻿
#if ENABLE_CRIWARE
﻿﻿
using UnityEngine;
using System.IO;
using Modules.CriWare;
using Extensions;

namespace Modules.MovieManagement
{
    public class ManaInfo
    {
        public string Usm { get; private set; }
        public string UsmPath { get; private set; }

        public ManaInfo(string path)
        {
            this.Usm = path;
            this.UsmPath = PathUtility.GetPathWithoutExtension(path);
        }
    }
}

#endif