/****************************************************
	文件：AssetBundleLoad.cs
	作者：Lonely
	github：https://github.com/AIValkyries/AssetBundleUpdate 
	日期：2020/07/30 19:30:02
	功能：Nothing
*****************************************************/

using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;


public class AssetBundleLoad
{
    AssetBundle _main;
    AssetBundleManifest _manifest;
    AssetDependency _dependency;
    Dictionary<string, AssetBundleInfo> _asset_bundles;

    // 当前已经加载的资源
    List<AssetBundleInfo> _is_current_loader;

    public List<AssetBundleInfo> CurrentLoader
    {
        get { return _is_current_loader; }
    }

    public void Setup()
    {
        _is_current_loader = new List<AssetBundleInfo>();
        _asset_bundles = new Dictionary<string, AssetBundleInfo>();
        _dependency = new AssetDependency();

        string assetPath = AssetsCommon.LocalAssetPath + "/" + AssetsCommon.GetPlatform();
        if (!File.Exists(assetPath))
            return;

        _main = AssetBundle.LoadFromFile(assetPath);
        _manifest = _main.LoadAsset<AssetBundleManifest>("AssetBundleManifest");

        string[] assetBundles = _manifest.GetAllAssetBundles();

        for (int i = 0; i < assetBundles.Length; i++)
        {
            string assetBundleName = assetBundles[i].ToLower();

            _asset_bundles.Add(assetBundleName, new AssetBundleInfo(assetBundleName));
            string[] dependencies = _manifest.GetAllDependencies(assetBundleName);
            _dependency.Add(assetBundleName, dependencies);
        }

        _main.Unload(false);
    }

    public int GetDependencyCount(string assetBundleName)
    {
        List<string> dependencys = _dependency.GetDependency(assetBundleName);
        if (dependencys != null)
            return dependencys.Count;
        return 0;
    }

    public List<string> GetDependencys(string assetBundleName)
    {
        return _dependency.GetDependency(assetBundleName);
    }

    // assetPath 为相对路径，
    // 例如:L_Resources/Animation/Directional Light
    public AssetBundleInfo LoadAssetBundle(string assetPath)
    {
        string assetBundleName = GetAssetBundleNameByAssetMaps(assetPath);

        AssetBundleInfo abInfo = null;
        if (_asset_bundles.TryGetValue(assetBundleName, out abInfo))
        {
            if (abInfo.IsLoader)
                return abInfo;
            InternalLoadAssetBundle(abInfo);
        }

        return abInfo;
    }

    public AssetBundleInfo LoadSingleAssetBundle(string assetPath)
    {
        string assetBundleName = GetAssetBundleNameByAssetMaps(assetPath);

        AssetBundleInfo abInfo = null;
        if (_asset_bundles.TryGetValue(assetBundleName, out abInfo))
        {
            if (abInfo.IsLoader)
                return abInfo;

            abInfo.LoadFromFile();
            _is_current_loader.Add(abInfo);
        }

        return abInfo;
    }

    public T LoadAsset<T>(string assetPath) where T : Object
    {
        string assetBundlePath = RemoveSuffix(assetPath);
        AssetBundleInfo info = LoadAssetBundle(assetBundlePath);
        if (info == null)
        {
            Debug.LogError("该资源没有找到AssetBundle:" + assetPath);
            return default(T);
        }

        return info.LoadAsset<T>(assetPath);
    }

    public T[] LoadAllAssets<T>(string assetBundleName) where T : Object
    {
        AssetBundleInfo abInfo = null;
        if (_asset_bundles.TryGetValue(assetBundleName, out abInfo))
        {
            if (abInfo.IsLoader)
            {
                Debug.Log("LoadAllAssets 已经加载过了哦!!!!!!!!!!!!");
                return abInfo.LoadAllAssets<T>();
            }
            else
            {
                InternalLoadAssetBundle(abInfo);
                return abInfo.LoadAllAssets<T>();
            }
        }
        else
        {
            return null;
        }
    }

    public void LoadAssetBundleAsync(AssetBundleLoaderRequest request)
    {
        string assetBundleName = GetAssetBundleNameByAssetMaps(request.AssetPath);

        AssetBundleInfo abInfo = null;
        if (_asset_bundles.TryGetValue(assetBundleName, out abInfo))
        {
            if (abInfo.IsLoader)
                return;
            InternalLoadAssetBundleAsync(abInfo, request);
        }
    }

    public void LoadSingleAssetBundleAsync(AssetBundleLoaderRequest request)
    {
        string assetBundleName = GetAssetBundleNameByAssetMaps(request.AssetPath);

        AssetBundleInfo abInfo = null;
        if (_asset_bundles.TryGetValue(assetBundleName, out abInfo))
        {
            if (abInfo.IsLoader)
                return;

            abInfo.AsynLoadFromFile(request);
        }
    }

    public void LoadAssetAsyncByName<T>(AssetAllLoaderRequest request) where T : Object
    {
        AssetBundleInfo abInfo = null;
        if (_asset_bundles.TryGetValue(request.AssetBundleName, out abInfo))
        {
            if (abInfo.IsLoader)
            {
                abInfo.AsyncLoadAllAssets<T>(request);
            }
            else
            {
                AssetBundleLoaderRequest abRequest = 
                    AssetBundleLoaderRequest.New(request.AssetBundleName);
                abRequest.OnCompleted += (rt) =>
                {
                    abInfo.AsyncLoadAllAssets<T>(request);
                };
                InternalLoadAssetBundleAsync(abInfo, abRequest);
            }
        }
    }

