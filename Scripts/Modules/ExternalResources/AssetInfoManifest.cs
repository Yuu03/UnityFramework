﻿
using UnityEngine;
using System;
using System.Linq;
using System.Collections.Generic;
using System.IO;
using UniRx;
using Extensions;
using Modules.AssetBundles;

#if ENABLE_CRIWARE

using Modules.CriWare;

#endif

namespace Modules.ExternalResource
{
    [Serializable]
    public class AssetInfo
    {
        //----- params -----

        //----- field -----

        [SerializeField]
        private string resourcesPath = null;
        [SerializeField]
        private string groupName = null;
        [SerializeField]
        private long fileSize = 0;
        [SerializeField]
        private string fileHash = null;
        [SerializeField]
        private AssetBundleInfo assetBundle = null;

        //----- property -----

        /// <summary> 読み込みパス </summary>
        public string ResourcesPath { get { return resourcesPath; } }
        /// <summary> グループ名 </summary>
        public string GroupName { get { return groupName; } }
        /// <summary> ファイルサイズ(byte) </summary>
        public long FileSize { get { return fileSize; } }
        /// <summary> ファイルハッシュ </summary>
        public string FileHash { get { return fileHash; } }
        /// <summary> アセットバンドル情報 </summary>
        public AssetBundleInfo AssetBundle { get { return assetBundle; } }
        
        /// <summary> アセットバンドルか </summary>
        public bool IsAssetBundle
        {
            get { return assetBundle != null && !string.IsNullOrEmpty(assetBundle.AssetBundleName); }
        }

        //----- method -----

        public AssetInfo(string resourcesPath, string groupName)
        {
            this.resourcesPath = resourcesPath;
            this.groupName = groupName;
        }

        public void SetFileInfo(string filePath)
        {
            if (File.Exists(filePath))
            {
                var fileInfo = new FileInfo(filePath);

                fileSize = fileInfo.Length;
                fileHash = FileUtility.GetHash(filePath);
            }
            else
            {
                Debug.LogErrorFormat("File not exists.\nFile : {0}", filePath);
            }
        }

        public void SetAssetBundleInfo(AssetBundleInfo assetBundleInfo)
        {
            assetBundle = assetBundleInfo;
        }
    }

    [Serializable]
    public class AssetBundleInfo
    {
        //----- params -----

        //----- field -----

        [SerializeField]
        private string assetBundleName = null;
        [SerializeField]
        private bool compress = false;
        [SerializeField]
        private string[] dependencies = null;

        //----- property -----

        /// <summary> アセットバンドル名 </summary>
        public string AssetBundleName { get { return assetBundleName; } }
        /// <summary> 圧縮 </summary>
        public bool Compress { get { return compress; } }
        /// <summary> 依存関係 </summary>
        public string[] Dependencies { get { return dependencies; } }

        //----- method -----

        public AssetBundleInfo(string assetBundleName)
        {
            this.assetBundleName = assetBundleName;

            SetCompress(false);
            SetDependencies(null);
        }

        public void SetCompress(bool compress)
        {
            this.compress = compress;
        }

        public void SetDependencies(string[] dependencies)
        {
            this.dependencies = dependencies ?? new string[0];
        }
    }

    public class AssetInfoManifest : ScriptableObject
    {
        //----- params -----

        public const string ManifestFileName = "AssetInfoManifest.asset";

        //----- field -----

        [SerializeField, ReadOnly]
        private AssetInfo[] assetInfos = new AssetInfo[0];

        private ILookup<string, AssetInfo> assetInfoByGroupName = null;
        private Dictionary<string, AssetInfo> assetInfoByResourcesPath = null;

        //----- property -----

        public static string AssetBundleName
        {
            get { return Path.GetFileNameWithoutExtension(ManifestFileName).ToLower(); }
        }

        //----- method -----

        public AssetInfoManifest(AssetInfo[] assetInfos)
        {
            this.assetInfos = assetInfos;
        }

        public IEnumerable<AssetInfo> GetAssetInfos(string groupName = null)
        {
            BuildCache();

            if (string.IsNullOrEmpty(groupName))
            {
                return assetInfos;
            }

            if (!assetInfoByGroupName.Contains(groupName))
            {
                return new AssetInfo[0];
            }

            return assetInfoByGroupName
                .Where(x => x.Key == groupName)
                .SelectMany(x => x);
        }

        public AssetInfo GetAssetInfo(string resourcesPath)
        {
            BuildCache();

            var info = assetInfoByResourcesPath.GetValueOrDefault(resourcesPath);

            return info;
        }

        public void SetAssetFileInfo(string exportPath, IProgress<Tuple<string, float>> progress = null)
        {
            for (var i = 0; i < assetInfos.Length; i++)
            {
                var assetInfo = assetInfos[i];

                var elements = new string[0];

                if (assetInfo.IsAssetBundle)
                {
                    var assetBundleName = assetInfo.AssetBundle.AssetBundleName;

                    elements = new string[] { exportPath, AssetBundleManager.AssetBundlesFolder, assetBundleName };
                }
                else
                {
                    #if ENABLE_CRIWARE

                    var extension = Path.GetExtension(assetInfo.ResourcesPath);

                    if (CriAssetDefinition.AssetAllExtensions.Any(x => x == extension))
                    {
                        elements = new string[] { exportPath, CriAssetDefinition.CriAssetFolder, assetInfo.ResourcesPath };
                    }

                    #endif
                }

                if (elements.IsEmpty()) { continue; }

                var filePath = PathUtility.Combine(elements);

                assetInfo.SetFileInfo(filePath);

                progress.Report(Tuple.Create(assetInfo.ResourcesPath, (float)i / assetInfos.Length));
            }

            BuildCache(true);
        }

        public void SetAssetBundleInfo(string exportPath, AssetBundleManifest assetBundleManifest, IProgress<Tuple<string, float>> progress = null)
        {
            const long CompressFileSize = 100 * 1024; // 100kbyteは圧縮.

            for (var i = 0; i < assetInfos.Length; i++)
            {
                var assetInfo = assetInfos[i];

                if (!assetInfo.IsAssetBundle) { continue; }

                var assetBundleName = assetInfo.AssetBundle.AssetBundleName;
                var dependencies = assetBundleManifest.GetAllDependencies(assetBundleName);

                var assetBundleInfo = new AssetBundleInfo(assetBundleName);

                assetBundleInfo.SetCompress(CompressFileSize <= assetInfo.FileSize);
                assetBundleInfo.SetDependencies(dependencies);

                assetInfo.SetAssetBundleInfo(assetBundleInfo);

                progress.Report(Tuple.Create(assetInfo.ResourcesPath, (float)i / assetInfos.Length));
            }

            BuildCache(true);
        }

        private void BuildCache(bool forceUpdate = false)
        {
            if (assetInfoByGroupName == null || forceUpdate)
            {
                assetInfoByGroupName = assetInfos.ToLookup(x => x.GroupName);
            }

            if (assetInfoByResourcesPath == null || forceUpdate)
            {
                assetInfoByResourcesPath = assetInfos.ToDictionary(x => x.ResourcesPath);
            }
        }
    }
}
