/****************************************************
	文件：FileManifestManager.cs
	作者：Lonely
	github：https://github.com/AIValkyries/AssetBundleUpdate
	日期：2020/08/27 14:24:31
	功能：Nothing
*****************************************************/

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class FileManifestManager
{
    static Dictionary<Type, IFileManifest> datas = new Dictionary<Type, IFileManifest>();

    static FileManifestManager()
    {
        VersionFileManifest versionFileManifest = new VersionFileManifest();
        versionFileManifest.Read();

        Add<ILocalVersionFileManifest>(versionFileManifest);

        FenBaoFileManifest fenBaoFileManifest = new FenBaoFileManifest();
        fenBaoFileManifest.Read();

        Add<ILocalFenBaoFileManifest>(fenBaoFileManifest);

        AssetBundlesFileManifest bundlesFileManifest = new AssetBundlesFileManifest();
        bundlesFileManifest.Read();

        Add<ILocalABFileManifest>(bundlesFileManifest);
    }

    public static void RemoveLocalFenBaoData(AssetDataInfo dataInfo)
    {
        if (LocalFenBao != null) 
            LocalFenBao.Remove(dataInfo);
    }

    public static void RemoveLocalABData(AssetDataInfo dataInfo)
    {
        if (LocalAB != null)
            LocalAB.Remove(dataInfo);
    }

    public static void UpdateLocalFenBaoData(AssetDataInfo newInfo)
    {
        if (LocalFenBao != null)
            LocalFenBao.Update(newInfo);
    }

    public static void UpdateLocalABData(AssetDataInfo newInfo)
    {
        if (LocalAB != null)
            LocalAB.Update(newInfo);
    }


    public static void WriteABDataByServer()
    {
        if (LocalAB != null)
        {
            LocalAB.WriteUpToDate();
        }
    }
    public static void WriteFenBaoDataByServer()
    {
        if (LocalFenBao != null)
        {
            LocalFenBao.WriteUpToDate();
        }
    }

    public static void WriteABDataByCurrent()
    {
        if (LocalAB != null)
        {
            LocalAB.WriteCurrent();
        }
    }

    public static void WriteFenBaoDataByCurrent()
    {
        if (LocalFenBao != null)
        {
            LocalFenBao.WriteCurrent();
        }
    }

    public static void Add<T>(IFileManifest serverFileManifest)
    {
        if (!datas.ContainsKey(typeof(T)))
        {
            datas.Add(typeof(T), serverFileManifest);
        }
        else
        {
            datas[typeof(T)] = serverFileManifest;
        }
    }

    public static T Get<T>() where T : IFileManifest
    {
        if (datas.ContainsKey(typeof(T)))
        {
            return (T)datas[typeof(T)];
        }
        return default(T);
    }

    public static void Clear()
    {
        datas.Clear();
    }

    public static ILocalVersionFileManifest LocalVersion
    {
        get
        {
            return Get<ILocalVersionFileManifest>();
        }
    }

    public static ILocalFenBaoFileManifest LocalFenBao
    {
        get
        {
            return Get<ILocalFenBaoFileManifest>();
        }
    }

    public static ILocalABFileManifest LocalAB
    {
        get
        {
            return Get<ILocalABFileManifest>();
        }
    }

}