    public void LoadAssetAsync<T>(AssetLoaderRequest request) where T : Object
    {
        string assetBundlePath = RemoveSuffix(request.AssetPath);
        string assetBundleName = GetAssetBundleNameByAssetMaps(assetBundlePath);

        AssetBundleInfo abInfo = null;

        if (_asset_bundles.TryGetValue(assetBundleName, out abInfo))
        {
            if (abInfo.IsLoader)
            {
                abInfo.AsyncLoadAsset<T>(request.AssetPath, request);
            }
            else
            {
                AssetBundleLoaderRequest abRequest = AssetBundleLoaderRequest.New(assetBundlePath);
                abRequest.OnCompleted += (rt) =>
                {
                    rt.AsyncLoadAsset<T>(request.AssetPath, request);
                };

                InternalLoadAssetBundleAsync(abInfo, abRequest);
            }
        }
    }

    public void LoadAllAssetsAsync<T>(AssetAllLoaderRequest request) where T : Object
    {
        AssetBundleInfo abInfo = null;
        if (_asset_bundles.TryGetValue(request.AssetBundleName, out abInfo))
        {
            if (abInfo.IsLoader)
            {
                abInfo.AsyncLoadAllAssets<T>(request);
                return;
            }
            else
            {
                AssetBundleLoaderRequest abRequest = AssetBundleLoaderRequest.New(request.AssetBundleName);
                abRequest.OnCompleted += (rt) =>
                {
                    rt.AsyncLoadAllAssets<T>(request);
                };

                InternalLoadAssetBundleAsync(abInfo, abRequest);
            }
        }
    }

    public string GetAssetBundleNameByAssetPath(string assetPath)
    {
        string assetBundlePath = RemoveSuffix(assetPath);
        return GetAssetBundleNameByAssetMaps(assetBundlePath);
    }

    public string GetAssetBundleNameByAssetMaps(string assetFileName)
    {
        string assetBundleName = assetFileName.Replace("/", ".").ToLower();

        if (!_asset_bundles.ContainsKey(assetBundleName)) // 有些ab中含有多个资源
        {
            // 移除文件名称，提炼文件夹路径
            string abNameByAssetPath = AssetMaps.GetAssetBundleNameByAssetPath(assetBundleName);
            if (abNameByAssetPath != null) 
                return abNameByAssetPath;

            int indexOf = assetBundleName.LastIndexOf(".");
            string abPath = assetBundleName.Substring(0, indexOf);

            assetBundleName = AssetMaps.GetAssetBundleNameByAssetPath(abPath);

            if (assetBundleName == string.Empty)
                return string.Empty;
        }

        return assetBundleName;
    }

    public void UninstallAssetBundle(string assetBundleName, bool unloadAllLoadedObjects = false)
    {
        AssetBundleInfo outInfo = null;
        if (_asset_bundles.TryGetValue(assetBundleName, out outInfo)) 
        {
            if (outInfo.IsUninstall())
            {
                outInfo.Uninstall(unloadAllLoadedObjects);
                _is_current_loader.Remove(outInfo);
                // 找打他的应用资源
                ReducingDependencie(assetBundleName);
            }
        }
    }

    public void UnloadLoadederAssetBundle(bool unloadAllLoadedObjects)
    {
        for (int i = 0; i < _is_current_loader.Count; i++) 
        {
            UninstallAssetBundle(_is_current_loader[i].GetAssetBundleName(), unloadAllLoadedObjects);
        }
        _is_current_loader.Clear();
    }

    void ReducingDependencie(string assetBundleName)
    {
        List<string> dependencys = _dependency[assetBundleName];
        if (dependencys == null || dependencys.Count <= 0)
            return;

        for (int i = 0; i < dependencys.Count; i++) 
        {
            AssetBundleInfo outInfo = null;
            if (_asset_bundles.TryGetValue(dependencys[i], out outInfo))
            {
                outInfo.ReduceReferenceCount();

                if (outInfo.IsUninstall())
                    UninstallAssetBundle(dependencys[i], true);
            }
        }
    }

    string RemoveSuffix(string assetName)
    {
        int extIndex = assetName.LastIndexOf(".");
        if (extIndex != -1)
            assetName = assetName.Substring(0, extIndex);
        return assetName;
    }

    void InternalLoadAssetBundle(AssetBundleInfo abInfo)
    {
        abInfo.LoadFromFile();
        _is_current_loader.Add(abInfo);

        List<string> dependencys = _dependency.GetDependency(abInfo.GetAssetBundleName());
        for (int i = 0; i < dependencys.Count; i++)
        {
            AssetBundleInfo info = LoadAssetBundle(dependencys[i]);
            info.AddReferenceCount();
        }
    }

    void InternalLoadAssetBundleAsync(
        AssetBundleInfo abInfo, AssetBundleLoaderRequest request)
    {
        abInfo.AsynLoadFromFile(request, (mainAB) =>
        {
            _is_current_loader.Add(abInfo);

            List<string> dependencys = _dependency.GetDependency(mainAB.GetAssetBundleName());
            for (int i = 0; i < dependencys.Count; i++)
            {
                AssetBundleLoaderRequest deRequest = AssetBundleLoaderRequest.New(dependencys[i]);
                LoadAssetBundleAsync(deRequest);

                deRequest.OnCompleted = (ab) =>
                {
                    ab.AddReferenceCount();
                };
            }
        });
    }


}
