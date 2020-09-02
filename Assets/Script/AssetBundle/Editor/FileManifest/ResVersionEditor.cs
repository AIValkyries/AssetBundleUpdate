/****************************************************
	文件：ResVersionEditor.cs
	作者：Lonely
	github：https://github.com/AIValkyries/AssetBundleUpdate 
	日期：2020/07/19 20:01:17
	功能：Nothing
*****************************************************/

using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

public class ResVersionEditor
{
    static ResVersionEditor _res_version_editor;
    public static ResVersionEditor Current
    {
        get
        {
            if (_res_version_editor == null) 
                _res_version_editor = new ResVersionEditor();
            return _res_version_editor;
        }
    }

    // 所有版本列表
    List<string> _all_version_number;
    protected VersionInfo _local_version;

    public void Read()
    {
        string versionPath = AssetBundleServerPath.FileManifest.GetVersionFileName(UnpackCommon.DefaultABPath);
        string versionListPath = AssetBundleServerPath.FileManifest.GetVersionListFileName(UnpackCommon.GetOsType());

        _all_version_number = new List<string>();
        if (!File.Exists(versionListPath))
        {
            using (FileStream fs = File.Create(versionListPath))
            {
                fs.Close();
            }
        }
        else
        {
            using (FileStream fs = File.Open(versionListPath,FileMode.Open))
            {
                byte[] bytes = new byte[fs.Length];
                fs.Read(bytes, 0, bytes.Length);
                string context = Encoding.Default.GetString(bytes);

                string[] allVersion = context.Split('\n');
                _all_version_number.AddRange(allVersion);
                fs.Close();
            }
        }

        if (!File.Exists(versionPath))
        {
            using (FileStream fs = File.Create(versionPath))
            {
                _local_version = new VersionInfo();
                _local_version.Version = UnpackCommon.GetSettingInfo().Version;
                _local_version.OsType = UnpackCommon.Target.ToString();
                _local_version.CurrentTime = System.DateTime.Now.ToString();

                fs.Close();
            }
        }
        else
        {
            string json = File.ReadAllText(versionPath);
            _local_version = JsonUtility.FromJson<VersionInfo>(json);
        }
    }

    public void Save(PackageType packageType)
    {
        // 写入版本
        _local_version = new VersionInfo();
        _local_version.Version = UnpackCommon.GetSettingInfo().Version;
        _local_version.OsType = UnpackCommon.Target.ToString();
        _local_version.CurrentTime = System.DateTime.Now.ToString();
        _local_version.Type = packageType.ToString(); //默认散包

        // 写入assetbundlemanifest

        string unpackPath = UnpackPath.Get();
        string mainAssetBundleName = string.Format("{0}/{1}", unpackPath, UnpackCommon.GetOsType());
        string mainManifestName = string.Format("{0}/{1}.manifest", unpackPath, UnpackCommon.GetOsType());

        long mainAssetBundleLength = 0;
        long mainManifestLength = 0;

        string mainABMD5 = string.Empty;
        string mainManifestMD5 = string.Empty;

        if (File.Exists(mainAssetBundleName))
        {
            using (FileStream fs = File.Open(mainAssetBundleName,FileMode.Open))
            {
                mainAssetBundleLength = fs.Length;
                mainABMD5 = FileUtils.GetFileMD5ByStream(fs);
                fs.Close();
            }
        }

        if (File.Exists(mainManifestName))
        {
            using (FileStream fs = File.Open(mainManifestName, FileMode.Open))
            {
                mainManifestLength = fs.Length;
                mainManifestMD5 = FileUtils.GetFileMD5ByStream(fs);
                fs.Close();
            }
        }

        string fileName = AssetsCommon.GetPlatformFolderName(AssetsCommon.GetPlatform());

        _local_version.AssetBundleManifestNames = new string[2];
        _local_version.AssetBundleManifestLengths = new string[2];
        _local_version.AssetBundleManifestMD5s = new string[2];

        _local_version.AssetBundleManifestNames[0] = fileName;
        _local_version.AssetBundleManifestNames[1] = string.Format("{0}.manifest", fileName);

        _local_version.AssetBundleManifestLengths[0] = mainAssetBundleLength.ToString();
        _local_version.AssetBundleManifestLengths[1] = mainManifestLength.ToString();

        _local_version.AssetBundleManifestMD5s[0] = mainABMD5;
        _local_version.AssetBundleManifestMD5s[1] = mainManifestMD5;

        string jsonStr = JsonUtility.ToJson(_local_version);
        string versionPath = AssetBundleServerPath.FileManifest.GetVersionFileName(UnpackCommon.DefaultABPath);

        if (File.Exists(versionPath))
            File.Delete(versionPath);

        using (FileStream stream = File.Create(versionPath))
        {
            byte[] bytes = Encoding.Default.GetBytes(jsonStr);
            stream.Write(bytes, 0, bytes.Length);
            stream.Close();
        }


        bool hasVersion = false;
        for (int i = 0; i < _all_version_number.Count; i++) 
        {
            if (_all_version_number[i] == _local_version.Version) 
            {
                hasVersion = true;
                break;
            }
        }

        if (!hasVersion)
            _all_version_number.Add(_local_version.Version);

        string versionListPath = AssetBundleServerPath.FileManifest.GetVersionListFileName(UnpackCommon.GetOsType());
        if (File.Exists(versionListPath))
            File.Delete(versionListPath);

        using (FileStream fs = File.Create(versionListPath))
        {
            string context = string.Empty;
            for (int i = 0; i < _all_version_number.Count; i++) 
            {
                if (i == _all_version_number.Count - 1)
                {
                    context += _all_version_number[i];
                }
                else
                {
                    context += _all_version_number[i] + "\n";
                }
            }

            byte[] bytes = System.Text.Encoding.Default.GetBytes(context);
            fs.Write(bytes, 0, bytes.Length);
            fs.Close();
        }

    }



}
