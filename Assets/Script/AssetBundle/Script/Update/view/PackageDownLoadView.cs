/****************************************************
	文件：PackageDownLoadView.cs
	作者：Lonely
	github：https://github.com/AIValkyries/AssetBundleUpdate
	日期：2020/07/29 20:57:52
	功能：Nothing
*****************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PackageDownLoadView : IGUI, IUpdate
{
    protected ResourcesDownLoad downLoadBase;
    UpdateStageResult DownLoadStageResult = null;
    UpdateStageResult CompressionStageResult = null;

    ResourcesUpdateManager resourcesUpdate;

    public PackageDownLoadView(ResourcesDownLoad downLoadBase)
    {
        Updates.Add(this);
        GUIS.Add(this);
        this.downLoadBase = downLoadBase;
        DownLoadStageResult = UpdateStageResult.DownLoad;
        CompressionStageResult = UpdateStageResult.Compression;
        resourcesUpdate = Managers.ResourcesUpdate;
    }

    public void OnGUI()
    {
        if (CompressionStageResult.IsEnable)
            ShowCompression();

        if (DownLoadStageResult.IsEnable)
            ShowDownLoadInfo();
    }

    void ShowCompression()
    {
        GUILayout.BeginHorizontal();
        GUILayout.TextField("正在解压!!!!!!!!");
        GUILayout.EndHorizontal();

        GUILayout.Space(10);

        GUILayout.BeginHorizontal();
        GUILayout.Label(string.Format("文件数量{0}进度", "解压"));
        GUILayout.HorizontalScrollbar(
             0, CompressionStageResult.CurrentCount,
             0, CompressionStageResult.FileCount,
             GUILayout.Width(500));
        GUILayout.TextField(string.Format("{0}/{1}",
            CompressionStageResult.CurrentCount,
            CompressionStageResult.FileCount));
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        GUILayout.Label(string.Format("Bytes{0}进度", "解压"));
        GUILayout.HorizontalScrollbar(
             0, CompressionStageResult.CurrentSize,
             0, CompressionStageResult.TotalSize,
             GUILayout.Width(500));
        GUILayout.TextField(string.Format("{0}/{1}", 
            CompressionStageResult.CurrentSize, 
            CompressionStageResult.TotalSize));
        GUILayout.EndHorizontal();
    }


    void ShowDownLoadErrorFile()
    {
        if (downLoadBase.DownLoadErrorFile == null || downLoadBase.DownLoadErrorFile.Count <= 0) 
            return;

        for (int i = 0; i < downLoadBase.DownLoadErrorFile.Count; i++)
        {
            GUILayout.BeginHorizontal();
            GUILayout.TextField(
                string.Format("服务器上的文件出现错误,FileName:{0} Count:{1}",
                downLoadBase.DownLoadErrorFile[i].AssetName, downLoadBase.DownLoadErrorFile[i].ErrorNumber));
            GUILayout.EndHorizontal();
        }
    }

    void ShowDownLoadFileInfo(string assetName)
    {
        GUILayout.BeginHorizontal();
        DownLoadFileResultInfo info = DownLoadStageResult.DownLoadFileInfos[assetName];

        GUILayout.Label(info.AssetName);
        GUILayout.HorizontalScrollbar(
             0, info.CurrentFileSize,
             0, info.TotalFileSize,
             GUILayout.Width(300));
        GUILayout.TextField(string.Format("{0}/{1}", info.CurrentFileSize, info.TotalFileSize));
        GUILayout.EndHorizontal();
    }

    void ShowDownLoadInfo()
    {
        GUILayout.BeginHorizontal();
        if (GUILayout.Button("停止下载"))
            AssetDownLoads.StopALLDownLoad();
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        GUILayout.TextField("正在下载!!!!!!!!");
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        GUILayout.TextField(string.Format("每秒[{0}]Bit", DownloaderConstant.DownLoadSpeed));
        GUILayout.EndHorizontal();

        GUILayout.Space(10);

        ShowDownLoadErrorFile();

        GUILayout.BeginHorizontal();
        GUILayout.Label(string.Format("文件数量{0}进度","下载"));
        GUILayout.HorizontalScrollbar(
             0, DownLoadStageResult.CurrentCount,
             0, DownLoadStageResult.FileCount,
             GUILayout.Width(500));
        GUILayout.TextField(string.Format("{0}/{1}", DownLoadStageResult.CurrentCount, DownLoadStageResult.FileCount));
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        GUILayout.Label(string.Format("Bytes{0}进度", "下载"));
        GUILayout.HorizontalScrollbar(
             0, DownLoadStageResult.CurrentSize,
             0, DownLoadStageResult.TotalSize,
             GUILayout.Width(500));
        GUILayout.TextField(string.Format("{0}/{1}", DownLoadStageResult.CurrentSize, DownLoadStageResult.TotalSize));
        GUILayout.EndHorizontal();

        if (DownLoadStageResult.DownLoadFileInfos == null)
            return;

        var itr = DownLoadStageResult.DownLoadFileInfos.Keys.GetEnumerator();
        while (itr.MoveNext())
        {
            if (DownLoadStageResult.DownLoadFileInfos[itr.Current].AssetName == string.Empty)
                continue;
            ShowDownLoadFileInfo(itr.Current);
        }
        itr.Dispose();
    }

    public void Update()
    {
  
    }

}
