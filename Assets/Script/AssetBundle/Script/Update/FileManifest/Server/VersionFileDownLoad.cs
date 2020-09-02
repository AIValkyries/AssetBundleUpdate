using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VersionFileDownLoad : 
    FileManifestAssetDownLoadBase, IServerVersionFileManifest
{
    List<string> _all_versions;
    Dictionary<string, VersionInfo> _version_infos;
    IServerVersionListFileManifest _server_version_list;

    public VersionFileDownLoad()
    {
        FileManifestManager.Add<IServerVersionFileManifest>(this);
       _server_version_list = FileManifestManager.Get<IServerVersionListFileManifest>();
        _version_infos = new Dictionary<string, VersionInfo>();
    }
 
    protected override void OnDownLoadFileInfoSuccessful(AssetDownInfo downInfo)
    {
        string version = _all_versions[downInfo.Index];
        string context = System.Text.Encoding.Default.GetString(downInfo.Buffer.Bytes);
        VersionInfo info = JsonUtility.FromJson<VersionInfo>(context);
        _version_infos[version] = info;

        //Debug.Log("版本文件:" + context);
    }

    protected override void OnAllDownLoadComplete()
    {
        List<string> abVersions = new List<string>();
        var itr = _version_infos.Keys.GetEnumerator();
        while(itr.MoveNext())
        {
            VersionInfo info = _version_infos[itr.Current];
            if (info.Type == PackageType.CompressionPack.ToString())
                continue;

            abVersions.Add(info.Version);
        }
        itr.Dispose();

        // 留最后一个 ab 版本文件
        if (abVersions.Count > 0)
        {
            for (int i = 0; i < abVersions.Count - 1; i++)
            {
                _version_infos.Remove(abVersions[i]);
                _all_versions.Remove(abVersions[i]);
            }
        }

        base.OnAllDownLoadComplete();
    }

    public override void StartDownLoad()
    {
        List<string> versionList = _server_version_list.GetAllVersion();
        if (versionList == null)
            return;

        _all_versions = GetNeedDownVersion(versionList);
        for (int i = 0; i < _all_versions.Count; i++)
        {
            HttpDownLoadAsync httpDownLoadAsync = CreateHttpDownLoad();

            _version_infos.Add(_all_versions[i], null);

            httpDownLoadAsync.Setup(NewAssetDownInfo(i));
            httpDownLoadAsync.StartDownLoad(new VersionDownLoadParam(_all_versions[i]));
        }
    }

    //获取需要下载的版本
    List<string> GetNeedDownVersion(List<string> dataInfos)
    {
        int localVersionIndex = 0;
        int serverVersionIndex = 0;

        for (int i = 0; i < dataInfos.Count; i++) 
        {
            if (FileManifestManager.LocalVersion.Version == dataInfos[i])
                localVersionIndex = i + 1;
            if (_server_version_list.GetLastVersion() == dataInfos[i])
                serverVersionIndex = i + 1;
        }

        if (localVersionIndex == serverVersionIndex)
        {
            Debug.Log("本地版本:" + FileManifestManager.LocalVersion.Version);
            Debug.Log("Server版本:" + _server_version_list.GetLastVersion());
            Debug.Log("开始索引:" + localVersionIndex);
            Debug.Log("结束索引:" + serverVersionIndex);
            Debug.LogError("哈哈出错了!");
        }

        List<string> versionNumber = new List<string>();
        for (int i = localVersionIndex; i < serverVersionIndex; i++)
        {
            versionNumber.Add(dataInfos[i]);
        }

        return versionNumber;
    }

    public void WriteVersion(string version)
    {
        if (_version_infos.ContainsKey(version))
        {
            VersionInfo versionInfo = _version_infos[version];
            FileManifestManager.LocalVersion.Write(versionInfo);
        }
    }

    public VersionInfo GetVersionInfo(string version)
    {
        if (_version_infos.ContainsKey(version))
            return _version_infos[version];
        return null;
    }

    public List<string> GetABVersion()
    {
        List<string> versions = new List<string>();
        var itr = _version_infos.Keys.GetEnumerator();
        while (itr.MoveNext())
        {
            VersionInfo info = _version_infos[itr.Current];
            if (info.Type == PackageType.CompressionPack.ToString())
                continue;
            versions.Add(itr.Current);
        }
        itr.Dispose();

        return versions;
    }

    public List<string> GetZipAllVersion()
    {
        List<string> versions = new List<string>();

        var itr = _version_infos.Keys.GetEnumerator();
        while (itr.MoveNext())
        {
            VersionInfo info = _version_infos[itr.Current];
            if (info.Type == PackageType.OnlyABPack.ToString())
                continue;
            versions.Add(itr.Current);
        }
        itr.Dispose();

        return versions;
    }

    public List<string> GetAllVersion()
    {
        return _all_versions;
    }

    public VersionInfo GetLastVersin()
    {
        if (_all_versions.Count <= 0)
            return null;

        string version = _all_versions[_all_versions.Count - 1];
        return _version_infos[version];
    }
}
