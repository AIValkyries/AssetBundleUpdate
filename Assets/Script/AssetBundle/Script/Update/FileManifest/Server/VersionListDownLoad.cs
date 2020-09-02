using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VersionListDownLoad : 
    FileManifestAssetDownLoadBase, IServerVersionListFileManifest
{
    List<string> _all_versions = new List<string>();

    public VersionListDownLoad()
    {
        FileManifestManager.Add<IServerVersionListFileManifest>(this);
    }

    protected override void OnDownLoadFileInfoSuccessful(AssetDownInfo downInfo)
    {
        string context = System.Text.Encoding.Default.GetString(downInfo.Buffer.Bytes);
        Debug.Log("版本列表:\n" + context);

        string[] versionList = context.Split('\n');
        for (int i = 0; i < versionList.Length; i++)
        {
            if (versionList[i] == string.Empty ||
                versionList[i] == " ") 
                continue;
            string versionName = versionList[i];
            _all_versions.Add(versionName.Trim());
        }
        isDone = true;
    }

    public override void StartDownLoad()
    {
        HttpDownLoadAsync httpDownloadAsync = CreateHttpDownLoad();
        httpDownloadAsync.Setup(new AssetDownInfo());
        VersionListDownLoadParam versionListDownLoad = new VersionListDownLoadParam();
        httpDownloadAsync.StartDownLoad(versionListDownLoad);
    }

    public List<string> GetAllVersion()
    {
        return _all_versions;
    }

    public string GetLastVersion()
    {
        if (_all_versions.Count <= 0)
            return string.Empty;
        return _all_versions[_all_versions.Count - 1];
    }
}
