/****************************************************
	文件：IFactory.cs
	作者：Lonely
	github：https://github.com/AIValkyries/AssetBundleUpdate
	日期：2020/08/25 19:26:55
	功能：Nothing
*****************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IndexData
{
    public int NextIndex;
    public int EndIndex;
}

public interface IFactory
{
    StageDataBase[] BuildStrategys();
    StageDataBase BuildZipDownloadStrategy(Dictionary<string, List<AssetDataInfo>> datas);
    StageDataBase BuildABDownloadStrategy(Dictionary<string, List<AssetDataInfo>> datas);

}

public abstract class StrategyFactoryBase : IFactory
{
    protected IServerVersionFileManifest serverVersion;
    protected IServerAssetBundleFileManifest serverAssetBundle;
    protected IServerFenbaoFileManifest serverFenbao;

    public StrategyFactoryBase()
    {
        serverVersion = FileManifestManager.Get<IServerVersionFileManifest>();
        serverAssetBundle = FileManifestManager.Get<IServerAssetBundleFileManifest>();
        serverFenbao = FileManifestManager.Get<IServerFenbaoFileManifest>();
    }

    protected Dictionary<string, List<AssetDataInfo>> GetAllDatas()
    {
        Dictionary<string, List<AssetDataInfo>> datas = new Dictionary<string, List<AssetDataInfo>>();

        Dictionary<string, List<AssetDataInfo>> fenBaoAllPackage = serverFenbao.GetAllPackage();
        Dictionary<string, List<AssetDataInfo>> abAll = serverAssetBundle.GetAllAssetBundle();

        var itr = fenBaoAllPackage.Keys.GetEnumerator();
        while (itr.MoveNext())
        {
            if (!datas.ContainsKey(itr.Current))
                datas.Add(itr.Current, new List<AssetDataInfo>());
            datas[itr.Current].AddRange(fenBaoAllPackage[itr.Current]);
        }
        itr.Dispose();

        var abItr = abAll.Keys.GetEnumerator();
        while (abItr.MoveNext())
        {
            if (datas.ContainsKey(abItr.Current))
            {
                Debug.LogError("分包版本和散包版本相同??????" + abItr.Current);
                break;
            }
            datas.Add(abItr.Current, new List<AssetDataInfo>());
            datas[abItr.Current].AddRange(abAll[abItr.Current]);
        }
        abItr.Dispose();

        return datas;
    }


    public virtual StageDataBase[] BuildStrategys()
    {
        return null;
    }

    public virtual StageDataBase BuildZipDownloadStrategy(Dictionary<string, List<AssetDataInfo>> datas)
    {
        return null;
    }

    public virtual StageDataBase BuildABDownloadStrategy(Dictionary<string, List<AssetDataInfo>> datas)
    {
        return null;
    }
}
