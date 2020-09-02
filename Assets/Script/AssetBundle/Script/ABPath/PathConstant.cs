/****************************************************
	文件：PathConstant.cs
	作者：Lonely
	github：https://github.com/AIValkyries/AssetBundleUpdate
	日期：2020/07/22 19:39:30
	功能：Nothing
*****************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public static class PathConstant
{
    public const string SERVER_FOLDER_NAME = "AssetBundleServer";

    public static class PathName
    {
        public const string DYNAMIC_SCENE_PREFAB_PATH = "Assets/L_Resources/DynamicScenePrefab/";
        public const string DYNAMIC_SCENE_CONFIG_FILE_PATH = "Assets/L_Resources/DynamicSceneConfig/";
    }

    public static class FolderName
    {
        // 平台文件夹名称定义
        public const string WIN = "Win";
        public const string ANDROID = "Android";
        public const string IOS = "IOS";

        // AssetBundleServer 的目录名称定义
        public const string FEN_BAO = "FenBao";
        public const string AB_CACHE = "AllAsset";
        public const string FILE_MANIFEST = "FileManifest";

        public static string GetFenBaoVersion(string version)
        {
            if (version == null || version == string.Empty)
            {
                Debug.LogError("啊!版本号空的!");
                return string.Empty;
            }

            string[] str = version.Split('.');
            int resVersion = System.Convert.ToInt32(str[0]);

            return string.Format("version_{0}", resVersion);
        }

        // 半文件名称 (就是需要和版本号组合的常量)
        public static string GetVersion(string version)
        {
            if (version == null || version == string.Empty) 
            {
                Debug.LogError("啊!版本号空的!");
                return string.Empty;
            }

            string[] str = version.Split('.');
            int resVersion = System.Convert.ToInt32(str[1]);

            return string.Format("version_{0}", resVersion);
        }
    }

    public static class FileName
    {
        // 文件清单名称
        public const string AB_ASSET_INFO = "ABAssetInfo.txt";
        public const string VERSION = "Version.txt";
        public const string VERSION_LIST = "VersionList.txt";
        public const string FEN_BAO = "FenBao.txt";
        public const string ASSETS_MAP_NAME = "AssetMaps.txt";

        public static string GetPackage(string version, int index)
        {
            string[] str = version.Split('.');
            int number = System.Convert.ToInt32(str[0]);

            return string.Format("Package{0}_{1}.zip", number, index);
        }
    }

}


