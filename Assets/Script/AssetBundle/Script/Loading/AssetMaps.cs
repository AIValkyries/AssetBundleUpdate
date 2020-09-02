/****************************************************
	文件：AssetMapsManager.cs
	作者：Lonely
	github：https://github.com/AIValkyries/AssetBundleUpdate
	日期：2020/08/04 22:27:13
	功能：Nothing
*****************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 有些资源路径被设置成了独立的ab名称，可以在uiwindow中配置
public static class AssetMaps
{
    static Dictionary<string, string> _asset_maps;

    static AssetMaps()
    {
        _asset_maps = new Dictionary<string, string>();
        string assetPath = "Assets/L_Resources/AssetMaps.txt";

        TextAsset textAsset = null;

        string assetMapsPath = AssetsCommon.LocalAssetPath + "/l_resources.assetmaps";
        AssetBundle assetMapsAB = AssetBundle.LoadFromFile(assetMapsPath);
        if (assetMapsAB != null)
            textAsset = assetMapsAB.LoadAsset<TextAsset>(assetPath);
        else
        {
#if UNITY_EDITOR
            textAsset = UnityEditor.AssetDatabase.LoadAssetAtPath<TextAsset>(assetPath);
#endif
        }

        if (textAsset == null)
            return;

        string[] texts = textAsset.text.Split('\n');

        for (int i = 0; i < texts.Length; i++)
        {
            if (texts[i] == string.Empty || texts[i] == " ")
                continue;
            string[] assetMaps = texts[i].Split('|');
            if (assetMaps.Length >= 2)
            {
                assetMaps[0] = assetMaps[0].Replace("/", ".").ToLower();
                assetMaps[1] = assetMaps[1].Replace("/", ".").ToLower();
                _asset_maps.Add(assetMaps[0], assetMaps[1]);
            }
        }

        assetMapsAB.Unload(true);
    }

    public static bool HasAssetBundleName(string assetPath)
    {
        if (_asset_maps == null)
            return false;
        if (!_asset_maps.ContainsKey(assetPath))
            return false;
        return true;
    }

    public static string GetAssetBundleNameByAssetPath(string assetPath)
    {
        string assetBundleName = assetPath.Replace("/", ".").ToLower();
        if (HasAssetBundleName(assetBundleName))
        {
            return _asset_maps[assetBundleName];
        }
        return string.Empty;
    }

}
