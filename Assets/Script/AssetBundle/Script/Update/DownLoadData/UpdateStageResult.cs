/****************************************************
	文件：UpdateStageResult.cs
	作者：Lonely
	github：https://github.com/AIValkyries/AssetBundleUpdate
	日期：2020/08/14 20:22:47
	功能：Nothing
*****************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpdateStageResult
{
    // 下载结果记录
    public static UpdateStageResult DownLoad = new UpdateStageResult();
    // 压缩结果
    public static UpdateStageResult Compression = new UpdateStageResult();

    public bool IsEnable = false;
    public int FileCount = 0;    // 解压缩或者下载的文件数量
    public int CurrentCount = 0; // 当前 Count
    public float Progress { get { return (float)CurrentSize / (float)TotalSize; } }
    public long TotalSize = 0;
    public long CurrentSize = 0;

    // 当前下载的文件信息
    public Dictionary<string, DownLoadFileResultInfo> DownLoadFileInfos;

    public static void UpdateDownLoadFileResultInfo(AssetDownInfo downInfo, long increment)
    {
        if (downInfo == AssetDownInfo.Empty)
            return;
        DownLoad.CurrentSize += increment;
        if (DownLoad.DownLoadFileInfos.ContainsKey(downInfo.AssetName))
            DownLoad.DownLoadFileInfos[downInfo.AssetName].CurrentFileSize += increment;
    }

    public static void AddDownLoadFileResultInfo(AssetDownInfo downInfo)
    {
        if (downInfo == AssetDownInfo.Empty)
            return;
        if (DownLoad.DownLoadFileInfos == null)
            DownLoad.DownLoadFileInfos = new Dictionary<string, DownLoadFileResultInfo>();

        if (!DownLoad.DownLoadFileInfos.ContainsKey(downInfo.AssetName))
        {
            DownLoadFileResultInfo resultInfo = new DownLoadFileResultInfo();
            resultInfo.TotalFileSize = downInfo.Buffer.ByteSize;
            if (downInfo.DownloadedBuffer != null)
                resultInfo.CurrentFileSize = downInfo.DownloadedBuffer.ByteSize;
            resultInfo.AssetName = downInfo.AssetName;
            DownLoad.DownLoadFileInfos.Add(downInfo.AssetName, resultInfo);
        }
    }

    public static void RemoveDownLoadFileResultInfo(AssetDownInfo downInfo)
    {
        if (downInfo == AssetDownInfo.Empty)
            return;
        if (DownLoad.DownLoadFileInfos == null)
            DownLoad.DownLoadFileInfos = new Dictionary<string, DownLoadFileResultInfo>();
        if (DownLoad.DownLoadFileInfos.ContainsKey(downInfo.AssetName))
            DownLoad.DownLoadFileInfos.Remove(downInfo.AssetName);
    }

    public void ClearAll()
    {
        IsEnable = false;
        ClearDownLoadInfo();
        ClearFilInfo();
    }

    public void ClearDownLoadInfo()
    {
        FileCount = 0;
        CurrentCount = 0;
        TotalSize = 0;
        CurrentSize = 0;
    }

    public void ClearFilInfo()
    {
        if (DownLoadFileInfos == null)
            return;

        var itr = DownLoadFileInfos.Keys.GetEnumerator();
        while(itr.MoveNext())
        {
            DownLoadFileInfos[itr.Current].Clear();
        }
        itr.Dispose();
    }

}


public class DownLoadFileResultInfo
{
    public DownLoadFileResultInfo()
    {
        AssetName = string.Empty;
    }

    public string AssetName;
    public long TotalFileSize = 0;
    public long CurrentFileSize = 0;
    public float FileProgress { get { return (float)CurrentFileSize / (float)TotalFileSize; } }

    public void Clear()
    {
        AssetName = string.Empty;
        TotalFileSize = 0;
        CurrentFileSize = 0;
    }
}
