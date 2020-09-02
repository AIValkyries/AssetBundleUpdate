using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using System.IO;

public class FenBaoManifestEditor
{
    static FenBaoManifestEditor _fen_bao_editor_manifest;
    public static FenBaoManifestEditor Current
    {
        get
        {
            if (_fen_bao_editor_manifest == null) 
                _fen_bao_editor_manifest = new FenBaoManifestEditor();
            return _fen_bao_editor_manifest;
        }
    }

    public void Write(List<AssetDataInfo> fenBaos)
    {
        // 将分包信息写入文件
        ABPathInfo pathInfo = new ABPathInfo(UnpackCommon.GetOsType(), UnpackCommon.GetVersion());
        string fileName = AssetBundleServerPath.FileManifest.GetFenBaoFileName(pathInfo);
        // 资源路径
        // 平台
        // 版本号

        StringBuilder str = new StringBuilder();

        for (int i = 0; i < fenBaos.Count; i++) 
        {
            if (i == fenBaos.Count - 1)
            {
                str.Append(fenBaos[i].ToString());
            }
            else
            {
                str.Append(fenBaos[i].ToString() + "\n");
            }
        }


        if (File.Exists(fileName))
            File.Delete(fileName);

        FileStream fs = File.Create(fileName);

        byte[] bytes = System.Text.Encoding.Default.GetBytes(str.ToString());
        fs.Write(bytes, 0, bytes.Length);
        fs.Dispose();
        fs.Close();
    }
}
