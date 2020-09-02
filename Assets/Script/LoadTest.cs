/****************************************************
	文件：LoadTest.cs
	作者：Lonely
	github：https://github.com/AIValkyries/AssetBundleUpdate
	日期：2020/08/05 21:28:20
	功能：Nothing
*****************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadTest : MonoBehaviour
{
    SceneAsyncLoader loadRequest = null;

    private void Awake()
    {
        DontDestroyOnLoad(this);

        List<IManager> managers = new List<IManager>()
    {
        new ResourcesSceneManager(),
        new ResourcesManager()
    };
        Managers.Add(managers);


        string assetName2 = "L_Resources/DataConfig/dataconfig_soundmissionfenbao.bytes";
        AssetLoaderRequest request = AssetLoaderRequest.New(assetName2);
        Managers.Resources.LoadAssetAsyncByPath<TextAsset>(request);

        request.OnLoaderCompleted = (asset) =>
        {
            Debug.Log("异步加载单个资源:" + asset.name);
        };

        string assetName1 = "L_Resources/Animation/Quad.controller";
        Object gameObject = Managers.Resources.LoadAsset<Object>(assetName1);
        Debug.Log("同步加载单个资源:" + gameObject.name);

        string assetPath1 = "L_Resources/DataConfig";
        Object[] objects1 = Managers.Resources.LoadAllAssets(assetPath1);
        Debug.Log("加载某个路径下的所有资源:" + objects1.Length);

        string assetPath2 = "L_Resources/DynamicSceneConfig";
        Object[] objects2 = Managers.Resources.LoadAllAssets(assetPath2);
        Debug.Log("加载某个路径下的所有资源:" + objects2.Length);

        string sceneName = "Scenes/SampleScene.unity";
        //StartCoroutine(unloadScene());
        loadRequest = Managers.SceneManager.LoadSceneAsync(sceneName);

        List<string> abAssetNames = new List<string>()
        {
            "dataconfig",
            "l_lua.protocolgenerated.accelerate"
        };

        //StartCoroutine(loadAssets(abAssetNames));
    }

    IEnumerator loadAssets(List<string> assetNames)
    {
        for (int i = 0; i < assetNames.Count; i++)
        {
            yield return new WaitForSeconds(0.5F);

            AssetAllLoaderRequest request = new AssetAllLoaderRequest(assetNames[i]);
            request.OnLoaderCompleted = onLoadAsset;
            Managers.Resources.LoadAssetAsyncByAssetBundleName<Object>(request);
        }
    }

    void onLoadAsset(Object[] @object)
    {
        // 加载到了一个对象
        if (@object == null || @object.Length <= 0)
            return;

        for (int i = 0; i < @object.Length; i++)
        {
            Object obj = @object[i];
            Debug.Log("OBJECT=" + obj.name);

        }
    }

    IEnumerator unloadScene()
    {
        Managers.SceneManager.UnloadScene("Scenes/SampleScene.unity");
        yield return new WaitForSeconds(0);
    }

}
