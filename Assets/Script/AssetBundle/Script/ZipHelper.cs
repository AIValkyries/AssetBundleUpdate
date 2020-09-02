/****************************************************
	文件：ZipHelper.cs
	作者：Lonely
	github：https://github.com/AIValkyries/AssetBundleUpdate
	日期：2020/08/12 21:06:43
	功能：Nothing
*****************************************************/

using ICSharpCode.SharpZipLib.Checksums;
using ICSharpCode.SharpZipLib.Zip;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public static class ZipHelper
{

    public static void Compress(List<string> assets, string packPath, string sourcePath)
    {
        Dictionary<string, FileStream> assetFiles = new Dictionary<string, FileStream>();

        for (int i = 0; i < assets.Count; i++)
        {
            string fileFullName = sourcePath + "/" + assets[i];
            FileStream info = File.Open(fileFullName, FileMode.Open);
            assetFiles.Add(assets[i], info);
        }

        using (ZipOutputStream outStream = new ZipOutputStream(File.Create(packPath)))
        {
            outStream.SetLevel(0);
            Crc32 crc = new Crc32();

            List<byte[]> zipBuffers = new List<byte[]>();

            var itr = assetFiles.Keys.GetEnumerator();
            long index = 0;
            int totalSize = 0;

            while (itr.MoveNext())
            {
                FileStream fs = assetFiles[itr.Current];
                byte[] buffer = new byte[fs.Length];
                fs.Read(buffer, 0, buffer.Length);

                totalSize += (int)fs.Length;
                zipBuffers.Add(buffer);

                ZipEntry entry = new ZipEntry(itr.Current);
                entry.DateTime = File.GetLastAccessTime(itr.Current);
                entry.Size = buffer.Length;
                entry.ZipFileIndex = assetFiles.Count - index++;
                fs.Close();
                crc.Reset();
                crc.Update(buffer);
                entry.Crc = crc.Value;
                outStream.PutNextEntry(entry);
                outStream.Write(buffer, 0, buffer.Length);
            }

            outStream.Close();
            itr.Dispose();
        }

    }

    public static List<AssetDataInfo> Decompress(
        string zipFileName,
        string targetPath)
    {
        List<AssetDataInfo> assetNames = new List<AssetDataInfo>();

        ZipEntry theEntry = null;
        int size = 0;
        byte[] bytes = new byte[2048];

        using (ZipInputStream s = new ZipInputStream(File.OpenRead(zipFileName)))
        {
            while ((theEntry = s.GetNextEntry()) != null)
            {
                if (theEntry.Name == string.Empty)
                    continue;

                string fileName = (targetPath + "/" + theEntry.Name);

                if (File.Exists(fileName))
                    File.Delete(fileName);
                AssetDataInfo info = new AssetDataInfo();
                info.Name = theEntry.Name;

                using (FileStream fs = File.Create(fileName))
                {
                    while (true)
                    {
                        size = s.Read(bytes, 0, bytes.Length);
                        if (size <= 0)
                            break;
                        fs.Write(bytes, 0, size);
                        UpdateStageResult.Compression.CurrentSize += size;
                    }

                    fs.Close();
                }

                using (FileStream fs = File.Open(fileName, FileMode.Open))
                {
                    info.Size = (int)fs.Length;
                    info.MD5 = FileUtils.GetFileMD5ByStream(fs);
                    info.IsCompressed = true;

                    fs.Close();
                }

                assetNames.Add(info);
            }
            s.Close();
        }

        return assetNames;
    }


}
