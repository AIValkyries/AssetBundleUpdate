using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public interface IAssetDownLoad
{
    bool IsDone { get; }
    void StartDownLoad();
    void StopDownLoad();
    void Abort();
    void Clear();
}

public static class AssetDownLoads
{
    static List<IAssetDownLoad> assetDownLoads = new List<IAssetDownLoad>();

    public static void Add(IAssetDownLoad downLoad)
    {
        if(!assetDownLoads.Contains(downLoad))
            assetDownLoads.Add(downLoad);
    }

    public static void Remove(IAssetDownLoad downLoad)
    {
        if (assetDownLoads.Contains(downLoad))
            assetDownLoads.Remove(downLoad);
    }

    public static void Clear()
    {
        assetDownLoads.Clear();
    }

    public static void StopALLDownLoad()
    {
        if (assetDownLoads.Count <= 0)
            return;

        for (int i = 0; i < assetDownLoads.Count; i++) 
        {
            assetDownLoads[i].StopDownLoad();
        }
    }

    public static bool IsAllDownLoadComplete
    {
        get
        {
            if (assetDownLoads.Count <= 0)  
                return false;

            for (int i = 0; i < assetDownLoads.Count; i++) 
            {
                if (!assetDownLoads[i].IsDone)
                    return false;
            }
            return true;
        }
    }

    public static int DownLoadCount
    {
        get { return assetDownLoads.Count; }
    }

}





