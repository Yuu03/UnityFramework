﻿
#if ENABLE_CRIWARE

using UnityEngine;
using System;
using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UniRx;
using Utage;
using Extensions;
using Modules.ExternalResource;
using Modules.SoundManagement;

namespace Modules.UtageExtension
{
    public class ExternalResourcesSoundAssetFile : AssetFileBase
    {
        //----- params -----

        //----- field -----

        private string resourcesPath = null;
        private string soundName = null;

        //----- property -----
        
        public CueInfo CueInfo { get; private set; }

        //----- method -----

        public ExternalResourcesSoundAssetFile(AssetFileManager mangager, AssetFileInfo fileInfo, IAssetFileSettingData settingData) : base(mangager, fileInfo, settingData)
        {
            var setting = settingData as IAssetFileSoundSettingData;

            if (setting != null)
            {
                resourcesPath = settingData.RowData.ParseCellOptional<string>("FileName", null);

                if (setting is AdvVoiceSetting)
                {                    
                    soundName = settingData.RowData.ParseCellOptional<string>("Voice", null);
                }
                else
                {
                    soundName = settingData.RowData.ParseCellOptional<string>("SoundName", null);
                }
            }
        }

        public override bool CheckCacheOrLocal()
        {
            return true;
        }

        public override IEnumerator LoadAsync(Action onComplete, Action onFailed)
        {
            if (string.IsNullOrEmpty(resourcesPath))
            {
                onFailed();
                yield break;
            }

            var updateYield = ExternalResources.UpdateAsset(resourcesPath).ToYieldInstruction(false);

            yield return updateYield;

            if (updateYield.HasError)
            {
                onFailed();
                yield break;
            }

            if (Priority != AssetFileLoadPriority.DownloadOnly)
            {
                CueInfo = ExternalResources.GetCueInfo(resourcesPath, soundName);

                if (CueInfo == null)
                {
                    IsLoadError = true;
                    onFailed();

                    yield break;
                }
            }

            IsLoadEnd = true;
            onComplete();
        }

        public override void Unload()
        {
            IsLoadEnd = false;
        }
    }
}

#endif
