/****************************************************
	文件：FileManifestBase.cs
	作者：Lonely
	github：https://github.com/AIValkyries/AssetBundleUpdate
	日期：2020/08/11 22:27:03
	功能：Nothing
*****************************************************/

using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

public abstract class FileManifestBase
{
    protected Dictionary<string, AssetDataInfo> localInfos;
    protected string fileName;

    protected abstract string GetLocalAssetInfoFileName();

    public FileManifestBase()
    {
        localInfos = new Dictionary<string, AssetDataInfo>();
        fileName = GetLocalAssetInfoFileName();
    }

    public void Read()
    {
        if (!File.Exists(fileName))
        {
            FileStream fs = File.Create(fileName);
            fs.Dispose();
            fs.Close();
        }
        else
        {
            using (FileStream fs = File.Open(fileName, FileMode.Open))
            {
                byte[] bytes = new byte[fs.Length];
                fs.Read(bytes, 0, bytes.Length);

                string context = Encoding.Default.GetString(bytes);
                if (context == string.Empty)
                {
                    fs.Close();
                    return;
                }

                string[] abInfos = context.Split('\n');

                for (int i = 0; i < abInfos.Length; i++)
                {
                    if (abInfos[i] == string.Empty)
                        continue;

                    string[] info = abInfos[i].Split('|');

                    AssetDataInfo assetInfo = new AssetDataInfo();
                    assetInfo.Name = info[0];
                    assetInfo.MD5 = info[1];
                    assetInfo.Size = System.Convert.ToInt32(info[2]);
                    assetInfo.IsCompressed = info[3] == "True" ? true : false;

                    Update(assetInfo);
                }
            }
        }
    }

    // 写入最新版本
    public virtual void WriteUpToDate() { }

    public void WriteCurrent()
    {
        if (localInfos == null || localInfos.Count <= 0)
            return;
        WriteToLocal(localInfos);
    }

    protected void WriteToLocal(
        Dictionary<string, AssetDataInfo> infos, bool deleteFile = false)
    {
        FileStream fs = null;

        if (deleteFile)
        {
            if (File.Exists(fileName))
                File.Delete(fileName);
        }

        if (!File.Exists(fileName))
        {
            fs = File.Create(fileName);
        }
        else
        {
            fs = File.Open(fileName, FileMode.Open);
        }

        StringBuilder sb = new StringBuilder();

        var itr = infos.Values.GetEnumerator();
        int index = 0;
        while (itr.MoveNext())
        {
            AssetDataInfo info = itr.Current;

            if (index == infos.Count - 1)
                sb.Append(info.ToString());
            else
                sb.Append(info.ToString() + "\n");

            ++index;
        }

        itr.Dispose();

        byte[] bytes = Encoding.Default.GetBytes(sb.ToString());

        fs.Write(bytes, 0, bytes.Length);
        fs.Dispose();
        fs.Close();
    }

    public void Remove(AssetDataInfo oldInfo)
    {
        if (localInfos.ContainsKey(oldInfo.Name))
            localInfos.Remove(oldInfo.Name);
    }

    public void Update(AssetDataInfo newInfo)
    {
        if (localInfos.ContainsKey(newInfo.Name))
        {
            localInfos[newInfo.Name] = newInfo;
        }
        else
        {
            localInfos.Add(newInfo.Name, newInfo);
        }
    }

    public bool IsDownloaded(AssetDataInfo newInfo)
    {
        AssetDataInfo outInfo;

        if (localInfos.TryGetValue(newInfo.Name, out outInfo))
        {
            if (outInfo.MD5 == newInfo.MD5 && 
                outInfo.Size == newInfo.Size)
                return true;
        }

        return false;
    }



}
