/****************************************************
	文件：CoroutineManager.cs
	作者：Lonely
	github：https://github.com/AIValkyries/AssetBundleUpdate
	日期：2020/07/12 21:38:47
	功能：Nothing
*****************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoroutineManager : MonoBehaviour
{
    static CoroutineManager _manager;

    static CoroutineManager GetManager()
    {
        if (_manager == null)
        {
            GameObject go = GameObject.Find("CoroutineManager");
            if (go == null)
            {
                go = new GameObject("CoroutineManager");
                _manager = go.AddComponent<CoroutineManager>();
            }
            else
            {
                _manager = go.GetComponent<CoroutineManager>();
            }

            go.transform.position = Vector3.zero;
            go.transform.localScale = Vector3.one;
            go.transform.localRotation = Quaternion.identity;

            DontDestroyOnLoad(go);
        }
        return _manager;
    }


    public static void Coroutine(string methodName)
    {
        GetManager().StartCoroutine(methodName);
    }

    public static void Coroutine(IEnumerator route)
    {
        GetManager().StartCoroutine(route);
    }
}
