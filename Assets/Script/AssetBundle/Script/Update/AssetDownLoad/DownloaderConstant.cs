/****************************************************
	文件：DownloaderConstant.cs
	作者：Lonely
	github：https://github.com/AIValkyries/AssetBundleUpdate
	日期：2020/08/24 22:18:54
	功能：Nothing
*****************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class DownloaderConstant
{
    // 最大下载错误次数
    public const int MAX_DOWNLOAD_ERROR_NUMBER = 3;
    public const int MAX_DOWNLOAD_QUEUE_COUNT = 1;  // 下载队列长度
    public const string DOWNLOAD_TYPE = "DownLoadType";
    // 每次传输流的大小
    public const string STREAM_FILENGTH = "StreamLength";
    public const string DOWNLOAD_SPEED = "DownLoadSpeed";
    // 524288   20480
    public const int DownLoadSpeed = 524288;  //每秒多少 bit
}

