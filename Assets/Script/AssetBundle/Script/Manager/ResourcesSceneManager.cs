/****************************************************
	文件：SceneResourcesManager.cs
	作者：Lonely
	github：https://github.com/AIValkyries/AssetBundleUpdate
	日期：2020/08/05 19:25:31
	功能：Nothing
*****************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourcesSceneManager : ResManger
{
    // 移除后缀名
    string RemoveSuffix(string assetName)
    {
        int extIndex = assetName.LastIndexOf(".");
        if (extIndex != -1)
            assetName = assetName.Substring(0, extIndex);
        return assetName;
    }

     string GetSceneXmlFileName(string sceneName)
    {
        string fileName = PathConstant.PathName.DYNAMIC_SCENE_CONFIG_FILE_PATH + sceneName + ".xml";

        int indexOf = fileName.IndexOf("/") + 1;
        fileName = fileName.Substring(indexOf, fileName.Length - indexOf);

        return fileName;
    }

    string GetAssetName(string assetPath)
    {
        int lastIndexOf = assetPath.LastIndexOf("/") + 1;
        return assetPath.Substring(lastIndexOf, assetPath.Length - lastIndexOf);
    }

     SceneAsyncLoader BuilderSceneAsyncLoader(string scenePath)
    {
        string assetBundleName = RemoveSuffix(scenePath);
        string sceneName = GetAssetName(assetBundleName);
        string sceneXmlFileName = GetSceneXmlFileName(sceneName);

        SceneAsyncLoader loader = new SceneAsyncLoader(
            assetBundleName,
            sceneName,
            sceneXmlFileName);

        return loader;
    }

    public SceneAsyncLoader LoadSceneAsync(string scenePath)
    {
        _resource_load.UnloadLoadederAssetBundle(true); // 卸载已经加载了的资源

        SceneAsyncLoader loader = BuilderSceneAsyncLoader(scenePath);
        CoroutineManager.Coroutine(loader.StartLoadSceneAsync(_resource_load));

        return loader;
    }

    public SceneAsyncLoader UnloadScene(string scenePath)
    {
        // 找到场景文件，找到场景ab，找到场景ab依赖
        SceneAsyncLoader loader = BuilderSceneAsyncLoader(scenePath);
        loader.StartUnloadSceneAsync(_resource_load);

        return loader;
    }

    public override void Startup(object param = null)
    {

    }

    public override void Terminate(object param = null)
    {
         
    }
}
