using System;
using System.Collections.Generic;
using UnityEngine;

public class FenBaoFileManifest : FileManifestBase, ILocalFenBaoFileManifest
{
    public List<AssetDataInfo> GetAllZip()
    {
        List<AssetDataInfo> datas = new List<AssetDataInfo>();

        var itr = localInfos.Values.GetEnumerator();
        while (itr.MoveNext())
        {
            if (itr.Current.IsCompressed)
                continue;
            datas.Add(itr.Current);
        }
        itr.Dispose();

        return datas;
    }

    public bool IsCompressed(AssetDataInfo serverInfo)
    {
        if (localInfos.ContainsKey(serverInfo.Name))
        {
            return localInfos[serverInfo.Name].IsCompressed;
        }
        return false;
    }

    public override void WriteUpToDate()
    {
        IServerVersionFileManifest versionFileManifest = FileManifestManager.Get<IServerVersionFileManifest>();
        IServerFenbaoFileManifest serverFenbao = FileManifestManager.Get<IServerFenbaoFileManifest>();
        Dictionary<string, List<AssetDataInfo>> assetDatas = serverFenbao.GetAllPackage();

        List<string> allZipVersion = versionFileManifest.GetZipAllVersion();
        string lastVersion = allZipVersion[allZipVersion.Count - 1];

        if (assetDatas.ContainsKey(lastVersion))
        {
            Dictionary<string, AssetDataInfo> temp = new Dictionary<string, AssetDataInfo>();

            var itr = assetDatas[lastVersion].GetEnumerator();
            while (itr.MoveNext())
            {
                itr.Current.IsCompressed = true;
                temp.Add(itr.Current.Name, itr.Current);
            }
            itr.Dispose();

            WriteToLocal(temp, true);
        }
    }

    protected override string GetLocalAssetInfoFileName()
    {
        string abPath = AssetsCommon.LocalAssetPath;
        return string.Format("{0}/{1}", abPath, PathConstant.FileName.FEN_BAO);
    }
}