using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ProtoBuf;
using System.IO;
using System.Net;

using System.Text;

public class Test : MonoBehaviour
{
    [System.NonSerialized]
    public string URL = "http://127.0.0.1:1500/AssetBundleServer/";

    ResourcesUpdateManager resUpdateManager;
    public bool StopDownLoad = false;

    private void Awake()
    {
        DontDestroyOnLoad(this);


        StartCoroutine(CheckDownLoad());
    }

    IEnumerator CheckDownLoad()
    {
        yield return 60;
        if (resUpdateManager.IsNeedUpdated())
        {
            Debug.Log("下载版本文件");
            // 下载版本文件
            //resUpdateManager.DownLoadFileManifest();
        }
    }

    private void Update()
    {
        Updates.UpdateALL();
        //resUpdateManager.UpdateDownLoad();
    }

}
