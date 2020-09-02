using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public abstract class AssetDownLoadBase : IAssetDownLoad, IUpdate, IDownloaderEvent
{
    protected readonly object LOCK_OBJ = new object();
    public Action<AssetDownLoadBase> DownLoadCompleteEvent;

    protected bool isDone;
    protected List<HttpDownLoadAsync> httpDownloadAsyncs;

    public AssetDownLoadBase()
    {
        Updates.Add(this);
        AssetDownLoads.Add(this);
        downLoad_error_events = new Queue<DownLoadErrorEvent>();
        download_successful_events = new Queue<DownloadSuccessfulEvent>();
        httpDownloadAsyncs = new List<HttpDownLoadAsync>();
    }

    #region IAssetDownLoad

    public void Abort()
    {
        var itr = httpDownloadAsyncs.GetEnumerator();
        while (itr.MoveNext())
        {
            itr.Current.Abort();
        }
        itr.Dispose();
    }

    public void StopDownLoad()
    {
        var itr = httpDownloadAsyncs.GetEnumerator();
        while (itr.MoveNext())
        {
            itr.Current.StopDownLoad();
        }
        itr.Dispose();

        FileManifestManager.WriteABDataByCurrent();
        FileManifestManager.WriteFenBaoDataByCurrent();
    }

    public abstract void StartDownLoad();

    public void Clear()
    {
        AssetDownLoads.Remove(this);
        Updates.Remove(this);
    }

    #endregion

    #region download event

    Queue<DownloadSuccessfulEvent> download_successful_events;
    Queue<DownLoadErrorEvent> downLoad_error_events;

    public void DownloadProgress(AssetDownInfo info, long increment)
    {
        OnDownloadProgress(info, increment);
    }

    protected virtual void OnDownloadProgress(AssetDownInfo info, long increment) { }

    public void DownLoadError(AssetDownInfo info, HttpDownLoadAsync loadAsync)
    {
        lock (LOCK_OBJ)
        {
            DownLoadErrorEvent errorEvent = new DownLoadErrorEvent(info);
            errorEvent.Event = (dio) =>
            {
                OnDownLoadError(dio);

                if (httpDownloadAsyncs.Contains(loadAsync))
                    httpDownloadAsyncs.Remove(loadAsync);
            };
            downLoad_error_events.Enqueue(errorEvent);
        }
    }

    protected virtual void OnDownLoadError(AssetDownInfo info) { }

    public void DownloadSuccessful(AssetDownInfo info, HttpDownLoadAsync loadAsync)
    {
        lock (LOCK_OBJ)
        {
            DownloadSuccessfulEvent successfulEvent = new DownloadSuccessfulEvent(info, loadAsync);
            successfulEvent.Event = (dif, la) =>
            {
                OnDownloadSuccessful(dif, la);

                if (httpDownloadAsyncs.Contains(loadAsync))
                    httpDownloadAsyncs.Remove(loadAsync);
            };

            download_successful_events.Enqueue(successfulEvent);
        }
    }

    protected virtual void OnDownloadSuccessful(AssetDownInfo info, HttpDownLoadAsync loadAsync) { }
    protected virtual void OnAllDownLoadComplete() { }

    #endregion

    #region IUpdate

    public void Update()
    {
        if (download_successful_events.Count > 0)
        {
            DownloadSuccessfulEvent successfulEvent = download_successful_events.Dequeue();
            successfulEvent.Callback();
        }

        if (downLoad_error_events.Count > 0)
        {
            DownLoadErrorEvent errorEvent = downLoad_error_events.Dequeue();
            errorEvent.Callback();
        }

        OnUpdate();
    }

    protected virtual void OnUpdate() { }

    #endregion

    protected int GetFenBaoIndex(string Name)
    {
        string[] str = Name.Split('.');
        if (str == null || str.Length <= 0)
            return -1;

        string fileNameNotExt = str[0];

        string[] fileNameStr = fileNameNotExt.Split('_');
        if (fileNameStr == null || fileNameStr.Length <= 0)
            return -1;

        int fenBaoIndex = System.Convert.ToInt32(fileNameStr[fileNameStr.Length - 1]);
        return fenBaoIndex;
    }

    protected HttpDownLoadAsync CreateHttpDownLoad()
    {
        HttpDownLoadAsync httpDownLoadAsync = new HttpDownLoadAsync(this);
        httpDownloadAsyncs.Add(httpDownLoadAsync);
        return httpDownLoadAsync;
    }

    protected void ClearHttpDownLoad()
    {
        httpDownloadAsyncs.Clear();
    }

    public abstract eWriteFileMode EWriteFileMode { get; }

    public virtual bool IsDone
    {
        get { return isDone; }
    }
}
