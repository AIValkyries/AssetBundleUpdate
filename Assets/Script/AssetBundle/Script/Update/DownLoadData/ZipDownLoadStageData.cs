/****************************************************
	文件：ZipOrABDownLoadStageStrategy.cs
	作者：Lonely
	github：https://github.com/AIValkyries/AssetBundleUpdate 
	日期：2020/08/25 15:40:19
	功能：Nothing
*****************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ZipDownLoadStageData : StageDataBase
{
    ZipDecompressStage _zip_decompress;

    public ZipDownLoadStageData(
         Dictionary<string, Queue<AssetDataInfo>> downInfos) : base(downInfos)
    {
        _zip_decompress = new ZipDecompressStage(this);
    }

    public override void TryRemove(AssetDownInfo info)
    {
        if (currentQueue.Count > 0)
            _zip_decompress.Enqueue(info);
        base.TryRemove(info);
    }

    public override List<AssetDownInfo> TryGetDownLoadData()
    {
        if (currentQueue.Count <= 0)
        {
            int index = 0;
            while (downloadQueue.Count > 0 &&
                currentQueue.Count < DownloaderConstant.MAX_DOWNLOAD_QUEUE_COUNT)
            {
                Queue<AssetDownInfo> downloadInfos = downloadQueue[index];

                while (downloadInfos.Count > 0 &&
                    currentQueue.Count < DownloaderConstant.MAX_DOWNLOAD_QUEUE_COUNT)
                {
                    currentQueue.Add(downloadInfos.Dequeue());
                }

                if (downloadInfos.Count <= 0)
                    downloadQueue.Remove(downloadQueue[index]);
                else
                    index++;
            }

            for (int i = 0; i < currentQueue.Count; i++)
            {
                UpdateStageResult.AddDownLoadFileResultInfo(currentQueue[i]);
            }

            return currentQueue;
        }

        return null;
    }

    protected override bool IsCompressed(AssetDataInfo info)
    {
        return FileManifestManager.LocalFenBao.IsCompressed(info);
    }

    public override IDownLoadParam GetDownLoadParam(AssetDownInfo info)
    {
        return new ZipDownLoadParam(info.Version, info.AssetName);
    }

    protected override bool IsDownloaded(AssetDataInfo serverInfo)
    {
        return FileManifestManager.LocalFenBao.IsDownloaded(serverInfo);
    }

    public override bool IsDone
    {
        get
        {
            for (int i = 0; i < downloadQueue.Count; i++)
            {
                if (downloadQueue[i].Count > 0)
                    return false;
            }

            if (!_zip_decompress.IsDone)
                _zip_decompress.StartDecompress();
            return true;
        }
    }

    public override eUpdateAssetType AssetType
    {
        get { return eUpdateAssetType.Zip; }
    }

    protected override int StartIndex
    {
        get { return _zip_decompress.Index; }
        set { }
    }


}
