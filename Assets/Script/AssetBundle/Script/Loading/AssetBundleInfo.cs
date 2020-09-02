/****************************************************
	文件：AssetBundleInfo.cs
	作者：Lonely
	github：https://github.com/AIValkyries/AssetBundleUpdate
	日期：2020/07/29 22:21:36
	功能：Nothing
*****************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class AssetBundleInfo
{
    int _reference_count;
    bool _is_loader;
    string _asset_name;
    AssetBundle _bundle;

    public bool IsLoader
    {
        get { return _is_loader; }
    }

    public AssetBundleInfo(string abName)
    {
        _asset_name = abName;
    }

    public string GetAssetBundleName()
    {
        return _asset_name;
    }

    public void AddReferenceCount()
    {
        _reference_count++;
    }

    public int GetReferenceCount()
    {
        return _reference_count;
    }

    public void ReduceReferenceCount()
    {
        _reference_count--;
    }

    // 是否可以卸载
    public bool IsUninstall()
    {
        return _reference_count == 0;
    }

    public void AsynLoadFromFile(
        AssetBundleLoaderRequest async,
        System.Action<AssetBundleInfo> loaderDependency = null)
    {
        if (_is_loader)
            return;

        string fileName = BuildFileName();
        AssetBundleCreateRequest request = AssetBundle.LoadFromFileAsync(fileName);

        async.Setup(request, this);
        request.completed +=(rt)=> 
        {
            _bundle = request.assetBundle;
            if (loaderDependency != null)
                loaderDependency(this);
            if (async.OnCompleted != null) 
                async.OnCompleted(this);
        };
        _is_loader = true;
    }

    public void AsyncLoadAsset<T>(string fileName, AssetLoaderRequest asset) where T : Object
    {
        if (_bundle == null)
            return;

        fileName = string.Format("{0}/{1}", "Assets", fileName);
        AssetBundleRequest request = _bundle.LoadAssetAsync<T>(fileName);
        asset.Setup(request);

        request.completed += (rt) =>
        {
            asset.Completed(request.asset);
        };
    }

    public void AsyncLoadAllAssets<T>(AssetAllLoaderRequest asset) where T : Object
    {
        if (_bundle == null)
            return;
        AssetBundleRequest request = _bundle.LoadAllAssetsAsync<T>();

        request.completed += (rt) =>
        {
            asset.Completed(request.allAssets);
        };
    }

    public void LoadFromFile()
    {
        if (_is_loader)
            return;
        string fileName = BuildFileName();
        _bundle = AssetBundle.LoadFromFile(fileName);
        _is_loader = true;
    }

    public T LoadAsset<T>(string fileName) where T : Object
    {
        if (_bundle == null)
            return default(T);

        fileName = string.Format("{0}/{1}", "Assets", fileName);
        return _bundle.LoadAsset<T>(fileName);
    }

    public T[] LoadAllAssets<T>() where T : Object
    {
        if (_bundle == null)
            return null;
        return _bundle.LoadAllAssets<T>();
    }

    public void Uninstall(bool unloadAllLoadedObjects)
    {
        if (_bundle == null)
            return;
        _is_loader = false;
        _bundle.Unload(unloadAllLoadedObjects);
    }

    string BuildFileName()
    {
        string assetPath = AssetsCommon.LocalAssetPath;
        return string.Format("{0}/{1}", assetPath, _asset_name);
    }

}
