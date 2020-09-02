using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class FileManifestAssetDownLoadBase : AssetDownLoadBase
{
    // 下载链
    protected FileManifestAssetDownLoadBase downloadLink;
    protected List<int> versionIndex;

    public FileManifestAssetDownLoadBase()
    {
        versionIndex = new List<int>();
    }

    public void SetDownLoadlink(FileManifestAssetDownLoadBase downLoadBase)
    {
        downloadLink = downLoadBase;
    }

    protected override void OnDownloadSuccessful(AssetDownInfo info, HttpDownLoadAsync loadAsync)
    {
        lock (LOCK_OBJ)
        {
            if (info.Buffer.Bytes.Length != info.BufferNumber)
            {
                byte[] bytes = new byte[info.BufferNumber];
                for (int i = 0; i < info.BufferNumber; i++)
                {
                    bytes[i] = info.Buffer.Bytes[i];
                }

                info.Buffer.Bytes = bytes;
            }

            OnDownLoadFileInfoSuccessful(info);

            versionIndex.Remove(info.Index);
            if (httpDownloadAsyncs.Contains(loadAsync))
                httpDownloadAsyncs.Remove(loadAsync);

            if (IsDownloadABatch())
            {
                OnAllDownLoadComplete();
                ClearHttpDownLoad();
                isDone = true;
            }
        }
    }

    protected override void OnDownLoadError(AssetDownInfo downInfo)
    {
        Debug.Log("下载错误:" + downInfo.AssetName + " ErrorCode:" + downInfo.ErrorCode);
    }

    protected override void OnAllDownLoadComplete()
    {
        if (DownLoadCompleteEvent != null)
            DownLoadCompleteEvent(this);

        // 是否全部下载完毕
        if (versionIndex.Count <= 0)
        {
            if (downloadLink != null)
                downloadLink.StartDownLoad();
        }
    }

    protected AssetDownInfo NewAssetDownInfo(int i)
    {
        AssetDownInfo downInfo = new AssetDownInfo();
        downInfo.Index = i;
        versionIndex.Add(i);
        return downInfo;
    }

    protected abstract void OnDownLoadFileInfoSuccessful(AssetDownInfo downInfo);

    // 下载完一批
    protected bool IsDownloadABatch()
    {
        return versionIndex.Count == 0;
    }


    public override eWriteFileMode EWriteFileMode
    {
        get { return eWriteFileMode.Manual; }
    }
}
