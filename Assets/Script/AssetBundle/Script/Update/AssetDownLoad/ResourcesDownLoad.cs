using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class ResourcesDownLoad : AssetDownLoadBase
{
    List<AssetDownInfo> _downLoad_error;
    public List<AssetDownInfo> DownLoadErrorFile
    {
        get { return _downLoad_error; }
    }

    int _stage_index;
    StageDataBase[] _strategy_bases;
    StageDataBase _current_strategy
    {
        get { return _strategy_bases[_stage_index]; }
    }

    public ResourcesDownLoad(StageDataBase[] strategyBases)
    {
        _stage_index = 0;
        _downLoad_error = new List<AssetDownInfo>();
        _strategy_bases = strategyBases;
    }

    #region 下载事件

    protected override void OnDownloadProgress(AssetDownInfo info, long increment)
    {
        _current_strategy.UpdateDownloadInfo(info, increment);
    }

    protected override void OnDownLoadError(AssetDownInfo info)
    {
        Debug.Log("ZIP下载错误！");
        // 回炉重造
        _current_strategy.GoBackDownLoadFileResultInfo(info);

        // 下载错误
        if (info.ErrorNumber < DownloaderConstant.MAX_DOWNLOAD_ERROR_NUMBER)
        {
            StartDownLoad(info);
        }
        else
        {
            if (_current_strategy.AssetType == eUpdateAssetType.Zip)
            {
                FileManifestManager.RemoveLocalFenBaoData(info.ToAssetDataInfo());
            }
            else
            {
                FileManifestManager.RemoveLocalABData(info.ToAssetDataInfo());
            }

            AssetDownLoads.StopALLDownLoad();
        }

        if (!_downLoad_error.Contains(info))
            _downLoad_error.Add(info);
    }

    protected override void OnDownloadSuccessful(AssetDownInfo info, HttpDownLoadAsync loadAsync)
    {
        Debug.Log("Asset下载成功！");
        if (_downLoad_error.Contains(info))
            _downLoad_error.Remove(info);

        if (StageDataBase.CheckVersionIsDownLoadComplete(info.Version))
        {
            // 一个版本下载完毕
            VersionDownLoadComplete(info.Version);
        }

        _current_strategy.TryRemove(info);
 
        if (IsDone)
        {
            if (_stage_index < (_strategy_bases.Length - 1))
            {
                _stage_index++;
                Debug.Log("换阶段了 STAGE_INDEX=" + _stage_index);
            }
            else
            {
                if (_downLoad_error.Count <= 0)
                    UpdateStageResult.DownLoad.ClearAll();

                OnAllDownLoadComplete();
                if (DownLoadCompleteEvent != null)
                    DownLoadCompleteEvent(this);
                FileManifestManager.WriteABDataByCurrent(); 

                isDone = true;

                Debug.Log("全部下载完成!!!!!!!");
            }
        }
        else
        {
            AssetDataInfo dataInfo = info.ToAssetDataInfo();
            if (_current_strategy.AssetType == eUpdateAssetType.Zip)
            {
                FileManifestManager.UpdateLocalFenBaoData(dataInfo);
            }
            else
            {
                FileManifestManager.UpdateLocalABData(dataInfo);
            }
        }

        TryStartDownLoad();
    }

    protected void VersionDownLoadComplete(string version)
    {
        IServerVersionFileManifest versionFile = FileManifestManager.Get<IServerVersionFileManifest>();
        versionFile.WriteVersion(version);
    }

    #endregion

    #region 开始下载

    public override void StartDownLoad()
    {
        TryStartDownLoad();
    }

    void TryStartDownLoad()
    {
        UpdateStageResult.DownLoad.IsEnable = true;
        _current_strategy.InitializeData();

        List<AssetDownInfo> downInfos = _current_strategy.TryGetDownLoadData();
        int count = downInfos == null ? 0 : downInfos.Count;
        if (count <= 0) 
            return;
        for (int i = 0; i < count; i++)
        {
            AssetDownInfo info = downInfos[i];
            StartDownLoad(info);
        }
    }

    void StartDownLoad(AssetDownInfo info)
    {
        HttpDownLoadAsync loadAsync = CreateHttpDownLoad();
        loadAsync.Setup(info);
        loadAsync.StartDownLoad(_current_strategy.GetDownLoadParam(info));
    }
 
    #endregion

    public override eWriteFileMode EWriteFileMode
    {
        get { return eWriteFileMode.Auto; }
    }

    public override bool IsDone
    {
        get
        {
            for (int i = 0; i < httpDownloadAsyncs.Count; i++)
            {
                if (!httpDownloadAsyncs[i].IsDone)
                    return false;
            }
            if (_current_strategy.IsDone)
                return true;

            return false;
        }
    }

}
