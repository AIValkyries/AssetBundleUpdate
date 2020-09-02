/****************************************************
		文件：AssetBundleCommon.cs
		作者：Lonely
		github：https://github.com/AIValkyries/AssetBundleUpdate
		日期：2020/06/28 20:20:16
		功能：Nothing
*****************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System.IO;


public static class UnpackCommon
{
    public static BuildTarget Target { get; set; }

    static AssetBundleSettingInfo _setting_info;

    public static AssetBundleSettingInfo GetSettingInfo()
    {        
        if (_setting_info == null)
        {
            _setting_info = new AssetBundleSettingInfo();
            _setting_info.LoadToFile();
        }
        return _setting_info;
    }

    public static void ClearAssetBundleSettingInfo()
    {
        _setting_info = null;
    }

    public static string GetVersion()
    {
        return GetSettingInfo().Version;
    }

    public static int GetFenBaoVersion()
    {
        string[] splits = GetSettingInfo().Version.Split('.');
        if (splits == null)
            return 0;
        return System.Convert.ToInt32(splits[0]);
    }

    public static int GetAssetUpdateVersion()
    {
        string[] splits = GetSettingInfo().Version.Split('.');
        if (splits == null) 
            return 0;
        return System.Convert.ToInt32(splits[1]);
    }

    public static ePlatformType GetOsType()
    {
        if (Target == BuildTarget.StandaloneWindows)
        {
            return ePlatformType.Win;
        }
        else if (Target == BuildTarget.Android)
        {
            return ePlatformType.Android;
        }
        else if (Target ==  BuildTarget.iOS)
        {
            return ePlatformType.IOS;
        }
        return ePlatformType.Win;
    }

    public static ABPathInfo DefaultABPath
    {
        get { return new ABPathInfo(GetOsType(), GetVersion()); }
    }
        


}
