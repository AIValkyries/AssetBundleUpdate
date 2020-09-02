using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class AssetBundleFileDownLoad : 
    FileManifestAssetDownLoadBase, IServerAssetBundleFileManifest
{
    List<string> _all_version = new List<string>();
    IServerVersionFileManifest _server_version;

    /// <summary>
    /// Key:版本号
    /// Value:版本资源s
    /// 按版本先后排序
    /// </summary>
    protected Dictionary<string, List<AssetDataInfo>> _data_infos;
    public AssetBundleFileDownLoad()
    {
        FileManifestManager.Add<IServerAssetBundleFileManifest>(this);
        _server_version = FileManifestManager.Get<IServerVersionFileManifest>();
        _data_infos = new Dictionary<string, List<AssetDataInfo>>();
    }

    public override void StartDownLoad()
    {
        _all_version = _server_version.GetABVersion();
        if (_all_version.Count <= 0)
        {
            base.OnAllDownLoadComplete();
            return;
        }

        for (int i = 0; i < _all_version.Count; i++)
        {
            HttpDownLoadAsync httpDownLoadAsync = CreateHttpDownLoad();

            _data_infos.Add(_all_version[i], new List<AssetDataInfo>());

            httpDownLoadAsync.Setup(NewAssetDownInfo(i));
            httpDownLoadAsync.StartDownLoad(new AssetBundleFileDownLoadParam(_all_version[i]));
        }
    }

    protected override void OnDownLoadFileInfoSuccessful(AssetDownInfo downInfo)
    {
        string version = _all_version[downInfo.Index];
        string context = Encoding.Default.GetString(downInfo.Buffer.Bytes);

        //Debug.Log("AssetBundle下载:" + version);

        string[] abInfos = context.Split('\n');

        for (int i = 0; i < abInfos.Length; i++)
        {
            if (abInfos[i] == string.Empty || abInfos[i] == " ")
                continue;   // 写入数据的时候没写好

            string[] abInfo = abInfos[i].Split('|');
            AssetDataInfo info = new AssetDataInfo();

            info.Name = abInfo[0];
            info.MD5 = abInfo[1];
            info.Size = Convert.ToInt32(abInfo[2]);
            info.Version = version;

            _data_infos[version].Add(info);
        }
    }

    public Dictionary<string, List<AssetDataInfo>> GetAllAssetBundle()
    {
        return _data_infos;
    }

}
