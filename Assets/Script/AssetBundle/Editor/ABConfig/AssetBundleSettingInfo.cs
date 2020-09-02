using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System.Xml;
using System.Text;
using System.IO;

public class AssetBundleSettingInfo
{
    public static string SETTING_FILE_PATH = "Assets/Script/AssetBundle/Editor/ABConfig/FileData/";
    public static string WIN_FILE_NAME = "AssetBundleSettingInfo_Win.xml";
    public static string ANDROID_FILE_NAME = "AssetBundleSettingInfo_Android.xml";
    public static string IOS_FILE_NAME = "AssetBundleSettingInfo_iOS.xml";

    public string Version;
    public string OutPath;
    public BuildAssetBundleOptions Options;
    public bool ResetAssetBundleName;
    public List<string> UnpackPath;    // 打包路径
    public List<string> IgnoreSuffix;  // 忽略后缀名
    public Dictionary<string, string> AssetsMap;

    public AssetBundleSettingInfo()
    {
        UnpackPath = new List<string>();
        IgnoreSuffix = new List<string>();
        AssetsMap = new Dictionary<string, string>();
    }

    string GetXmlName()
    {
        string assetName = string.Empty;
        if (UnpackCommon.Target == BuildTarget.StandaloneWindows)
        {
            assetName = SETTING_FILE_PATH + WIN_FILE_NAME;
        }
        else if (UnpackCommon.Target == BuildTarget.Android)
        {
            assetName = SETTING_FILE_PATH + ANDROID_FILE_NAME;
        }
        else if (UnpackCommon.Target == BuildTarget.iOS)
        {
            assetName = SETTING_FILE_PATH + IOS_FILE_NAME;
        }
        return assetName;
    }

#if UNITY_EDITOR

    public void SaveToFile()
    {
        XmlDocument xmldoc = new XmlDocument();
        XmlDeclaration declaration = xmldoc.CreateXmlDeclaration("1.0", "utf-8", string.Empty);
        xmldoc.AppendChild(declaration);

        XmlElement root = xmldoc.CreateElement("SettingInfo");
        xmldoc.AppendChild(root);

        XmlElement version = xmldoc.CreateElement("Version");
        version.InnerText = Version;
        root.AppendChild(version);

        XmlElement outPath = xmldoc.CreateElement("OutPath");
        outPath.InnerText = OutPath;
        root.AppendChild(outPath);

        XmlElement options = xmldoc.CreateElement("Options");
        options.InnerText = Options.ToString();
        root.AppendChild(options);

        XmlElement resetAbName = xmldoc.CreateElement("ResetAssetBundleName");
        resetAbName.InnerText = (ResetAssetBundleName == true ? 1 : 0).ToString();
        root.AppendChild(resetAbName);

        StringBuilder UnpackPathsb = GetStringBuilder(UnpackPath);
        StringBuilder IgnoreSuffixsb = GetStringBuilder(IgnoreSuffix);
        StringBuilder AssetsMapsb = GetAssetsMap(AssetsMap);

        XmlElement unpackPath = xmldoc.CreateElement("UnpackPath");
        unpackPath.InnerText = UnpackPathsb.ToString();
        root.AppendChild(unpackPath);

        XmlElement ignoreSuffix = xmldoc.CreateElement("IgnoreSuffix");
        ignoreSuffix.InnerText = IgnoreSuffixsb.ToString();
        root.AppendChild(ignoreSuffix);

        XmlElement assetsMap = xmldoc.CreateElement("AssetsMap");
        assetsMap.InnerText = AssetsMapsb.ToString();
        root.AppendChild(assetsMap);

        // 将AssetsMap写入到 L_Resources 目录下的文件中，
        // 在加载ab时有用到
        StringBuilder @string = new StringBuilder();
        var itr = AssetsMap.Keys.GetEnumerator();
        while (itr.MoveNext())
        {
            @string.Append(itr.Current + "|" + AssetsMap[itr.Current] + "\n");
        }
        itr.Dispose();

        string filePath = Application.dataPath + "/L_Resources/";
        if (!Directory.Exists(filePath))
            Directory.CreateDirectory(filePath);

        string fileName = filePath + PathConstant.FileName.ASSETS_MAP_NAME;
        if (File.Exists(fileName))
            File.Delete(fileName);

        using (FileStream fs = File.Create(fileName))
        {
            byte[] bytes = System.Text.Encoding.Default.GetBytes(@string.ToString());
            fs.Write(bytes, 0, bytes.Length);
            fs.Close();
        }

        string xmlName = GetXmlName();
        if (!System.IO.File.Exists(xmlName))
        {
            using (FileStream fs = System.IO.File.Create(xmlName))
                fs.Close();
        }

        xmldoc.Save(GetXmlName());

        AssetDatabase.Refresh();
    }

