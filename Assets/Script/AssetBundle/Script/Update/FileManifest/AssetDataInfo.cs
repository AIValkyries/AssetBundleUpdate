using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum PackageType
{
    OnlyABPack,            // 零散的ab包，
    CompressionPack        // 压缩的包(分包了)
}

public class VersionInfo 
{
    // 版本号 格式:0.0
    // 第一个数字为分包版本号;第二个为资源版本号,只要更新就会递增
    public string Version;
    public string OsType;
    public string CurrentTime;
    // PackageType
    public string Type;

    //AssetBundleManifest
    public string[] AssetBundleManifestNames;
    public string[] AssetBundleManifestMD5s;
    public string[] AssetBundleManifestLengths;
}


public class AssetDataInfo
{
    public string Name;
    public string MD5;
    public int Size;
    public bool IsCompressed;
    public string Version;

    public override string ToString()
    {
        string trueStr = IsCompressed ? "True" : "False";
        return Name + "|" + MD5 + "|" + Size + "|" + trueStr;
    }

    public AssetDownInfo ToAssetDownInfo(int index)
    {
        AssetDownInfo info = new AssetDownInfo();

        info.AssetName = Name;
        info.MD5 = MD5;
        info.Version = Version;
        info.TotalSize = Size;
        info.BufferNumber = 0;
        info.ErrorNumber = 0;
        info.Index = index;

        return info;
    }
}

public class AssetBuffer
{
    public byte[] Bytes;
    public int ByteSize;

    public void Clear()
    {
        Bytes = null;
    }
}

// 资源下载信息  
public class AssetDownInfo
{
    public static AssetDownInfo Empty = new AssetDownInfo();

    public AssetDownInfo()
    {
        Buffer = new AssetBuffer();
        AssetName = string.Empty;
        MD5 = string.Empty;
        Version = string.Empty;
        TotalSize = 0;
    }

    public string AssetName;
    public string MD5;
    public string Version;
    public int TotalSize;

    public AssetBuffer Buffer;
    public AssetBuffer DownloadedBuffer;

    public int BufferNumber;          // 当前已经下载的大小

    public int Index;
    public int QueueIndex;
    public int ErrorNumber;

    public eDownErrorCode ErrorCode;  // 下载错误

    public int DownLoadSize
    {
        get { if (DownloadedBuffer == null) return 0; else return DownloadedBuffer.ByteSize; }
    }

    public void Reset()
    {
        Buffer.Clear();
        if (DownloadedBuffer != null)
        {
            DownloadedBuffer.Clear();
            DownloadedBuffer = null;
        }
        BufferNumber = 0;
    }

    public AssetDataInfo ToAssetDataInfo()
    {
        AssetDataInfo info = new AssetDataInfo();
        info.Name = AssetName;
        info.MD5 = MD5;
        info.Size = TotalSize;
        info.Version = Version;
        return info;
    }
  
}

