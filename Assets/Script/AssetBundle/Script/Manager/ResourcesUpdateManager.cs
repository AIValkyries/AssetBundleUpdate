/****************************************************
	文件：ResourcesUpdateManager.cs
	作者：Lonely
	github：https://github.com/AIValkyries/AssetBundleUpdate
	日期：2020/07/12 20:59:07
	功能：Nothing
*****************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class ResourcesUpdateManager : IManager, IUpdate
{
    // 一般由服务器传过来,自定义一下当前最高版本
    string _server_max_version = "3.4";

    FenbaoFileDownLoad _fen_bao_downLoad = null;
    AssetBundleFileDownLoad _file_downLoad = null;
    bool _have_download_plying;
    PackageDownLoadView _first_view;

    // 是否需要更新
    public bool IsNeedUpdated()
    {
        if (FileManifestManager.LocalVersion.Version != _server_max_version)
            return true;

        return false;
    }

    public void StartDownLoad()
    {
        VersionListDownLoad versionListDownLoad = new VersionListDownLoad();
        VersionFileDownLoad versionFileDownLoad = new VersionFileDownLoad();
        _fen_bao_downLoad = new FenbaoFileDownLoad();
        _file_downLoad = new AssetBundleFileDownLoad();

        versionListDownLoad.SetDownLoadlink(versionFileDownLoad);
        versionFileDownLoad.SetDownLoadlink(_fen_bao_downLoad);
        _fen_bao_downLoad.SetDownLoadlink(_file_downLoad);
 
        versionListDownLoad.StartDownLoad();
        StartZipDownLoad();
    }

    void StartZipDownLoad()
    {
        _file_downLoad.DownLoadCompleteEvent = (dl) =>
        {
            AssetBundleManifestDownLoad manifestDownLoad = new AssetBundleManifestDownLoad();
            manifestDownLoad.StartDownLoad();

            manifestDownLoad.DownLoadCompleteEvent = (rt) =>
            {
                FirstPackageStageDataFactory zipDownloadStragegy = new FirstPackageStageDataFactory();
                StageDataBase[] strategyBases = zipDownloadStragegy.BuildStrategys();

                if (strategyBases != null && strategyBases.Length > 0)   //含有边玩边下载
                {
                    Debug.Log("边玩边下载首包=" + strategyBases.Length);

                    ResourcesDownLoad firstDownload = new ResourcesDownLoad(strategyBases);
                    firstDownload.DownLoadCompleteEvent = FristPackage;
                    firstDownload.StartDownLoad();
                    _first_view = new PackageDownLoadView(firstDownload);
                }
                else
                {
                    FristPackage(null);
                }
            };

        };
    }

    void DownloadPlaying()
    {
        DownloadPlayingStageDataFactory strategyFactory = new DownloadPlayingStageDataFactory();
        StageDataBase[] strategyBases = strategyFactory.BuildStrategys();

        if (strategyBases == null || strategyBases.Length <= 0)
            return;

        Debug.Log("边玩边下载包=" + strategyBases.Length);

        ResourcesDownLoad zipDownload = new ResourcesDownLoad(strategyBases);
        zipDownload.StartDownLoad();
        if (_first_view != null)
        {
            GUIS.Remove(_first_view);
            _first_view = null;
        }
        new PackageDownLoadView(zipDownload);

        zipDownload.DownLoadCompleteEvent = (rt) =>
        {
            Updates.Clear();
            GUIS.Clear();
            AssetDownLoads.Clear();
        };
    }

    public void FristPackage(AssetDownLoadBase loadBase)
    {
        string sceneName = "Scenes/SampleScene.unity";
        // 进入第二个场景
        SceneAsyncLoader sceneAsync = Managers.SceneManager.LoadSceneAsync(sceneName);
        sceneAsync.loadCompleted = (sa) =>
        {
            Debug.Log("场景加载完毕!开边玩边下载");
            // 边玩边下载
            DownloadPlaying();
        };
    }

    public void Startup(object param = null)
    {
     
    }

    public void Terminate(object param = null)
    {
       
    }

    public void Update()
    {
        if (AssetDownLoads.IsAllDownLoadComplete)
        {
            Debug.Log("全部下载完成!!");
            GUIS.Clear();
            AssetDownLoads.Clear();
            Updates.Clear();
        }
    }

}
