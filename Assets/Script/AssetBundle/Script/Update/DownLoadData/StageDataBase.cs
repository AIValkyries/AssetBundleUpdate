/****************************************************
	文件：StrategyBase.cs
	作者：Lonely
	github：https://github.com/AIValkyries/AssetBundleUpdate
	日期：2020/08/25 15:38:07
	功能：Nothing
*****************************************************/

using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

 
public abstract class StageDataBase
{
    protected static Dictionary<string, List<AssetDownInfo>> sourceData;

    public static Dictionary<string, int> VersionRecord = new Dictionary<string, int>();

    string _local_path;
    Dictionary<string, Queue<AssetDataInfo>> _cache_server_data;

    protected List<Queue<AssetDownInfo>> downloadQueue;
    protected List<AssetDownInfo> currentQueue;

    public static bool CheckVersionIsDownLoadComplete(string version)
    {
        if (VersionRecord.ContainsKey(version))
        {
            VersionRecord[version]--;
            if (VersionRecord[version] <= 0)
                return true;
        }
        return false;
    }

    public StageDataBase(Dictionary<string, Queue<AssetDataInfo>> downInfos)
    {
        sourceData = new Dictionary<string, List<AssetDownInfo>>();
        _local_path = AssetsCommon.LocalAssetPath;
        _cache_server_data = downInfos;

        downloadQueue = new List<Queue<AssetDownInfo>>();
        currentQueue = new List<AssetDownInfo>();
    }

    protected virtual bool IsCompressed(AssetDataInfo info) { return false; }
    protected abstract bool IsDownloaded(AssetDataInfo info);
    public abstract IDownLoadParam GetDownLoadParam(AssetDownInfo info);

    #region InitializeData

    public void InitializeData()
    {
        if (_cache_server_data.Count <= 0)
            return;

        int i = StartIndex;
        int queueIndex = 0;
        Dictionary<string, List<AssetDataInfo>> packages = new Dictionary<string, List<AssetDataInfo>>();

        var itr = _cache_server_data.Keys.GetEnumerator();
        while (itr.MoveNext())
        {
            Queue<AssetDataInfo> datas = _cache_server_data[itr.Current];
            downloadQueue.Add(new Queue<AssetDownInfo>());

            while (datas.Count > 0)
            {
                AssetDataInfo serverInfo = datas.Dequeue();
                UpdateStageResult.DownLoad.TotalSize += serverInfo.Size;
                UpdateStageResult.DownLoad.FileCount++;

                if (IsDownloaded(serverInfo) ||
                    IsCompressed(serverInfo))
                {
                    UpdateStageResult.DownLoad.CurrentSize += serverInfo.Size;
                    UpdateStageResult.DownLoad.CurrentCount++;
                    continue;
                }

                AssetDownInfo download = serverInfo.ToAssetDownInfo(i);
                download.QueueIndex = queueIndex;
                download.Version = itr.Current;

                if (!VersionRecord.ContainsKey(itr.Current))
                    VersionRecord.Add(itr.Current, 0);
                VersionRecord[itr.Current]++;

                InitDownLoadInfo(download);
                OnAddAssets(itr.Current, download);

                downloadQueue[downloadQueue.Count - 1].Enqueue(download);
                i++;
            }
            queueIndex++;
        }
        itr.Dispose();

        _cache_server_data.Clear();
    }

    protected void OnAddAssets(string version, AssetDownInfo info)
    {
        if (!sourceData.ContainsKey(version))
            sourceData.Add(version, new List<AssetDownInfo>());
        sourceData[version].Add(info);
    }

    void InitDownLoadInfo(AssetDownInfo info)
    {
        string localFileName = _local_path + "/" + info.AssetName;
        if (File.Exists(localFileName))
        {
            try
            {
                using (FileStream fs = File.Open(localFileName, FileMode.Open))
                {
                    if (fs.Length >= info.TotalSize)  // 需要删除
                    {
                        fs.Close();
                        File.Delete(localFileName);
                    }
                    else
                    {
                        info.DownloadedBuffer = new AssetBuffer();
                        info.DownloadedBuffer.ByteSize = (int)fs.Length;
                        info.DownloadedBuffer.Bytes = new byte[fs.Length];
                        fs.Read(info.DownloadedBuffer.Bytes, 0, (int)fs.Length);
                        fs.Close();
                    }
                }
            }
            catch (System.Exception ex)
            {
                Debug.LogError(ex.Message);
            }
        }
        info.ErrorCode = eDownErrorCode.None;
        info.Buffer.ByteSize = info.TotalSize;

        if (info.DownloadedBuffer != null)
            UpdateStageResult.DownLoad.CurrentSize += info.DownloadedBuffer.ByteSize;
    }

    #endregion

    public virtual void TryRemove(AssetDownInfo info)
    {
        UpdateStageResult.RemoveDownLoadFileResultInfo(info);

        if (sourceData.ContainsKey(info.Version))
        {
            if (sourceData[info.Version].Contains(info))
                sourceData[info.Version].Remove(info);
        }

        if (currentQueue.Count > 0)
        {
            currentQueue.Remove(info);
            UpdateStageResult.DownLoad.CurrentCount++;
        }
    }

    public abstract List<AssetDownInfo> TryGetDownLoadData();

    public virtual bool IsDone
    {
        get
        {
            for (int i = 0; i < downloadQueue.Count; i++)
            {
                if (downloadQueue[i].Count > 0)
                    return false;
            }
            return true;
        }
    }

    public bool IsDownLoadAllVersion
    {
        get
        {
            bool downLoadALl = true;
            var itr = sourceData.Keys.GetEnumerator();
            while (itr.MoveNext())
            {
                if (sourceData[itr.Current].Count > 0)
                {
                    downLoadALl = false;
                    break;
                }
            }
            itr.Dispose();

            return downLoadALl;
        }
    }

    public abstract eUpdateAssetType AssetType { get; }
    protected virtual int StartIndex { get { return 0; } set { } }


    #region DownLoadFileResultInfo
 
    // 如果下载错误则回退
    public void GoBackDownLoadFileResultInfo(AssetDownInfo info)
    {
        long currentSize = UpdateStageResult.DownLoad.DownLoadFileInfos[info.AssetName].CurrentFileSize;
        UpdateStageResult.DownLoad.DownLoadFileInfos[info.AssetName].CurrentFileSize = 0;
        UpdateStageResult.DownLoad.CurrentSize -= currentSize;
        UpdateStageResult.DownLoad.CurrentCount--;
        info.Reset();
    }

    public void UpdateDownloadInfo(AssetDownInfo info, long increment)
    {
        UpdateStageResult.UpdateDownLoadFileResultInfo(info, increment);
    }

    #endregion

}
