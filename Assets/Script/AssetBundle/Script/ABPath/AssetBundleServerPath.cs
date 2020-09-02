/****************************************************
	文件：AssetBundleServerPath.cs
	作者：Lonely
	github：https://github.com/AIValkyries/AssetBundleUpdate
	日期：2020/07/22 19:38:48
	功能：Nothing
*****************************************************/

using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public static class AssetBundleServerPath
{
    public static string URL = "http://127.0.0.1:1500/AssetBundleServer";


    public static class ABCache
    {
        public static string GetABCachePath(ePlatformType osType)
        {
            string assetPath = AssetsCommon.GetAssetPath();
            string assetFolderName = PathConstant.FolderName.AB_CACHE;
            string osFolderName = AssetsCommon.GetPlatformFolderName(osType);

            List<string> paths = new List<string>()
        {
                   PathConstant.SERVER_FOLDER_NAME,
            assetFolderName,
            osFolderName
        };
            string path = AssetsCommon.BuildPath(assetPath, paths);
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);

            return path;
        }
    }


    public static class FenBao
    {
        // 例如:AssetBundlerServer/FenBao/Win/version_1/package0_i
        public static string GetFenBaoPath(ABPathInfo pathInfo)
        {
            string assetPath = AssetsCommon.GetAssetPath();
            string assetFolderName = PathConstant.FolderName.FEN_BAO;
            string osFolderName = AssetsCommon.GetPlatformFolderName(pathInfo.OsType);
            string versionFolderName = PathConstant.FolderName.GetFenBaoVersion(pathInfo.Version);

            if (osFolderName == string.Empty)
            {
                Debug.LogError("平台文件夹名称获取错误:" + pathInfo.OsType);
                return string.Empty;
            }

            List<string> paths = new List<string>()
        {
                PathConstant.SERVER_FOLDER_NAME,
            assetFolderName,
            osFolderName,
            versionFolderName
        };

            string path = AssetsCommon.BuildPath(assetPath, paths);
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);

            return path;
        }

        public static string GetPackageFileName(ABPathInfo pathInfo, int index)
        {
            return PathConstant.FileName.GetPackage(pathInfo.Version, index);
        }

        public static string GetPackageFullFileName(ABPathInfo pathInfo, int index)
        {
            string assetPath = GetFenBaoPath(pathInfo);
            string fileName = PathConstant.FileName.GetPackage(pathInfo.Version, index);

            return AssetsCommon.BuildFileName(assetPath, fileName);
        }
    }

    public static class FileManifest
    {
        public static string GetVersionListPath(ePlatformType osType)
        {
            string assetPath = AssetsCommon.GetAssetPath();
            string assetFolderName = PathConstant.FolderName.FILE_MANIFEST;
            string osFolderName = AssetsCommon.GetPlatformFolderName(osType);

            List<string> paths = new List<string>()
        {
  PathConstant.SERVER_FOLDER_NAME,
            assetFolderName,
            osFolderName
        };
            string path = AssetsCommon.BuildPath(assetPath, paths);
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);

            return path;
        }

        // 例如:AssetBundlerServer/FileManifest/Win/version_1/vesion.txt
        public static string GetFileManifestPath(ABPathInfo pathInfo)
        {
            string assetPath = AssetsCommon.GetAssetPath();
            string assetFolderName = PathConstant.FolderName.FILE_MANIFEST;
            string osFolderName = AssetsCommon.GetPlatformFolderName(pathInfo.OsType);
            string versionFolderName = PathConstant.FolderName.GetVersion(pathInfo.Version);

            if (osFolderName == string.Empty)
            {
                Debug.LogError("平台文件夹名称获取错误:" + pathInfo.OsType);
                return string.Empty;
            }

            List<string> paths = new List<string>()
        {
  PathConstant.SERVER_FOLDER_NAME,
            assetFolderName,
            osFolderName,
            versionFolderName
        };

            string path = AssetsCommon.BuildPath(assetPath, paths);
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);

            return path;
        }

        public static string GetABAssetInfoFileName(ABPathInfo pathInfo)
        {
            string assetPath = GetFileManifestPath(pathInfo);
            return AssetsCommon.BuildFileName(assetPath, PathConstant.FileName.AB_ASSET_INFO);
        }

        public static string GetVersionFileName(ABPathInfo pathInfo)
        {
            string assetPath = GetFileManifestPath(pathInfo);
            return AssetsCommon.BuildFileName(assetPath, PathConstant.FileName.VERSION);
        }

        public static string GetVersionListFileName(ePlatformType osType)
        {
            string assetPath = GetVersionListPath(osType);
            return AssetsCommon.BuildFileName(assetPath, PathConstant.FileName.VERSION_LIST);
        }

        public static string GetFenBaoFileName(ABPathInfo pathInfo)
        {
            string assetPath = GetFileManifestPath(pathInfo);
            return AssetsCommon.BuildFileName(assetPath, PathConstant.FileName.FEN_BAO);
        }
    }

    public static class FileManifestURL
    {
        public static string GetFileManifestURL(ABPathInfo pathInfo)
        {
            string assetFolderName = PathConstant.FolderName.FILE_MANIFEST;
            string osFolderName = AssetsCommon.GetPlatformFolderName(pathInfo.OsType);
            string versionFolderName = PathConstant.FolderName.GetVersion(pathInfo.Version);

            List<string> paths = new List<string>()
        {
            assetFolderName,
            osFolderName,
            versionFolderName
        };

            return AssetsCommon.BuildPath(URL, paths);
        }


        public static string GetABAssetInfoURL(ABPathInfo pathInfo)
        {
            string url = GetFileManifestURL(pathInfo);
            return AssetsCommon.BuildFileName(url, PathConstant.FileName.AB_ASSET_INFO);
        }

        public static string GetVersionURL(ABPathInfo pathInfo)
        {
            string url = GetFileManifestURL(pathInfo);
            return AssetsCommon.BuildFileName(url, PathConstant.FileName.VERSION);
        }

        public static string GetVersionListURL(ePlatformType osType)
        {
            string assetFolderName = PathConstant.FolderName.FILE_MANIFEST;
            string osFolderName = AssetsCommon.GetPlatformFolderName(osType);

            List<string> paths = new List<string>()
        {
            assetFolderName,
            osFolderName
        };
            string url = AssetsCommon.BuildPath(URL, paths);
            return AssetsCommon.BuildFileName(url, PathConstant.FileName.VERSION_LIST);
        }

        public static string GetFenBaoURL(ABPathInfo pathInfo)
        {
            string url = GetFileManifestURL(pathInfo);
            return AssetsCommon.BuildFileName(url, PathConstant.FileName.FEN_BAO);
        }


    }

    public static class FenBaoURL
    {
        public static string GetFenBaoURL(ABPathInfo pathInfo)
        {
            string assetFolderName = PathConstant.FolderName.FEN_BAO;
            string osFolderName = AssetsCommon.GetPlatformFolderName(pathInfo.OsType);
            string versionFolderName = PathConstant.FolderName.GetFenBaoVersion(pathInfo.Version);

            List<string> paths = new List<string>()
        {
            assetFolderName,
            osFolderName,
            versionFolderName
        };

            return AssetsCommon.BuildPath(URL, paths);
        }

        public static string GetPackageFileName(
            ABPathInfo pathInfo, string fileName)
        {
            string url = GetFenBaoURL(pathInfo);
            return AssetsCommon.BuildFileName(url, fileName);
        }

    }

    public static class ABCacheURL
    {
        public static string GetABCacheURL(ePlatformType osType)
        {
            string assetFolderName = PathConstant.FolderName.AB_CACHE;
            string osFolderName = AssetsCommon.GetPlatformFolderName(osType);

            List<string> paths = new List<string>()
            {
               assetFolderName,
               osFolderName
            };
            return AssetsCommon.BuildPath(URL, paths);
        }

        public static string GetAssetBundleFileName(
            ePlatformType osType, string fileName)
        {
            string url = GetABCacheURL(osType);
            return AssetsCommon.BuildFileName(url, fileName);
        }

    }

}
