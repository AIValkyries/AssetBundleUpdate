/****************************************************
	文件：HttpDownLoadAsync.cs
	作者：Lonely
	github：https://github.com/AIValkyries/AssetBundleUpdate
	日期：2020/08/07 19:19:40
	功能：Nothing
*****************************************************/

using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;
using System.IO;
using System;
using System.Threading;


public class HttpDownLoadAsync
{
    readonly object LOCK_OBJ = new object();
    // 超时时间(毫秒)
    const int TIMEOUT_TIME = 10000;

    const int BUFFER_SIZE = 1024;
    byte[] _buffer;
    string _local_fileName;
    bool _is_stop_download;

    AssetDownInfo _down_info;
    IDownloaderEvent _downloader_event;

    HttpWebRequest _http_request;
    HttpWebResponse _http_response;
    string _local_path;
    bool _is_done;

    public HttpDownLoadAsync(IDownloaderEvent downloaderEvent)
    {
        _local_path = AssetsCommon.LocalAssetPath;
        _downloader_event = downloaderEvent;
    }

    // 开始下载
    public void Setup(AssetDownInfo downInfo)
    {
        _down_info = downInfo;
        _is_stop_download = false;
        _is_done = false;

        _local_fileName = _local_path + "/" + downInfo.AssetName;
        _buffer = new byte[BUFFER_SIZE];
    }

    public void StartDownLoad(IDownLoadParam downLoadParam)
    {
        //Debug.Log("开始下载:" + downLoadParam.GetURL());
        try
        {
            lock (LOCK_OBJ)
            {
                _http_request = (HttpWebRequest)HttpWebRequest.Create(downLoadParam.GetURL());
                _http_request.AddRange(_down_info.DownLoadSize);
                DownLoadCommon.AddHttpHeader(
                    _http_request,
                    downLoadParam.GetDownLoadType(),
                    downLoadParam.GetVersion());

                IAsyncResult ar = _http_request.BeginGetResponse(ResponseCallback, _http_request);
                RegisterTimeOut(ar.AsyncWaitHandle);  // 检测是否超时
            }
        }
        catch (Exception e)
        {
            Debug.Log(e.Message);
            UnregisterTimeOut();
            DownLoadError(eDownErrorCode.NoResponse);
        }
    }

    void ResponseCallback(IAsyncResult ar)
    {
        lock (LOCK_OBJ)
        {
            _http_response = (HttpWebResponse)_http_request.EndGetResponse(ar);
            BeginRead();
        }
    }

    void BeginRead()
    {
        lock (LOCK_OBJ)
        {
            if (IsFailed)
                return;
            if (_is_stop_download)
                return;

            Stream stream = _http_response.GetResponseStream();

            if (_down_info.Buffer.Bytes == null)
            {
                int bufferSize = _down_info.Buffer.ByteSize == 0 ? BUFFER_SIZE : _down_info.Buffer.ByteSize;
                _down_info.Buffer.Bytes = new byte[bufferSize];
                _down_info.BufferNumber = 0;
            }

            stream.BeginRead(_buffer, 0, BUFFER_SIZE, EndRead, _down_info);
        }
    }

    void EndRead(IAsyncResult ar)
    {
        lock (LOCK_OBJ)
        {
            Stream stream = _http_response.GetResponseStream();

            if (_http_response.StatusCode != HttpStatusCode.OK) 
            {
                Debug.LogError("HTTP下载错误 HttpStatusCode:" + _http_response.StatusCode);
                DownLoadError(eDownErrorCode.DownloadError);
                return;
            }

            int streamLength = stream.EndRead(ar);

            if (streamLength <= 0)
            {
                // 下载完成
                if (_downloader_event.EWriteFileMode == eWriteFileMode.Auto)
                {
                    bool writeRes = WriteNewFile(true);
                    if (!writeRes)
                    {
                        //Debug.Log("下载错误!!" + _local_fileName);
                        if (File.Exists(_local_fileName))
                        {
                            try
                            {
                                File.Delete(_local_fileName);
                            }
                            catch (System.Exception ex)
                            {
                                Debug.LogError(ex.Message);
                            }
                        }
                        // 下载出错了重新下载
                        DownLoadError(eDownErrorCode.DownloadError);
                    }
                    else
                    {
                        DownLoadFinish();
                    }
                }
                else
                {
                    DownLoadFinish();
                }
                Close();
                return;
            }

            if (_down_info.Buffer.Bytes.Length == _down_info.BufferNumber) 
            {
                int totalCount = _down_info.Buffer.Bytes.Length + _down_info.BufferNumber;
                byte[] bytes = new byte[totalCount];

                for (int i = 0; i < _down_info.Buffer.Bytes.Length; i++) 
                {
                    bytes[i] = _down_info.Buffer.Bytes[i];
                }
                _down_info.Buffer.Bytes = bytes;
            }

            for (int i = 0; i < streamLength; i++)
            {
                _down_info.Buffer.Bytes[i + _down_info.BufferNumber] = _buffer[i];
            }
            _down_info.BufferNumber += streamLength;
            if (_downloader_event != null)
                _downloader_event.DownloadProgress(_down_info, streamLength);

            BeginRead();
        }
    }

