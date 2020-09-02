using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class AddScriptInfo : UnityEditor.AssetModificationProcessor
{
    private static string fileDescribe =
       "/****************************************************\n" +
        "\t文件：#SCRIPTNAME#\n" +
        "\t作者：#CreateAuthor#\n" +
        "\tgithub：https://github.com/AIValkyries/AssetBundleUpdate \n" +
        "\t日期：#CreateTime#\n" +
         "\t功能：Nothing\n" +
       "*****************************************************/\n\n";

    /// <summary>
    /// int 好无意义，纯粹为了方便遍历
    /// </summary>
    static Dictionary<string, int> ignorePath = new Dictionary<string, int>()
    {
        { "Plugins",0}
    };


    static bool IsIgnorePath(string[] iterm)
    {
        for (int i = 0; i < iterm.Length; i++) 
        {
            if (ignorePath.ContainsKey(iterm[i]))
                return true;
        }

        return false;
    }

    private static void OnWillCreateAsset(string path)
    {
        path = path.Replace(".meta","");

        if (path.EndsWith(".cs") == true) 
        {
            // 文件名的分割获取
            string[] iterm = path.Split('/');
            if (IsIgnorePath(iterm))
                return;
            
            string str = fileDescribe;

            //读取该路径下的.cs文件中的所有文本.
            //注意，此时Unity已经对脚本完成了模版内容的替换，包括#SCRIPTNAME#也已经被替换为文件名了，读取到的是替换后的文本内容
            str += File.ReadAllText(path);

            // 进行关键字的文件名、作者和时间获取，并替换
            str = str.Replace("#SCRIPTNAME#", iterm[iterm.Length - 1]).Replace(
                "#CreateAuthor#", "Lonely").Replace(
                "#CreateTime#", string.Format("{0:0000}/{1:00}/{2:00} {3:00}:{4:00}:{5:00}", DateTime.Now.Year,
                DateTime.Now.Month, DateTime.Now.Day, DateTime.Now.Hour, DateTime.Now.Minute,
                DateTime.Now.Second));

            // 重新写入脚本中，完成数据修改
            File.WriteAllText(path, str);
        }

    }




}
