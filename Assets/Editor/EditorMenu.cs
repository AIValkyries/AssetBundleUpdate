using ICSharpCode.SharpZipLib.Zip;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;

public class EditorMenu : MonoBehaviour
{
    #region 场景菜单

    [MenuItem("Assets/将当前场景物体写入场景文件")]
    public static void WillCurrentSceneObjectWriteSceneFileByAssets()
    {
        DynamicScene.GenerateSceneFile(UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene());
    }

    [MenuItem("Assets/将所有场景物体写入场景文件")]
    public static void WillAllSceneObjectWriteSceneFileByAssets()
    {
        DynamicScene.GenerateAllSceneFile();
    }

    [MenuItem("Assets/当前场景复原")]
    public static void RecoverySceneByConfig()
    {
        DynamicScene.RecoveryScene();
    }

    [MenuItem("Assets/清空当前场景")]
    public static void ClearScene()
    {
        DynamicScene.ClearScene();
    }

    #endregion

    [MenuItem("AssetBundle/CompressTest")]
    public static void CompressTest()
    {
        string packPath = "E:/myProject/LonelyFramework/Assets/PersistentAssets/dataconfig";

        long size = 0;
        string md51 = FileUtils.GetFileMD5(packPath, ref size);

        string localPath = AssetsCommon.LocalAssetPath;

        string zipFileName = string.Format("{0}/{1}",
            localPath,
            "Package1_0.zip");
        List<AssetDataInfo> assetNames = ZipHelper.Decompress(
            zipFileName,
            localPath);

        Debug.Log(md51);

        Debug.Log("压缩成功!");
    }

    [MenuItem("AssetBundle/DecompressTest")]
    public static void DecompressTest()
    {
        string targetPath = "E:/myProject/LonelyFramework/AssetBundleServer/FenBao/Win/version_1/";
        string zipFileName = "E:/myProject/LonelyFramework/AssetBundleServer/FenBao/Win/version_1/Package1_11111.zip";

        UpdateStageResult updateStageResult = new UpdateStageResult();

        DownLoadFileResultInfo info = new DownLoadFileResultInfo();

        List<AssetDataInfo> list = new List<AssetDataInfo>();

        ZipHelper.Decompress(
             zipFileName,
             targetPath);

        Debug.Log("解压成功!");
    }


    [MenuItem("AssetBundle/Test")]
    public static void Test()
    {
        AssetBundle.UnloadAllAssetBundles(true);
        string path = Application.dataPath +
            "/StreamingAssets/Win/Win";
        AssetBundle bundle = AssetBundle.LoadFromFile(path);
        AssetBundleManifest manifest = bundle.LoadAsset
            <AssetBundleManifest>("AssetBundleManifest");

        string[] assetBundlePaths = manifest.GetAllDependencies("scenes.samplescene");

        for (int i = 0; i < assetBundlePaths.Length; i++)
        {
            Debug.Log(assetBundlePaths[i]);
        }

        bundle.Unload(true);
    }

    #region ab 打包菜单
    [MenuItem("AssetBundle/打包设置GUI")]
    public static void SettingUnpackGUI()
    {
        // 设置ab压缩,
        // 忽略路径
        // ab输出路径
        // 版本号
        var win = EditorWindow.GetWindow<AssetBundleEditorSettingGUI>("ABSettings", true);
        if (win != null)
            win.Show();
    }

    [MenuItem("AssetBundle/BuildWin")]
    public static void BuildWin()
    {
        UnpackCommon.Target = BuildTarget.StandaloneWindows;
        AssetBundleUnpack.BuildAssetBundle(BuildTarget.StandaloneWindows);
    }

    [MenuItem("AssetBundle/BuildAndroid")]
    public static void BuildAndroid()
    {
        UnpackCommon.Target = BuildTarget.Android;
        AssetBundleUnpack.BuildAssetBundle(BuildTarget.Android);
    }

    [MenuItem("AssetBundle/BuildIOS")]
    public static void BuildIOS()
    {
        UnpackCommon.Target = BuildTarget.iOS;
        AssetBundleUnpack.BuildAssetBundle(BuildTarget.iOS);
    }

    [MenuItem("AssetBundle/分包Win")]
    public static void FenBaoWin()
    {
        UnpackCommon.Target = BuildTarget.StandaloneWindows;
        SubAssetPackage.Subpackage();
    }

    [MenuItem("AssetBundle/分包安卓")]
    public static void FenBaoAndroid()
    {
        UnpackCommon.Target = BuildTarget.Android;
        SubAssetPackage.Subpackage();
    }

    [MenuItem("AssetBundle/分包IOS")]
    public static void FenBaoIOS()
    {
        UnpackCommon.Target = BuildTarget.iOS;
        SubAssetPackage.Subpackage();
    }

    #endregion






}
