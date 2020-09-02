/****************************************************
	文件：AsyncLoaderQuene.cs
	作者：Lonely
	github：https://github.com/AIValkyries/AssetBundleUpdate
	日期：2020/08/02 22:22:02
	功能：Nothing
*****************************************************/

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public abstract class RequestBase
{
    AsyncOperation _async_operation;

    public bool IsDone { get { return _async_operation == null ? false : _async_operation.isDone; } }
    public float Progress { get { return _async_operation == null ? 0 : _async_operation.progress; } }

    public void Setup(AsyncOperation async)
    {
        _async_operation = async;
    }

}

// 所有资源加载请求
public class AssetAllLoaderRequest : RequestBase
{
    string _asset_bundle_name;

    public string AssetBundleName
    {
        get { return _asset_bundle_name; }
    }

    UnityEngine.Object[] _allAssets;
    public UnityEngine.Object[] allAssets { get { return _allAssets; } }

    public AssetAllLoaderRequest(string assetBundleName)
    {
        _asset_bundle_name = assetBundleName;
    }

    public static AssetAllLoaderRequest New(string assetBundleName)
    {
        return new AssetAllLoaderRequest(assetBundleName);
    }

    public Action<UnityEngine.Object[]> OnLoaderCompleted;

    public void Completed(UnityEngine.Object[] objects)
    {
        _allAssets = objects;
        if (OnLoaderCompleted != null)
        {
            OnLoaderCompleted(objects);
        }
    }

}

// 资源加载请求
public class AssetLoaderRequest : RequestBase
{
    string _asset_path;
    UnityEngine.Object _asset;

    public string AssetPath { get { return _asset_path; } }
    public UnityEngine.Object asset { get { return _asset; } }

    public AssetLoaderRequest(string assetPath) { _asset_path = assetPath; }

    public static AssetLoaderRequest New(string assetPath)
    {
        return new AssetLoaderRequest(assetPath);
    }

    public void Completed(UnityEngine.Object @object)
    {
        _asset = @object;
        if (OnLoaderCompleted != null) 
        {
            OnLoaderCompleted(@object);
        }
    }

    public Action<UnityEngine.Object> OnLoaderCompleted;
}

public class AssetBundleLoaderRequest : RequestBase
{
    string _asset_bundle_path;
    AssetBundleInfo _asset_bundle_info;

    public string AssetPath { get { return _asset_bundle_path; } }
    public AssetBundleInfo ABInfo { get { return _asset_bundle_info; } }

    public AssetBundleLoaderRequest(string assetBundlePath) { _asset_bundle_path = assetBundlePath; }

    public void Setup(AsyncOperation async, AssetBundleInfo info)
    {
        base.Setup(async);
        _asset_bundle_info = info;
    }

    public static AssetBundleLoaderRequest New(string assetBundlePath)
    {
        return new AssetBundleLoaderRequest(assetBundlePath);
    }

    public Action<AssetBundleInfo> OnCompleted;
}
