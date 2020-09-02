/****************************************************
	文件：DownLoadStageFactory.cs
	作者：Lonely
	github：https://github.com/AIValkyries/AssetBundleUpdate
	日期：2020/08/25 19:27:30
	功能：Nothing
*****************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class FirstPackageStageDataFactory : StrategyFactoryBase
{
    IndexData _index_data;

    public override StageDataBase[] BuildStrategys()
    {
        Dictionary<string, List<AssetDataInfo>> datas = GetAllDatas();

        List<StageDataBase> strategies = new List<StageDataBase>();

        _index_data = new IndexData();

        int quitCount = 50;

        while ((_index_data.NextIndex < (datas.Count)) && quitCount > 0)  
        {
            StageDataBase zipStrategy = BuildZipDownloadStrategy(datas);
            StageDataBase abStrategy  = BuildABDownloadStrategy(datas);

            if (zipStrategy != null)
                strategies.Add(zipStrategy);
            if (abStrategy != null)
                strategies.Add(abStrategy);

            quitCount--;
        }

        return strategies.ToArray();
    }


    public override StageDataBase BuildZipDownloadStrategy(
         Dictionary<string, List<AssetDataInfo>> datas)
    {
        bool hasABPackage = false;

        List<string> allVersions = serverVersion.GetAllVersion();

        if (_index_data.NextIndex == allVersions.Count)
            return null;

        //Debug.Log("--------------------------START----------------------------");
        //Debug.Log("ZIP,TotalCount:" + allVersions.Count + " NextIndex:" + _index_data.NextIndex);

        _index_data.EndIndex = -1;

        for (int i = _index_data.NextIndex; i < allVersions.Count; i++)
        {
            VersionInfo info = serverVersion.GetVersionInfo(allVersions[i]);
            if (info == null)
            {
                Debug.LogError("没有找到版本信息文件:VERSION=" + allVersions[i]);
                break;
            }
            if (info.Type == PackageType.OnlyABPack.ToString())
            {
                hasABPackage = true;
                break;
            }

            _index_data.EndIndex = i;
        }

        Dictionary<string, Queue<AssetDataInfo>> downLoadData = new Dictionary<string, Queue<AssetDataInfo>>();

        for (int i = _index_data.NextIndex; i <= _index_data.EndIndex; i++)
        {
            string version = allVersions[i];

            downLoadData.Add(version,new Queue<AssetDataInfo>());

            List<AssetDataInfo> assetDatas = datas[version];
            int dataCount = assetDatas.Count;

            if (i == _index_data.EndIndex) 
            {
                if (!hasABPackage)
                    dataCount = 1;
            }

            //Debug.Log("Version:" + version + " DataCount:" + dataCount);
            Queue<AssetDataInfo> tempDatas = new Queue<AssetDataInfo>();

            for (int j = 0; j < dataCount; j++)
            {
                downLoadData[version].Enqueue(assetDatas[j]);
            }
        }

        _index_data.NextIndex = _index_data.EndIndex + 1;

        //Debug.Log("-------------------------------END----------------------");

        if (downLoadData.Count <= 0)
            return null;

        return new ZipDownLoadStageData(downLoadData);
    }

    public override StageDataBase BuildABDownloadStrategy(
        Dictionary<string, List<AssetDataInfo>> datas)
    {
        List<string> allVersions = serverVersion.GetAllVersion();
        if (_index_data.NextIndex == allVersions.Count)
            return null;

        //Debug.Log("AssetBundle,TotalCount:" + allVersions.Count + " NextIndex:" + _index_data.NextIndex);

        List<string> abVersions = new List<string>();

        int i = _index_data.NextIndex;

        for (; i < allVersions.Count; i++)
        {
            VersionInfo info = serverVersion.GetVersionInfo(allVersions[i]);
            if (info.Type != PackageType.OnlyABPack.ToString())
                break;
            abVersions.Add(allVersions[i]);
            _index_data.NextIndex = i + 1;
        }

        Dictionary<string, Queue<AssetDataInfo>> downLoadData = new Dictionary<string, Queue<AssetDataInfo>>();

        i = 0;
        for (; i < abVersions.Count; i++)
        {
            string version = abVersions[i];
            downLoadData.Add(version, new Queue<AssetDataInfo>());

            List<AssetDataInfo> assetDatas = datas[version];
       
            for (int j = 0; j < assetDatas.Count; j++)
            {
                downLoadData[version].Enqueue(assetDatas[j]);
            }
        }

        //Debug.Log("----------------------------------");
        return new ABDownLoadStageData(downLoadData);
    }

 
}
