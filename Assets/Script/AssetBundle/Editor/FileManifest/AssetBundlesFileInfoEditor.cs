/****************************************************
	文件：ABAssetInfoEditor.cs
	作者：Lonely
	github：https://github.com/AIValkyries/AssetBundleUpdate
	日期：2020/07/19 20:13:22
	功能：Nothing
*****************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Text;


public class AssetBundlesFileInfoEditor
{
    static AssetBundlesFileInfoEditor _ab_asset_info_editor;
    public static AssetBundlesFileInfoEditor Current
    {
        get
        {
            if (_ab_asset_info_editor == null) 
                _ab_asset_info_editor = new AssetBundlesFileInfoEditor();
            return _ab_asset_info_editor;
        }
    }

    public void Save(object @object)
    {
        string[] assetBundlePaths = @object as string[];
        ABPathInfo pathInfo = new ABPathInfo(UnpackCommon.GetOsType(), UnpackCommon.GetVersion());
        string abAssetPath = AssetBundleServerPath.FileManifest.GetABAssetInfoFileName(pathInfo);

        StringBuilder @string = new StringBuilder();
       
        for (int i = 0; i < assetBundlePaths.Length; i++)
        {
            long assetByteSize = 0;
            string path = UnpackPath.GetABFile(assetBundlePaths[i]);
            string md5 = FileUtils.GetFileMD5(path, ref assetByteSize);

            @string.Append(assetBundlePaths[i] +
                "|" + md5 + "|" + assetByteSize + "\n");
        }

        if (File.Exists(abAssetPath))
            File.Delete(abAssetPath);

        using (FileStream stream = File.Create(abAssetPath))
        {
            byte[] bytes = Encoding.Default.GetBytes(@string.ToString());
            stream.Write(bytes, 0, bytes.Length);
            stream.Close();
        }

    }
}
