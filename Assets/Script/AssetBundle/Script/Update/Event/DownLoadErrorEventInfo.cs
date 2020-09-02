/****************************************************
	文件：DownLoadErrorEventInfo.cs
	作者：Lonely
	github：https://github.com/AIValkyries/AssetBundleUpdate
	日期：2020/08/18 19:55:38
	功能：Nothing
*****************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DownLoadErrorEvent
{
    AssetDownInfo _down_info;

    public DownLoadErrorEvent(
        AssetDownInfo downInfo)
    {
        _down_info = downInfo;
    }

    public void Callback()
    {
        if (Event != null)
        {
            Event(_down_info);
        }
    }

    public System.Action<AssetDownInfo> Event;

}
