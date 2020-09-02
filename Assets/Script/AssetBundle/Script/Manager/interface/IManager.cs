/****************************************************
	文件：IManager.cs
	作者：Lonely
	github：https://github.com/AIValkyries/AssetBundleUpdate
	日期：2020/08/18 20:56:14
	功能：Nothing
*****************************************************/

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public interface IManager
{
    void Startup(System.Object param = null);

    void Terminate(System.Object param = null);
}

public static class Managers
{
    static Dictionary<Type, IManager> _managers = new Dictionary<Type, IManager>();

    public static void Add(IManager managerBase)
    {
        if (!_managers.ContainsKey(managerBase.GetType()))
            _managers.Add(managerBase.GetType(), managerBase);
    }

    public static void Add(List<IManager> managers)
    {
        if (managers == null)
            return;
        for (int i = 0; i < managers.Count; i++)
        {
            Add(managers[i]);
        }
    }

    public static void Remove(IManager managerBase)
    {
        if (_managers.ContainsKey(managerBase.GetType()))
            _managers.Remove(managerBase.GetType());
    }

    public static void Clear()
    {
        _managers.Clear();
    }

    public static ResourcesSceneManager SceneManager
    {
        get
        {
            Type type = typeof(ResourcesSceneManager);
            if (_managers.ContainsKey(type))
            {
                return _managers[type] as ResourcesSceneManager;
            }
            return null;
        }
    }

    public static ResourcesManager Resources
    {
        get
        {
            Type type = typeof(ResourcesManager);
            if (_managers.ContainsKey(type))
            {
                return _managers[type] as ResourcesManager;
            }
            return null;
        }
    }

    public static ResourcesUpdateManager ResourcesUpdate
    {
        get
        {
            Type type = typeof(ResourcesUpdateManager);
            if (_managers.ContainsKey(type))
            {
                return _managers[type] as ResourcesUpdateManager;
            }
            return null;
        }
    }
}