/****************************************************
	文件：IManager.cs
	作者：Lonely
	github：https://github.com/AIValkyries/AssetBundleUpdate
	日期：2020/08/05 19:44:30
	功能：Nothing
*****************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


public abstract class ResManger : IManager
{
    protected static AssetBundleLoad _resource_load;

    public static void SetupAssetBundle()
    {
        _resource_load = new AssetBundleLoad();
        _resource_load.Setup();
    }

    public virtual void Startup(object param = null)
    {
       
    }

    public virtual void Terminate(object param = null)
    {
       
    }
}

 
