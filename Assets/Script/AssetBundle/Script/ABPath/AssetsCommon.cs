/****************************************************
	文件：ABPath.cs
	作者：Lonely
	github：https://github.com/AIValkyries/AssetBundleUpdate
	日期：2020/07/12 19:14:24
	功能：Nothing
*****************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Text;

public enum ePlatformType
{
    Android,
    IOS,
    Win
}

public class ABPathInfo
{
    public ePlatformType OsType;
    public string Version;

    public override string ToString()
    {
        return string.Format("{0}/{1}", OsType, Version);
    }

    public ABPathInfo() { }

    public ABPathInfo(ePlatformType osType)
    {
        OsType = osType;
        Version = string.Empty;
    }

    public ABPathInfo(ePlatformType type, string version)
    {
        OsType = type;
        Version = version;
    }
}


public class AssetsCommon
{
    public static ePlatformType GetPlatform()
    {
        if (Application.platform == RuntimePlatform.Android)
        {
            return ePlatformType.Android;
        }
        else if (Application.platform == RuntimePlatform.WindowsPlayer)
        {
            return ePlatformType.Win;
        }
        else if (Application.platform == RuntimePlatform.IPhonePlayer) 
        {
            return ePlatformType.IOS;
        }
        return ePlatformType.Win;
    }

    public static string GetAssetPath()
    {
        string dataPath = Application.dataPath;
        dataPath = dataPath.Substring(0, dataPath.Length - "/Assets".Length);
        return dataPath;
    }

    public static string GetPlatformFolderName(ePlatformType osType)
    {
        switch (osType)
        {
            case ePlatformType.Android:
                return PathConstant.FolderName.ANDROID;
            case ePlatformType.IOS:
                return PathConstant.FolderName.IOS;
            case ePlatformType.Win:
                return PathConstant.FolderName.WIN;
        }
        return string.Empty;
    }

    // 构建路径，folder按顺序构建哦
    public static string BuildPath(string path, List<string> folders)
    {
        if (folders.Count <= 0)
        {
            Debug.LogError("文件夹名称集合错误");
            return string.Empty;
        }

        StringBuilder stringBuilder = new StringBuilder();
        stringBuilder.Append(path);
        stringBuilder.Append("/");

        for (int i = 0; i < folders.Count; i++)
        {
            stringBuilder.Append(folders[i]);
            if (i != folders.Count - 1)
                stringBuilder.Append("/");
        }

        return stringBuilder.ToString();
    }

    public static string BuildFileName(string path, string fileName)
    {
        StringBuilder stringBuilder = new StringBuilder();
        stringBuilder.Append(path);
        stringBuilder.Append("/");
        stringBuilder.Append(fileName);
        return stringBuilder.ToString();
    }



    public static string GetAssetLoadPath()
    {
#if UNITY_EDITOR
        return Application.streamingAssetsPath + "/" + GetPlatformFolderName(GetPlatform());
#elif UNITY_IOS
 return GetLocalAssetPath();
#elif UNITY_ANDROID
 return GetLocalAssetPath();
#elif UNITY_STANDALONE_WIN
 return GetLocalAssetPath();
#endif
    }

    static string _local_asset_path = string.Empty;
    public static string LocalAssetPath
    {
        get
        {
            if (_local_asset_path == string.Empty) 
                _local_asset_path = GetLocalAssetPath();
            return _local_asset_path;
        }
    }

    static string GetLocalAssetPath()
    {
#if UNITY_STANDALONE_WIN
        string path = Application.dataPath + "/PersistentAssets";
        if (!System.IO.Directory.Exists(path))
        {
            Directory.CreateDirectory(path);
        }

     return path;

#elif UNITY_IOS
        string path = Application.persistentDataPath;
    if (!System.IO.Directory.Exists(path))
    {
       Directory.CreateDirectory(path);
    }

   return Application.persistentDataPath;
 
#elif UNITY_ANDROID

     string path = Application.persistentDataPath;
     if (!System.IO.Directory.Exists(path))
     {
        Directory.CreateDirectory(path);
     }

   return Application.persistentDataPath;
#endif

    }



}
