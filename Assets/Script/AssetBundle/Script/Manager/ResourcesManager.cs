/****************************************************
	文件：ResourcesManager.cs
	作者：Lonely
	github：https://github.com/AIValkyries/AssetBundleUpdate
	日期：2020/08/04 20:14:09
	功能：Nothing
*****************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class ResourcesManager : ResManger
{
    private const string _use_asset_bundle = "UseAssetBundleInEditor";
    static int _flag_num_use_asset_bundle = 1;

    public bool UseAssetBundle
    {
        get
        {
#if UNITY_EDITOR
            return _flag_num_use_asset_bundle == 0 ? false : true;
#elif UNITY_IOS
 return true;
#elif UNITY_ANDROID
 return true;
#elif UNITY_STANDALONE_WIN
 return true;
#endif
        }
    }

    public void LoadAssetAsyncByAssetBundleName<T>(AssetAllLoaderRequest request) where T : Object
    {
        if (UseAssetBundle)
            _resource_load.LoadAssetAsyncByName<T>(request);
    }

    public void LoadAssetAsyncByPath<T>(AssetLoaderRequest request) where T : Object
    {
        if (!UseAssetBundle)
        {
            string assetPath = string.Format("Assets/{0}", request.AssetPath);

            T t = LoadAssetAtPath<T>(assetPath);
            if (request.OnLoaderCompleted != null)
                request.OnLoaderCompleted(t);
        }
        else
        {
            _resource_load.LoadAssetAsync<T>(request);
        }
    }

    public T LoadAsset<T>(string assetPath) where T : Object
    {
        if (!UseAssetBundle)
        {
            assetPath = string.Format("Assets/{0}", assetPath);
            return LoadAssetAtPath<T>(assetPath);
        }
        else
        {
            return _resource_load.LoadAsset<T>(assetPath);
        }
    }

    // 某个文件夹的路径
    public Object[] LoadAllAssets(string assetPath)
    {
        if (!UseAssetBundle)
        {
            List<Object> objects = GetAllObjectByPath(assetPath);
            return objects.ToArray();
        }
        else
        {
            string assetBundleName = _resource_load.GetAssetBundleNameByAssetMaps(assetPath);
            if (assetBundleName != string.Empty)
            {
                return _resource_load.LoadAllAssets<Object>(assetBundleName);
            }

            List<Object> objects = GetAllObjectByPath(assetPath);
            return objects.ToArray();
        }
    }


    bool ExistDotMetaFile(string fileName)
    {
        string ext = Path.GetExtension(fileName);
        if (ext == ".meta")
            return true;
        return false;
    }

    List<Object> GetAllObjectByPath(string assetPath)
    {
        string dataPath = Application.dataPath;
        string fullPath = string.Format("{0}/{1}", dataPath, assetPath);
        if (!Directory.Exists(fullPath))
            return null;

        DirectoryInfo directory = new DirectoryInfo(fullPath);

        FileInfo[] files = directory.GetFiles();
        if (files.Length <= 0)
            return null;

        List<Object> objects = new List<Object>();
        for (int i = 0; i < files.Length; i++)
        {
            if (ExistDotMetaFile(files[i].FullName))
                continue;

            string childAssetPath = string.Empty;
            Object @object = null;

            if (!UseAssetBundle)
            {
                childAssetPath = string.Format("Assets/{0}/{1}", assetPath, files[i].Name);
                @object = LoadAssetAtPath<Object>(childAssetPath);
            }
            else
            {
                childAssetPath = string.Format("{0}/{1}", assetPath, files[i].Name);
                @object = _resource_load.LoadAsset<Object>(childAssetPath);
            }

            if (@object != null)
                objects.Add(@object);
        }

        return objects;
    }

    T LoadAssetAtPath<T>(string asset) where T : Object
    {
#if UNITY_EDITOR
        return UnityEditor.AssetDatabase.LoadAssetAtPath<T>(asset);
#elif UNITY_IOS
 return null;
#elif UNITY_ANDROID
  return null;
#elif UNITY_STANDALONE_WIN
 return null;
#endif

    }

    Object[] LoadAllAssetAtPath(string assetPath)
    {
#if UNITY_EDITOR
        return UnityEditor.AssetDatabase.LoadAllAssetsAtPath(assetPath);
#elif UNITY_IOS
 return null;
#elif UNITY_ANDROID
  return null;
#elif UNITY_STANDALONE_WIN
 return null;
#endif
    }


}
