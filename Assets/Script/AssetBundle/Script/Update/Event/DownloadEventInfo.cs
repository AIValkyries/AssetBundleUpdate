/****************************************************
	文件：DownloadEventInfo.cs
	作者：Lonely
	github：https://github.com/AIValkyries/AssetBundleUpdate 
	日期：2020/08/18 19:50:58
	功能：Nothing
*****************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DownloadSuccessfulEvent
{
    AssetDownInfo _down_info;
    HttpDownLoadAsync _load_async;

    public DownloadSuccessfulEvent(
        AssetDownInfo downInfo, HttpDownLoadAsync loadAsync)
    {
        _down_info = downInfo;
        _load_async = loadAsync;
    }

    public void Callback()
    {
        if (Event != null) 
        {
            Event(_down_info, _load_async);
        }
    }

    public System.Action<AssetDownInfo, HttpDownLoadAsync> Event;

}
