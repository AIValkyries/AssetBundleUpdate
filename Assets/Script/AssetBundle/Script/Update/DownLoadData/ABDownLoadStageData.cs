/****************************************************
	文件：ABDownLoadStageStrategy.cs
	作者：Lonely
	github：https://github.com/AIValkyries/AssetBundleUpdate
	日期：2020/08/26 19:23:46
	功能：Nothing
*****************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ABDownLoadStageData : StageDataBase
{
    public ABDownLoadStageData(Dictionary<string, Queue<AssetDataInfo>> downInfos) : base(downInfos){}

    public override List<AssetDownInfo> TryGetDownLoadData()
    {
        if (downloadQueue.Count <= 0)
            return null;

        List<AssetDownInfo> datas = new List<AssetDownInfo>();
        for (int i = 0; i < downloadQueue.Count; i++)
        {
            int number = currentQueue.Count;
            while (downloadQueue[i].Count > 0)
            {
                if (number >= DownloaderConstant.MAX_DOWNLOAD_QUEUE_COUNT)
                    break;

                AssetDownInfo info = downloadQueue[i].Dequeue();

                datas.Add(info);
                currentQueue.Add(info);
                number++;
            }
        }

        if (datas.Count > 0)
        {
            for (int i = 0; i < datas.Count; i++)
            {
                UpdateStageResult.AddDownLoadFileResultInfo(datas[i]);
            }
        }

        return datas;
    }

    public override eUpdateAssetType AssetType
    {
        get { return eUpdateAssetType.AssetBundle; }
    }

    public override IDownLoadParam GetDownLoadParam(AssetDownInfo info)
    {
        return new AssetBundleDownLoadParam(info.AssetName);
    }

    protected override bool IsDownloaded(AssetDataInfo serverInfo)
    {
        return FileManifestManager.LocalAB.IsDownloaded(serverInfo);
    }
}