    public void LoadToFile()
    {
        string xmlName = GetXmlName();
        if (!System.IO.File.Exists(xmlName))
            return;

        XmlDocument xmldoc = new XmlDocument();
        xmldoc.Load(xmlName);
        XmlNodeList root = xmldoc.GetElementsByTagName("SettingInfo");

        for (int i = 0; i < root.Count; i++)
        {
            XmlNode nodes = root[i];

            foreach (XmlNode n in nodes.ChildNodes)
            {
                XmlElement element = n as XmlElement;

                if (element.Name.Equals("Version"))
                {
                    Version = element.InnerText;
                }
                else if (element.Name.Equals("OutPath"))
                {
                    OutPath = element.InnerText;
                }
                else if (element.Name.Equals("Options"))
                {
                    Options = (BuildAssetBundleOptions)System.Enum.Parse(typeof(BuildAssetBundleOptions), element.InnerText);
                }
                else if (element.Name.Equals("ResetAssetBundleName"))
                {
                    ResetAssetBundleName = int.Parse(element.InnerText) == 1 ? true : false;
                }
                else if (element.Name.Equals("UnpackPath"))
                {
                    string unpackPath = element.InnerText;
                    string[] paths = unpackPath.Split(';');
                    if (paths == null)
                        continue;

                    for (int j = 0; j < paths.Length; j++)
                    {
                        UnpackPath.Add(paths[j]);
                    }
                }
                else if (element.Name.Equals("IgnoreSuffix"))
                {
                    string ignoreSuffix = element.InnerText;
                    string[] paths = ignoreSuffix.Split(';');
                    if (paths == null)
                        continue;

                    for (int j = 0; j < paths.Length; j++)
                    {
                        IgnoreSuffix.Add(paths[j]);
                    }
                }
                else if (element.Name.Equals("AssetsMap"))
                {
                    string assetsMap = element.InnerText;
                    string[] elStr = assetsMap.Split(';');

                    for (int j = 0; j < elStr.Length; j++)
                    {
                        string[] group = elStr[j].Split(',');
                        if (group == null || group.Length != 2 ||
                            group[0] == null || group[1] == null)
                            continue;
                        if (!AssetsMap.ContainsKey(group[0]))
                            AssetsMap.Add(group[0], group[1]);
                    }

                }

            }
        }
    }


    StringBuilder GetAssetsMap(Dictionary<string,string> src)
    {
        StringBuilder builder = new StringBuilder();

        int count = 0;
        var itr = src.Keys.GetEnumerator();
        while (itr.MoveNext())
        {
            if (count == src.Count - 1)
                builder.Append(itr.Current + "," + src[itr.Current]);
            else
                builder.Append(itr.Current + "," + src[itr.Current] + ";");

            count++;
        }
        itr.Dispose();

        return builder;
    }

    StringBuilder GetStringBuilder(List<string> src)
    {
        StringBuilder sb = new StringBuilder();
        for (int i = 0; i < src.Count; i++)
        {
            if (i == src.Count - 1)
                sb.Append(src[i]);
            else
                sb.Append(src[i] + ";");
        }

        return sb;
    }

#endif


}
