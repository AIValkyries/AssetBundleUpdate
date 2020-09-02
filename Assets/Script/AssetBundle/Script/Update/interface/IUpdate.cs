/****************************************************
	文件：IUpdate.cs
	作者：Lonely
	github：https://github.com/AIValkyries/AssetBundleUpdate
	日期：2020/08/23 21:55:56
	功能：Nothing
*****************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public interface IUpdate
{
    void Update();
}

public static class Updates
{
    static List<IUpdate> _updates = new List<IUpdate>();

    public static void Add(IUpdate update)
    {
        if (!_updates.Contains(update))
            _updates.Add(update);
    }

    public static void Remove(IUpdate update)
    {
        if (_updates.Contains(update))
            _updates.Remove(update);
    }

    public static void UpdateALL()
    {
        if (_updates.Count <= 0)
            return;

        for (int i = 0; i < _updates.Count; i++)
        {
            _updates[i].Update();
        }
    }

    public static void Clear()
    {
        _updates.Clear();
    }

}

public interface IGUI
{
    void OnGUI();
}

public static class GUIS
{
    static List<IGUI> gUIs = new List<IGUI>();

    public static void OnGUIALL()
    {
        if (gUIs.Count <= 0)
            return;

        for (int i = 0; i < gUIs.Count; i++)
        {
            gUIs[i].OnGUI();
        }
    }


    public static void Add(IGUI gUI)
    {
        if (!gUIs.Contains(gUI))
        {
            gUIs.Add(gUI);
        }
    }

    public static void Remove(IGUI gUI)
    {
        if (gUIs.Contains(gUI))
        {
            gUIs.Remove(gUI);
        }
    }

    public static void Clear()
    {
        gUIs.Clear();
    }

}