/****************************************************
	文件：DownloadPlayingStrategyFactory.cs
	作者：Lonely
	github：https://github.com/AIValkyries/AssetBundleUpdate
	日期：2020/08/26 14:45:29
	功能：Nothing
*****************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DownloadPlayingStageDataFactory : StrategyFactoryBase
{
    public override StageDataBase[] BuildStrategys()
    {
        Dictionary<string, List<AssetDataInfo>> datas = GetAllDatas();

        List<string> allVersion = serverVersion.GetAllVersion();
        VersionInfo lastVersion = serverVersion.GetVersionInfo(allVersion[allVersion.Count - 1]);

        if (lastVersion.Type == PackageType.OnlyABPack.ToString())
            return null;

        List<StageDataBase> strategies = new List<StageDataBase>();

        List<AssetDataInfo> assetDatas = datas[lastVersion.Version];
        Dictionary<string, Queue<AssetDataInfo>> downLoadData = new Dictionary<string, Queue<AssetDataInfo>>();
        downLoadData.Add(lastVersion.Version, new Queue<AssetDataInfo>());

        Debug.Log("边玩边下载的包----------Version:" + lastVersion.Version);

        for (int i = 1; i < assetDatas.Count; i++) 
        {
            if (FileManifestManager.LocalFenBao.IsDownloaded(assetDatas[i]))
                continue;
            downLoadData[lastVersion.Version].Enqueue(assetDatas[i]);
        }
        strategies.Add(new ZipDownLoadStageData(downLoadData));
        return strategies.ToArray();
    }

}
