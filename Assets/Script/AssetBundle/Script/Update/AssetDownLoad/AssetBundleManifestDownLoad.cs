/****************************************************
	文件：AssetBundleManifestDownLoad.cs
	作者：Lonely
	github：https://github.com/AIValkyries/AssetBundleUpdate
	日期：2020/09/02 19:54:07
	功能：Nothing
*****************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AssetBundleManifestDownLoad : AssetDownLoadBase
{
    List<int> _version_index;
    public AssetBundleManifestDownLoad()
    {
        _version_index = new List<int>();
    }

    public override eWriteFileMode EWriteFileMode
    {
        get { return eWriteFileMode.Auto; }
    }

    public override void StartDownLoad()
    {
        VersionInfo info = FileManifestManager.Get<IServerVersionFileManifest>().GetLastVersin();
        if (info == null)
            return;
        if (info.AssetBundleManifestNames == null)
            return;

        for (int i = 0; i < info.AssetBundleManifestNames.Length; i++) 
        {
            AssetBundleDownLoadParam param = new AssetBundleDownLoadParam(info.AssetBundleManifestNames[i]);
            HttpDownLoadAsync httpDownLoadAsync = CreateHttpDownLoad();
         
            AssetDownInfo downInfo = new AssetDownInfo();
            downInfo.AssetName = info.AssetBundleManifestNames[i];
            downInfo.Buffer.ByteSize = int.Parse(info.AssetBundleManifestLengths[i]);
            downInfo.MD5 = info.AssetBundleManifestMD5s[i];
            downInfo.Index = i;
            _version_index.Add(i);

            httpDownLoadAsync.Setup(downInfo);
            httpDownLoadAsync.StartDownLoad(param);
        }
    }

    protected override void OnDownloadSuccessful(AssetDownInfo info, HttpDownLoadAsync loadAsync)
    {
        _version_index.Remove(info.Index);
        if (_version_index.Count <= 0)
        {
            OnAllDownLoadComplete();
            if (DownLoadCompleteEvent != null)
                DownLoadCompleteEvent(this);
        }
    }

    protected override void OnAllDownLoadComplete()
    {
        ResManger.SetupAssetBundle();
    }

}
