/****************************************************
	文件：ZipDecompressStageStrategy.cs
	作者：Lonely
	github：https://github.com/AIValkyries/AssetBundleUpdate
	日期：2020/08/25 15:40:36
	功能：Nothing
*****************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;


public class ZipDecompressStage
{
    List<AssetDownInfo> _decompress_queue;
    StageDataBase _owner;
    bool _is_done;
    int _index;

    public ZipDecompressStage(StageDataBase stage)
    {
        _owner = stage;
        _decompress_queue = new List<AssetDownInfo>();
        List<AssetDataInfo> localAllZip = FileManifestManager.LocalFenBao.GetAllZip();
        if (localAllZip == null)
            return;

        for (int i = 0; i < localAllZip.Count; i++)
        {
            AssetDataInfo dataInfo = localAllZip[i];
            _decompress_queue.Add(dataInfo.ToAssetDownInfo(i));
        }
        _index = localAllZip.Count;
    }

    public void Enqueue(AssetDownInfo info)
    {
        _decompress_queue.Add(info);
    }

    // 开始解压缩
    public void StartDecompress()
    {
        UpdateStageResult.DownLoad.IsEnable = false;
        UpdateStageResult.Compression.IsEnable = true;
        UpdateStageResult.Compression.ClearAll();

        var deItr = _decompress_queue.GetEnumerator();
        while (deItr.MoveNext())
        {
            UpdateStageResult.Compression.TotalSize += deItr.Current.TotalSize;
            UpdateStageResult.Compression.FileCount++;
        }
        deItr.Dispose();

        string localPath = AssetsCommon.LocalAssetPath;
        if (!Directory.Exists(localPath))
            Directory.CreateDirectory(localPath);

        AssetDownInfo[] tempDatas = new AssetDownInfo[_decompress_queue.Count];
        for (int i = 0; i < tempDatas.Length; i++) 
        {
            int index = _decompress_queue[i].Index;
            tempDatas[index] = _decompress_queue[i];
        }

        for (int i = 0; i < tempDatas.Length; i++)
        {
            AssetDownInfo downInfo = tempDatas[i];

            string zipFileName = string.Format("{0}/{1}", localPath, downInfo.AssetName);
            List<AssetDataInfo> assetNames = ZipHelper.Decompress(
                zipFileName,
                localPath);

            AssetDataInfo dataInfo = downInfo.ToAssetDataInfo();
            dataInfo.IsCompressed = true;

            UpdateStageResult.Compression.CurrentCount++;
            FileManifestManager.UpdateLocalFenBaoData(dataInfo);

            for (int j = 0; j < assetNames.Count; j++)
            {
                FileManifestManager.UpdateLocalABData(assetNames[j]);
            }

            if (File.Exists(zipFileName))
                File.Delete(zipFileName);
        }
    
        UpdateStageResult.Compression.IsEnable = false;

        if (_owner.IsDownLoadAllVersion)
        {
            FileManifestManager.WriteFenBaoDataByServer();
        }
        else
        {
            FileManifestManager.WriteFenBaoDataByCurrent();
        }

        FileManifestManager.WriteABDataByCurrent();
    }

    public int Index
    {
        get { return _index; }
    }

    public bool IsDone
    {
        get { return _is_done; }
    }
}
