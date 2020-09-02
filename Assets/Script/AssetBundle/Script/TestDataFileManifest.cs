/****************************************************
	文件：TestDataFileMan.cs
	作者：Lonely
	github：https://github.com/AIValkyries/AssetBundleUpdate
	日期：2020/08/25 21:08:26
	功能：Nothing
*****************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// {"Version":"1.1","OsType":"StandaloneWindows","CurrentTime":"8/24/2020 8:44:57 PM","Type":"CompressionPack"}
public class serverVersionFileManifestTest : IServerVersionFileManifest
{
    Dictionary<string, VersionInfo> versionInfos;

    public serverVersionFileManifestTest()
    {
        FileManifestManager.Add<IServerVersionFileManifest>(this);
        versionInfos = new Dictionary<string, VersionInfo>();

        System.Text.StringBuilder stringBuilder = new System.Text.StringBuilder();

        // OnlyABPack
        // CompressionPack
        string version1 = "1.1";
        string version2 = "1.2";
        string version3 = "2.3";
        string version4 = "3.4";
        string version5 = "4.5";
        string version6 = "4.6";
        string version7 = "5.7";
        string version8 = "6.8";

        versionInfos.Add(version1, BuildVersion(version1, PackageType.CompressionPack));
        versionInfos.Add(version2, BuildVersion(version2, PackageType.OnlyABPack));
        versionInfos.Add(version3, BuildVersion(version3, PackageType.CompressionPack));
        versionInfos.Add(version4, BuildVersion(version4, PackageType.OnlyABPack));
        versionInfos.Add(version5, BuildVersion(version5, PackageType.CompressionPack));
        versionInfos.Add(version6, BuildVersion(version6, PackageType.OnlyABPack));
        versionInfos.Add(version7, BuildVersion(version5, PackageType.CompressionPack));
        versionInfos.Add(version8, BuildVersion(version8, PackageType.CompressionPack));
    }

    VersionInfo BuildVersion(string version, PackageType type)
    {
        VersionInfo info = new VersionInfo();
        info.Version = version;
        info.Type = type.ToString();
        return info;
    }

    public List<string> GetABVersion()
    {
        List<string> abVersion = new List<string>();
        var itr = versionInfos.Keys.GetEnumerator();

        while (itr.MoveNext())
        {
            VersionInfo info = versionInfos[itr.Current];
            if (info.Type == "OnlyABPack")
            {
                abVersion.Add(itr.Current);
            }
        }
        itr.Dispose();

        return abVersion;
    }

    public List<string> GetAllVersion()
    {
        List<string> allVersions = new List<string>();
        var itr = versionInfos.Keys.GetEnumerator();

        while (itr.MoveNext())
        {
            allVersions.Add(itr.Current);
        }
        itr.Dispose();

        return allVersions;
    }

    public VersionInfo GetVersionInfo(string version)
    {
        if (versionInfos.ContainsKey(version))
            return versionInfos[version];
        return null; 
    }

    public List<string> GetZipAllVersion()
    {
        List<string> zipVersion = new List<string>();
        var itr = versionInfos.Keys.GetEnumerator();

        while (itr.MoveNext())
        {
            VersionInfo info = versionInfos[itr.Current];
            if (info.Type == "CompressionPack")
                zipVersion.Add(itr.Current);
        }
        itr.Dispose();

        return zipVersion;
    }

    public void WriteVersion(string version)
    {
        throw new System.NotImplementedException();
    }

    public VersionInfo GetLastVersin()
    {
        throw new System.NotImplementedException();
    }
}

public class fenBaoFileManifestTest : IServerFenbaoFileManifest
{
    Dictionary<string, List<AssetDataInfo>> _data_infos;

    public fenBaoFileManifestTest()
    {
        FileManifestManager.Add<IServerFenbaoFileManifest>(this);

        _data_infos = new Dictionary<string, List<AssetDataInfo>>();
        IServerVersionFileManifest versionFileManifest = FileManifestManager.Get<IServerVersionFileManifest>();

        List<string> zipVersions = versionFileManifest.GetZipAllVersion();

        int zipFileCount = 3;

        var itr = zipVersions.GetEnumerator();
        int versionIndex = 1;

        while (itr.MoveNext())
        {
            _data_infos.Add(itr.Current, new List<AssetDataInfo>());

            for (int i = 0; i < zipFileCount; i++) 
            {
                string name = string.Format(" Package{0}_{1}.zip", versionIndex, i);
                _data_infos[itr.Current].Add(BuildFenBao(name));
            }

            versionIndex++;
        }
        itr.Dispose();
    }

    AssetDataInfo BuildFenBao(string name)
    {
        AssetDataInfo info = new AssetDataInfo();
        info.Name = name;
        return info;
    }

    public void CompleteADownload(AssetDataInfo dataInfo)
    {
        throw new System.NotImplementedException();
    }

    public Dictionary<string, List<AssetDataInfo>> GetAllPackage()
    {
        return _data_infos;
    }

    public bool IsDownloadAll()
    {
        throw new System.NotImplementedException();
    }
}


public class AssetBundleFileManifestTest : IServerAssetBundleFileManifest
{
    protected Dictionary<string, List<AssetDataInfo>> _data_infos;

    public AssetBundleFileManifestTest()
    {
        FileManifestManager.Add<IServerAssetBundleFileManifest>(this);
        IServerVersionFileManifest versionFileManifest = FileManifestManager.Get<IServerVersionFileManifest>();

        _data_infos = new Dictionary<string, List<AssetDataInfo>>();
        List<string> abVersions = versionFileManifest.GetABVersion();
        var itr = abVersions.GetEnumerator();

        while (itr.MoveNext())
        {
            _data_infos.Add(itr.Current,new List<AssetDataInfo>());
            int count = 3;

            for (int i = 0; i < count; i++) 
            {
                _data_infos[itr.Current].Add(BuildAssetData(itr.Current + "__" + i.ToString()));
            }

        }
        itr.Dispose();

    }

    AssetDataInfo BuildAssetData(string name)
    {
        AssetDataInfo info = new AssetDataInfo();
        info.Name = name;
        return info;
    }

    public Dictionary<string, List<AssetDataInfo>> GetAllAssetBundle()
    {
        return _data_infos;
    }

    public bool HaveAssetBundle()
    {
        return _data_infos.Count > 0;
    }
}


