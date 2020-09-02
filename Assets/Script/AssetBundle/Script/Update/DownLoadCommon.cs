/****************************************************
	文件：DownLoadCommon.cs
	作者：Lonely
	github：https://github.com/AIValkyries/AssetBundleUpdate
	日期：2020/07/27 19:50:35
	功能：Nothing
*****************************************************/

using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;

public static class DownLoadCommon
{
    public static void AddHttpHeader(
        HttpWebRequest webRequest,
        eDownLoadType downLoadType, 
        string version)
    {
        int versionNumber = 0;
        if (version != string.Empty) 
        {
            string[] versionContext = version.Split('.');
            if (downLoadType == eDownLoadType.ZipFile)
            {
                versionNumber = System.Convert.ToInt32(versionContext[0]);
            }
            else if (downLoadType == eDownLoadType.FileManifest)
            {
                versionNumber = System.Convert.ToInt32(versionContext[1]);
            }
        }

        // DownLoadType
        webRequest.Headers.Add("DType", 
            string.Format("{0}={1}",
            DownloaderConstant.DOWNLOAD_TYPE,
            downLoadType));

        // DownLoadSpeed
        webRequest.Headers.Add("DLSpeed",
            string.Format("{0}={1}",
             DownloaderConstant.DOWNLOAD_SPEED,
              DownloaderConstant.DownLoadSpeed));

        // Version
        webRequest.Headers.Add("Version",
           string.Format("{0}={1}",
            "Version",
             versionNumber));

        // OsType
        ePlatformType osType = AssetsCommon.GetPlatform();
        webRequest.Headers.Add("PlatformType",
             string.Format("{0}={1}",
              "OsType",
               osType));
    }

}
