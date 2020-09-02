/****************************************************
	文件：OnAssetsEvent.cs
	作者：Lonely
	github：https://github.com/AIValkyries/AssetBundleUpdate
	日期：2020/07/08 20:53:23
	功能：Nothing
*****************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System.IO;

public class OnAssetsEvent : UnityEditor.AssetModificationProcessor
{
    public static AssetDeleteResult OnWillDeleteAsset(string assetPath, RemoveAssetOptions option)
    {
        string ext = Path.GetExtension(assetPath);
        if (ext.Equals(".unity"))
        {
            Debug.Log("资源即将被删除 : " + assetPath);

            assetPath = assetPath.Replace(".unity", string.Empty);

            string[] splits = assetPath.Split('/');
            string sceneName = splits[splits.Length - 1];

            string xmlName = PathConstant.PathName.DYNAMIC_SCENE_CONFIG_FILE_PATH + sceneName + ".xml";
            if (File.Exists(xmlName))
            {
                File.Delete(xmlName);
                Debug.Log(string.Format("[{0}]场景的动态文件删除了!", sceneName));
            }

            string prefabPath = PathConstant.PathName.DYNAMIC_SCENE_PREFAB_PATH + sceneName;
            if (Directory.Exists(prefabPath))
            {
                DirectoryInfo parent = new DirectoryInfo(prefabPath);

                FileInfo[] fileInfos = parent.GetFiles();
                for (int i = 0; i < fileInfos.Length; i++) 
                {
                    File.Delete(fileInfos[i].FullName);
                }

                Directory.Delete(prefabPath);
                Debug.Log(string.Format("[{0}]场景的动态Prefab删除了!", sceneName));
            }

            AssetDatabase.Refresh();
        }
        return AssetDeleteResult.DidNotDelete;
    }

}
