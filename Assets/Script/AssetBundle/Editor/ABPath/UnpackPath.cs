/****************************************************
	文件：UnpackPath.cs
	作者：Lonely
	github：https://github.com/AIValkyries/AssetBundleUpdate
	日期：2020/07/22 19:36:51
	功能：Nothing
*****************************************************/

using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;


public static class UnpackPath
{
    public static string Get()
    {
        string dataPath = Application.streamingAssetsPath;
        string osFolderName = AssetsCommon.GetPlatformFolderName(UnpackCommon.GetOsType());

        List<string> folders = new List<string>()
        {
            osFolderName
        };

        string path = AssetsCommon.BuildPath(dataPath, folders);
        if (!Directory.Exists(path))
            Directory.CreateDirectory(path);

        return path;
    }

    public static string GetABFile(string assetName)
    {
        ePlatformType osType = UnpackCommon.GetOsType();

        string dataPath = Application.streamingAssetsPath;
        string osFolderName = AssetsCommon.GetPlatformFolderName(osType);

        List<string> folders = new List<string>()
        {
            osFolderName
        };
        string path = AssetsCommon.BuildPath(dataPath, folders);
        if (!Directory.Exists(path))
            Directory.CreateDirectory(path);

        return AssetsCommon.BuildFileName(path, assetName);
    }

    public static string GetMainAssetBundleName()
    {
        string path = Get();
        string osFolderName = AssetsCommon.GetPlatformFolderName(UnpackCommon.GetOsType());

        return string.Format("{0}/{1}", path, osFolderName);
    }

}
