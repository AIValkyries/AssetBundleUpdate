/****************************************************
	文件：ResVersion.cs
	作者：Lonely
	github：https://github.com/AIValkyries/AssetBundleUpdate
	日期：2020/07/19 19:40:44
	功能：版本文件信息
*****************************************************/

using System.IO;
using UnityEngine;


public class VersionFileManifest : ILocalVersionFileManifest
{
    public static string DEFAUIT_VERSION = "0.0";

    VersionInfo _local_version;

    public void Read()
    {
        string fileName = GetLocalVersionFileName();
        if (!File.Exists(fileName))
        {
            FileStream fs = File.Create(fileName);
            fs.Dispose();
            fs.Close();

            _local_version = new VersionInfo();
            _local_version.Version = DEFAUIT_VERSION;
        }
        else
        {
            using (FileStream fs = File.Open(fileName, FileMode.Open))
            {
                byte[] bytes = new byte[fs.Length];
                fs.Read(bytes, 0, bytes.Length);

                string context = System.Text.Encoding.Default.GetString(bytes);
                if (context == string.Empty)
                {
                    _local_version = new VersionInfo();
                    _local_version.Version = DEFAUIT_VERSION;
                }
                else
                {
                    _local_version = JsonUtility.FromJson<VersionInfo>(context);
                    fs.Close();
                }
              
            }
        }
    }

    public void Write(VersionInfo info)
    {
        _local_version = info;
        string fileName = GetLocalVersionFileName();
        if (File.Exists(fileName))
            File.Delete(fileName);

        using (FileStream fs = File.Create(fileName))
        {
            string context = JsonUtility.ToJson(info);
            byte[] bytes = System.Text.Encoding.Default.GetBytes(context);

            fs.Write(bytes, 0, bytes.Length);
            fs.Close();
        }
    }

    public string GetLocalVersionFileName()
    {
        string abPath = AssetsCommon.LocalAssetPath;
        return string.Format("{0}/{1}", abPath, PathConstant.FileName.VERSION);
    }

    public string Version
    {
        get
        {
            return _local_version == null ? 
                string.Empty : _local_version.Version;
        }
    }

}


