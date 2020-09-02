using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FenbaoFileDownLoad : 
    FileManifestAssetDownLoadBase, IServerFenbaoFileManifest
{
    IServerVersionFileManifest _server_version;
    Dictionary<string, List<AssetDataInfo>> _all_zip;
    List<string> _all_version = new List<string>();


    public FenbaoFileDownLoad() : base()
    {
        FileManifestManager.Add<IServerFenbaoFileManifest>(this);
        _all_zip = new Dictionary<string, List<AssetDataInfo>>();
        _server_version = FileManifestManager.Get<IServerVersionFileManifest>();
    }

    public override void StartDownLoad()
    {
        _all_version = _server_version.GetZipAllVersion();
        if (_all_version.Count <= 0)
        {
            base.OnAllDownLoadComplete();
            return;
        }

        for (int i = 0; i < _all_version.Count; i++) 
        {
            HttpDownLoadAsync httpDownLoadAsync = CreateHttpDownLoad();

            _all_zip.Add(_all_version[i], new List<AssetDataInfo>());

            httpDownLoadAsync.Setup(NewAssetDownInfo(i));
            httpDownLoadAsync.StartDownLoad(new FenBaoDownLoadParam(_all_version[i]));
        }
    }

    protected override void OnDownLoadFileInfoSuccessful(AssetDownInfo downInfo)
    {
        string version = _all_version[downInfo.Index];
        string context = System.Text.Encoding.Default.GetString(downInfo.Buffer.Bytes);
        string[] fenBaoStrs = context.Split('\n');

        //Debug.Log("分包下载:" + version);

        for (int i = 0; i < fenBaoStrs.Length; i++)
        {
            if (fenBaoStrs[i] == string.Empty || fenBaoStrs[i] == " ")
                continue;
            string[] infoStr = fenBaoStrs[i].Split('|');

            AssetDataInfo info = new AssetDataInfo();
            info.Name = infoStr[0];
            info.MD5 = infoStr[1];
            info.Size = System.Convert.ToInt32(infoStr[2]);
            info.Version = version;

            _all_zip[version].Add(info);
        }
    }

    // 全部下载完成，组织首包和边玩边下载的包
    protected override void OnAllDownLoadComplete()
    {
        base.OnAllDownLoadComplete();
    }

    public Dictionary<string, List<AssetDataInfo>> GetAllPackage()
    {
        return _all_zip;
    }
 
}
