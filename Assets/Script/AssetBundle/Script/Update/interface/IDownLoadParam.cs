/****************************************************
	文件：IDownLoadParam.cs
	作者：Lonely
	github：https://github.com/AIValkyries/AssetBundleUpdate
	日期：2020/08/07 22:24:07
	功能：Nothing
*****************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDownLoadParam
{
    string GetVersion();
    string GetURL();
    eDownLoadType GetDownLoadType();	
}

public abstract class FileManifestDownLoadParam : IDownLoadParam
{
    protected string version;

    public FileManifestDownLoadParam(string ver)
    {
        version = ver;
    }

    public abstract string GetURL();

    public eDownLoadType GetDownLoadType()
    {
        return eDownLoadType.FileManifest;
    }

    public string GetVersion()
    {
        return version;
    }
}

public class VersionListDownLoadParam : IDownLoadParam
{
    public eDownLoadType GetDownLoadType()
    {
        return eDownLoadType.FileManifest;
    }

    public string GetURL()
    {
        return AssetBundleServerPath.FileManifestURL.GetVersionListURL(AssetsCommon.GetPlatform());
    }
    public string GetVersion()
    {
        return string.Empty;
    }
}

public class VersionDownLoadParam : FileManifestDownLoadParam
{
    public VersionDownLoadParam(string version) : base(version) { }

    public override string GetURL()
    {
        ABPathInfo pathInfo = new ABPathInfo(AssetsCommon.GetPlatform(), version);
        return AssetBundleServerPath.FileManifestURL.GetVersionURL(pathInfo);
    }
}

public class FenBaoDownLoadParam : FileManifestDownLoadParam
{
    public FenBaoDownLoadParam(string version) : base(version) { }
    public override string GetURL()
    {
        ABPathInfo pathInfo = new ABPathInfo(AssetsCommon.GetPlatform(), version);
        return AssetBundleServerPath.FileManifestURL.GetFenBaoURL(pathInfo);
    }
}

public class AssetBundleFileDownLoadParam : FileManifestDownLoadParam
{
    public AssetBundleFileDownLoadParam(string version) : base(version) { }
    public override string GetURL()
    {
        ABPathInfo pathInfo = new ABPathInfo(AssetsCommon.GetPlatform(), version);
        return AssetBundleServerPath.FileManifestURL.GetABAssetInfoURL(pathInfo);
    }
}


public class ZipDownLoadParam : IDownLoadParam
{
    protected string version;
    protected string assetName;

    public ZipDownLoadParam(string ver, string an) 
    {
        version = ver;
        assetName = an;
    }

    public eDownLoadType GetDownLoadType()
    {
        return eDownLoadType.ZipFile;
    }

    public string GetURL()
    {
        ABPathInfo pathInfo = new ABPathInfo(AssetsCommon.GetPlatform(), version);
        return AssetBundleServerPath.FenBaoURL.GetPackageFileName(pathInfo, assetName);
    }

    public string GetVersion()
    {
        return version;
    }

}

public class AssetBundleDownLoadParam : IDownLoadParam
{
    protected string assetName;

    public AssetBundleDownLoadParam(string an)
    {
        assetName = an;
    }

    public eDownLoadType GetDownLoadType()
    {
        return eDownLoadType.ABFile;
    }

    public string GetURL()
    {
        return AssetBundleServerPath.ABCacheURL.GetAssetBundleFileName(AssetsCommon.GetPlatform(), assetName);
    }

    public string GetVersion()
    {
        return string.Empty;
    }

}