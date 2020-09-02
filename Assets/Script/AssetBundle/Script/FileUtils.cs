/****************************************************
	文件：FileUtils.cs
	作者：Lonely
	github：https://github.com/AIValkyries/AssetBundleUpdate
	日期：2020/07/02 21:08:48
	功能：Nothing
*****************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Security.Cryptography;
using System.IO;

public class FileUtils
{

    public static string GetBytesMD5(byte[] bytes)
    {
        string strmd5 = "";
        MD5 md5 = MD5.Create();
        byte[] md5Bytes = md5.ComputeHash(bytes);

        md5.Clear();

        for (int i = 0; i < md5Bytes.Length; i++)
        {
            strmd5 += md5Bytes[i].ToString("x").PadLeft(2, '0');
        }

        return strmd5;
    }

    public static string GetFileMD5ByStream(FileStream stream)
    {
        string strmd5 = "";
        byte[] bytes = new byte[stream.Length];
        stream.Read(bytes, 0, bytes.Length);
        MD5 md5 = MD5.Create();
        byte[] bMd5 = md5.ComputeHash(bytes);

        md5.Clear();

        for (int i = 0; i < bMd5.Length; i++)
        {
            strmd5 += bMd5[i].ToString("x").PadLeft(2, '0');
        }
        return strmd5;
    }

    public static string GetFileMD5(string fileName, ref long byteSize)
    {
        if (!File.Exists(fileName))
            return string.Empty;

        string strmd5 = "";
        using (FileStream stream = new FileStream(fileName, FileMode.Open))
        {
            byteSize = stream.Length;
            byte[] bytes = new byte[stream.Length];
            stream.Read(bytes, 0, bytes.Length);

            MD5 md5 = MD5.Create();
            byte[] bMd5 = md5.ComputeHash(bytes);
           
            md5.Clear();

            for (int i = 0; i < bMd5.Length; i++) 
            {
                strmd5 += bMd5[i].ToString("x").PadLeft(2, '0');
            }
            stream.Close();
            stream.Dispose();
        }

        return strmd5;
    }

}
