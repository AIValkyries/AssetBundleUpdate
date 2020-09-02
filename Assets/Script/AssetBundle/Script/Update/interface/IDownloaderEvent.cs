/****************************************************
	文件：IDownloader.cs
	作者：Lonely
	github：https://github.com/AIValkyries/AssetBundleUpdate
	日期：2020/08/07 20:29:51
	功能：Nothing
*****************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDownloaderEvent
{
    eWriteFileMode EWriteFileMode { get; }
    void DownloadProgress(AssetDownInfo info,long increment);
    void DownLoadError(AssetDownInfo info, HttpDownLoadAsync loadAsync);
    void DownloadSuccessful(AssetDownInfo info, HttpDownLoadAsync loadAsync);
}
