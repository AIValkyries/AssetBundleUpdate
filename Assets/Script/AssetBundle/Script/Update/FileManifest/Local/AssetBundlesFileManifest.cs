/****************************************************
	文件：ABAssetInfoManager.cs
	作者：Lonely
	github：https://github.com/AIValkyries/AssetBundleUpdate
	日期：2020/07/19 19:42:18
	功能：Nothing
*****************************************************/

using System.Collections.Generic;
using UnityEngine;

public class AssetBundlesFileManifest : 
    FileManifestBase, ILocalABFileManifest
{
    public override void WriteUpToDate()
    {
        IServerVersionFileManifest versionFileManifest = FileManifestManager.Get<IServerVersionFileManifest>();
        List<string> allABVersion = versionFileManifest.GetABVersion();
        if (allABVersion.Count <= 0)
            return;

        IServerAssetBundleFileManifest serverFenbao = FileManifestManager.Get<IServerAssetBundleFileManifest>();
        Dictionary<string, List<AssetDataInfo>> assetDatas = serverFenbao.GetAllAssetBundle();

        string lastVersion = allABVersion[allABVersion.Count - 1];

        if (assetDatas.ContainsKey(lastVersion))
        {
            Dictionary<string, AssetDataInfo> temp = new Dictionary<string, AssetDataInfo>();

            var itr = assetDatas[lastVersion].GetEnumerator();
            while (itr.MoveNext())
            {
                temp.Add(itr.Current.Name, itr.Current);
            }
            itr.Dispose();

            WriteToLocal(temp, true);
        }
    }

    protected override string GetLocalAssetInfoFileName()
    {
        string abPath = AssetsCommon.LocalAssetPath;
        return string.Format("{0}/{1}", abPath, PathConstant.FileName.AB_ASSET_INFO);
    }

}