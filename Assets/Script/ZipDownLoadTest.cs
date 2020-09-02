/****************************************************
	文件：ZipDownLoadTest.cs
	作者：Lonely
	github：https://github.com/AIValkyries/AssetBundleUpdate
	日期：2020/08/14 20:53:26
	功能：Nothing
*****************************************************/

using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public class ZipDownLoadTest : MonoBehaviour
{
    [System.NonSerialized]
    public string URL = "http://127.0.0.1:1500/AssetBundleServer/";


    private void Awake()
    {
        DontDestroyOnLoad(this);

        List<IManager> managers = new List<IManager>()
    {
        new ResourcesSceneManager(),
        new ResourcesManager(),
        new ResourcesUpdateManager()
    };
        Managers.Add(managers);
        StartCoroutine(CheckDownLoad());
    }

    //static MethodInfo clearMethod = null;
    ///// <summary>
    ///// 清空log信息
    ///// </summary>
    //private static void ClearConsole()
    //{
    //    if (clearMethod == null)
    //    {
    //        System.Type log = typeof(UnityEditor.EditorWindow).Assembly.GetType("UnityEditor.LogEntries");
    //        clearMethod = log.GetMethod("Clear");
    //    }
    //    clearMethod.Invoke(null, null);
    //}

    IEnumerator CheckDownLoad()
    {
        yield return 60;
        if (Managers.ResourcesUpdate.IsNeedUpdated())
        {
            Debug.Log("下载版本文件");
            // 下载版本文件
            Managers.ResourcesUpdate.StartDownLoad();
        }
    }

    private void Update()
    {
        Updates.UpdateALL();
    }

    private void OnGUI()
    {
        GUIS.OnGUIALL();     
    }

}