    public void StopDownLoad()
    {
        _is_stop_download = true;
        _is_done = true;

        Close();
        WriteNewFile(false);
    }

    public void DownLoadFinish()
    {
        lock (LOCK_OBJ)
        {
            _is_done = true;
            IsFailed = false;
            _down_info.ErrorCode = eDownErrorCode.None;
            if (_downloader_event != null)
                _downloader_event.DownloadSuccessful(_down_info, this);
        }
    }

    public void Abort()
    {
        lock (LOCK_OBJ)
        {
            DownLoadError(eDownErrorCode.Abort);
        }
    }

    void DownLoadError(eDownErrorCode errorCode)
    {
        _is_done = true;
        IsFailed = true;
        _down_info.ErrorNumber++;
        _down_info.ErrorCode = errorCode;
        if (_downloader_event != null)
            _downloader_event.DownLoadError(_down_info, this);

        Debug.Log("下载错误=" + _down_info.AssetName);

        Close();
    }

    void Close()
    {
        lock (LOCK_OBJ)
        {
            if (_http_request != null)
            {
                _http_request.Abort();
                _http_request = null;
            }

            if (_http_response != null)
            {
                _http_response.Close();
                _http_response = null;
            }
        }
    }

    public bool IsDone { get { return _is_done; } }
    public bool IsFailed { get; private set; }


    #region 将下载的文件内容写入到本地

    bool CompareMD5(byte[] totalBytes)
    {
        string md5 = FileUtils.GetBytesMD5(totalBytes);
        if (md5 != _down_info.MD5)
            return false;
        return true;
    }

    bool WriteNewFile(bool compareMd5)
    {
        FileStream fileStream;
        if (!File.Exists(_local_fileName))
            fileStream = File.Create(_local_fileName);
        else
            fileStream = File.Open(_local_fileName, FileMode.Open);

        // 合并bytes
        byte[] totalBytes = MergeBytes(fileStream);

        // 是否需要比较md5
        if (compareMd5)
        {
            bool isOk = CompareMD5(totalBytes);
            if (!isOk)   // 下载错误
            {
                Debug.Log("SIZE=" + _down_info.TotalSize);
                fileStream.Dispose();
                fileStream.Close();
                return false;
            }
        }

        fileStream.Write(totalBytes, 0, totalBytes.Length);
        fileStream.Dispose();
        fileStream.Close();

        return true;
    }

    byte[] MergeBytes(FileStream fileStream)
    {
        int totalLength = _down_info.DownLoadSize + _down_info.BufferNumber;
        byte[] totalBytes = new byte[totalLength];

        int offset = 0;
        for (int i = 0; i < _down_info.DownLoadSize; i++)
        {
            totalBytes[i] = _down_info.DownloadedBuffer.Bytes[i];
            offset++;
        }

        for (int i = 0; i < _down_info.BufferNumber; i++)
        {
            totalBytes[i + offset] = _down_info.Buffer.Bytes[i];
        }

        return totalBytes;
    }

    #endregion 

    #region TIME_OUT

    RegisteredWaitHandle _registered_handle;
    WaitHandle _handle;

    void RegisterTimeOut(WaitHandle handle)
    {
        _handle = handle;
        _registered_handle = ThreadPool.RegisterWaitForSingleObject(
            handle,
            OnTimeoutCallback, 
            _down_info, 
            TIMEOUT_TIME,
            true);
    }

    void UnregisterTimeOut()
    {
        if (_registered_handle != null && _handle != null) 
            _registered_handle.Unregister(_handle);
    }

    void OnTimeoutCallback(object state, bool timedOut)
    {
        lock (LOCK_OBJ)
        {
            if (timedOut)
            {
                DownLoadError(eDownErrorCode.TimeOut);
            }
            UnregisterTimeOut();
        }
    }

    #endregion

}
