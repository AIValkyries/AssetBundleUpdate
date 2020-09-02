/****************************************************
	文件：IAssetFileManifestFileData.cs
	作者：Lonely
	github：https://github.com/AIValkyries/AssetBundleUpdate
	日期：2020/08/18 22:23:08
	功能：Nothing
*****************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public interface IFileManifest
{

}

public interface IServerVersionListFileManifest : IFileManifest
{
    List<string> GetAllVersion();
    string GetLastVersion();
}

public interface IServerVersionFileManifest : IFileManifest
{
    void WriteVersion(string version);
    VersionInfo GetLastVersin();
    List<string> GetAllVersion();
    List<string> GetABVersion();
    List<string> GetZipAllVersion();
    VersionInfo GetVersionInfo(string version);
}

public interface IServerFenbaoFileManifest : IFileManifest
{
    Dictionary<string, List<AssetDataInfo>> GetAllPackage();
}

public interface IServerAssetBundleFileManifest : IFileManifest
{
    Dictionary<string, List<AssetDataInfo>> GetAllAssetBundle();
}


public interface ILocalVersionFileManifest : IFileManifest
{
    void Read();
    void Write(VersionInfo info);
    string Version { get; }
}

public interface ILocalFenBaoORABFileManifest:IFileManifest
{
    void Update(AssetDataInfo newInfo);
    void Remove(AssetDataInfo oldInfo);
    void Read();
    void WriteUpToDate();   // 写入最新版本
    void WriteCurrent();
    bool IsDownloaded(AssetDataInfo newInfo);
}

public interface ILocalFenBaoFileManifest : ILocalFenBaoORABFileManifest
{
    bool IsCompressed(AssetDataInfo serverInfo);
    List<AssetDataInfo> GetAllZip();
}

public interface ILocalABFileManifest : ILocalFenBaoORABFileManifest
{
  
}
