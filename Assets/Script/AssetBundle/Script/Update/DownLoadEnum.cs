/****************************************************
	文件：DownLoadEnum.cs
	作者：Lonely
	github：https://github.com/AIValkyries/AssetBundleUpdate
	日期：2020/08/07 20:08:00
	功能：Nothing
*****************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum eUpdateAssetType
{
    Zip,
    AssetBundle
}

public enum eDownloadMode
{
    Batch,             // 下载完一批才下载另一批           
    Continuous,        // 下载玩一个接着下载另一个
    End
}

public enum eDownLoadType
{
    FileManifest = 0,
    ABFile = 1,
    ZipFile = 2
}

public enum eDownErrorCode
{
    None,         // 无
    NoResponse,   // 服务器未响应
    DownloadError,// 下载出错
    TimeOut,      // 超时
    Abort,        // 强制关闭   
    DiskFull,     // 存储空间不足  
}

public enum eWriteFileMode
{
    Auto,   // 下载成功后自动写入本地文件
    Manual, // 下载后通过手动方式写入本地文件
}
