/****************************************************
文件：AssetBundleUnpack.cs
作者：Lonely
github：https://github.com/AIValkyries/AssetBundleUpdate
日期：2020/06/26 22:13:55
功能：ab打包
*****************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using System.Text;

public class AssetBundleUnpack
{
    #region luaFile

    public static void ClearLuaFile()
    {
        string luaPath = Application.dataPath + "/L_Lua";
        ClearLuaFile(luaPath);
        AssetDatabase.Refresh();
    }

    static void ClearLuaFile(string path)
    {
        DirectoryInfo directoryInfo = new DirectoryInfo(path);
        DirectoryInfo[] childs = directoryInfo.GetDirectories();

        for (int i = 0; i < childs.Length; i++) 
        {
            ClearLuaFile(childs[i].FullName);
        }

        FileInfo[] fileInfos = directoryInfo.GetFiles();

        for (int i = 0; i < fileInfos.Length; i++)
        {
            string ext = Path.GetExtension(fileInfos[i].FullName);
            if (ext.Equals(".meta") || ext.Equals(".lua"))
                continue;
            if (ext.Equals(".bytes"))
                File.Delete(fileInfos[i].FullName);
        }
    }

    public static void CopyLuaToTempDir()
    {
        string luaPath = Application.dataPath + "/L_Lua";
        CopyLuaToTempDir(luaPath);
        AssetDatabase.Refresh();
    }

    static void CopyLuaToTempDir(string path)
    {
        DirectoryInfo parent = new DirectoryInfo(path);
        DirectoryInfo[] childs = parent.GetDirectories();

        for (int i = 0; i < childs.Length; i++) 
        {
            CopyLuaToTempDir(childs[i].FullName);
        }

        FileInfo[] fileInfos = parent.GetFiles();

        for (int i = 0; i < fileInfos.Length; i++) 
        {
            string ext = Path.GetExtension(fileInfos[i].FullName);
            if (ext.Equals(".meta"))
                continue;

            string newFileName = fileInfos[i].FullName.Replace(".lua", ".bytes");

            using (FileStream rd = File.Open(fileInfos[i].FullName, FileMode.Open))
            {
                byte[] source = new byte[rd.Length];
                rd.Read(source, 0, source.Length);

                using (FileStream wd = File.Create(newFileName)) 
                {
                    wd.Write(source, 0, source.Length);
                    wd.Close();
                }
                rd.Close();
            } 
        }

    }

    #endregion

    public static void CopyABToServer()
    {
        ePlatformType osType = UnpackCommon.GetOsType();
        string abSourcePath = UnpackPath.Get();
        string abTargetPath = AssetBundleServerPath.ABCache.GetABCachePath(osType);

        DirectoryInfo source = new DirectoryInfo(abSourcePath);

        FileInfo[] fileInfos = source.GetFiles();
        for (int i = 0; i < fileInfos.Length; i++) 
        {
            string abSourceFileName = fileInfos[i].FullName;
            string abTargetFileName = abTargetPath + "/"+ fileInfos[i].Name;
            if(File.Exists(abTargetFileName))
                File.Delete(abTargetFileName);

            File.Copy(abSourceFileName, abTargetFileName);
        }
    }

    public static void ClearNames()
    {
        AssetBundleSettingInfo settingInfo = UnpackCommon.GetSettingInfo();
        for (int i = 0; i < settingInfo.UnpackPath.Count; i++)
        {
            string assetPath = Application.dataPath + "/" + settingInfo.UnpackPath[i];
            ClearAssetBundleName(assetPath);
        }
        Debug.Log("清除完成!");
    }

    // 所有的着色器打成一个包
    public static void ResetNames()
    {
        ClearNames();
        AssetBundleSettingInfo settingInfo = UnpackCommon.GetSettingInfo();

        for (int i = 0; i < settingInfo.UnpackPath.Count; i++) 
        {
            string sourcePath = Application.dataPath + "/" + settingInfo.UnpackPath[i];
            SetAssetBundleName(sourcePath);
        }

        Debug.Log("设置完成!");
    }

    public static void BuildAssetBundle(BuildTarget target)
    {
        ResVersionEditor.Current.Read();

        ClearLuaFile();
        CopyLuaToTempDir();

        if (UnpackCommon.GetSettingInfo().ResetAssetBundleName)
            ResetNames();

        AssetBundleManifest manifest = BuildPipeline.BuildAssetBundles(
            UnpackPath.Get(),
            UnpackCommon.GetSettingInfo().Options,
            target);

        ResVersionEditor.Current.Save(PackageType.OnlyABPack);
        AssetBundlesFileInfoEditor.Current.Save(manifest.GetAllAssetBundles());

        ClearLuaFile();
        CopyABToServer();
    }

    static void ClearAssetBundleName(string fullName)
    {
        DirectoryInfo directory = new DirectoryInfo(fullName);
        DirectoryInfo[] childs = directory.GetDirectories();

        for (int i = 0; i < childs.Length; i++)
        {
            string assetPath = childs[i].FullName;
            ClearAssetBundleName(assetPath);
        }
        FileInfo[] fileInfos = directory.GetFiles();

        for (int i = 0; i < fileInfos.Length; i++)
        {
            string ext = Path.GetExtension(fileInfos[i].FullName);
            if (UnpackCommon.GetSettingInfo().IgnoreSuffix.Contains(ext))
                continue;

            string resPath = GetAssetPath(fileInfos[i].FullName);
            AssetImporter importer = AssetImporter.GetAtPath(resPath);
            importer.assetBundleName = string.Empty;
        }
    }

    static void SetAssetBundleName(string sourcePath)
    {
        string relativelyAssetPath = GetRelativelyAssetPath(sourcePath);
        string assetBundleName = GetAssetBundleNameBySettingFile(relativelyAssetPath);

        DirectoryInfo directory = new DirectoryInfo(sourcePath);
        DirectoryInfo[] childs = directory.GetDirectories();

        for (int i = 0; i < childs.Length; i++) 
        {
            string assetPath = childs[i].FullName;
            SetAssetBundleName(assetPath);
        }

        FileInfo[] fileInfos = directory.GetFiles();

        for (int i = 0; i < fileInfos.Length; i++)
        {
            string ext = Path.GetExtension(fileInfos[i].FullName);
            if (UnpackCommon.GetSettingInfo().IgnoreSuffix.Contains(ext))
                continue;

            string resPath = GetAssetPath(fileInfos[i].FullName);
            AssetImporter importer = AssetImporter.GetAtPath(resPath);
            if (assetBundleName != string.Empty)
                importer.assetBundleName = assetBundleName;
            else
            {
                string abName = GetAssetBundleNameByFileName(fileInfos[i].FullName);
                importer.assetBundleName = abName;
            }
        }
    }

    static string GetAssetBundleNameBySettingFile(string relativelyAssetPath)
    {
        var itr = UnpackCommon.GetSettingInfo().AssetsMap.Keys.GetEnumerator();
        while (itr.MoveNext())
        {
            string path = itr.Current;
            if (itr.Current == relativelyAssetPath) 
            {
                return UnpackCommon.GetSettingInfo().AssetsMap[itr.Current];
            }
        }
        itr.Dispose();

        return string.Empty;
    }

    static string GetAssetBundleNameByFileName(string fullName)
    {
        string ext = Path.GetExtension(fullName);
        string assetPath = "";
        string formatPath = fullName.Replace("\\", ".");
        int index = formatPath.IndexOf("Assets");
        if (index != -1) 
        {
            index += 7;
            assetPath = formatPath.Substring(index);
            assetPath = assetPath.Replace(ext,string.Empty);
        }

        return assetPath;
    }

    static string GetRelativelyAssetPath(string fullPath)
    {
        string assetPath = "";
        string formatPath = fullPath.Replace("\\","/");
        int index = formatPath.LastIndexOf("Assets");
        if (index != -1)
        {
            index += 7;
            assetPath = formatPath.Substring(index);
        }

        return assetPath;
    }

    static string GetAssetPath(string fullPath)
    {
        string assetPath = "";
        string formatPath = fullPath.Replace("\\", "/");
        int index = formatPath.IndexOf("Assets");
        if (index != -1)
            assetPath = formatPath.Substring(index);
        return assetPath;
    }

    public static string GetAssetBundlePathByABName(string assetBundleName)
    {
        string str = assetBundleName;
        str = str.Replace(".", "/");
        return Application.streamingAssetsPath + "/" + str;
    }



}
