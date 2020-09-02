using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class AssetBundleEditorSettingGUI : EditorWindow
{
    const string EXPLANATION = "AB打包默认为散包(一个文件一个.ab),但是你可以" +
        "通过'设置AssetBundleName'(文件夹:包名)，将某个文件夹下的" +
        "所有文件打成一个包";

    AssetBundleSettingInfo _setting_info;
    List<string> _asset_bundle_name_path;
    List<string> _asset_bundle_names;

    Vector2 _assetBundleMapRect;
    Vector2 _unpackPathRect;

    private void OnEnable()
    {
        _asset_bundle_name_path = new List<string>();
        _asset_bundle_names = new List<string>();

#if UNITY_STANDALONE_WIN
       UnpackCommon.Target = BuildTarget.StandaloneWindows;
#endif

#if UNITY_ANDROID
        AssetBundleCommon.Target = BuildTarget.Android;
#endif

#if UNITY_IPHONE
      AssetBundleCommon.Target = BuildTarget.iOS;
#endif

        _setting_info = UnpackCommon.GetSettingInfo();
        _asset_bundle_name_path.AddRange(_setting_info.AssetsMap.Keys);
        _asset_bundle_names.AddRange(_setting_info.AssetsMap.Values);

        _setting_info.AssetsMap.Clear();
    }

    void OnInspectorUpdate()
    {
        Repaint();
    }

    private void OnDestroy()
    {
        _setting_info = null;
        UnpackCommon.ClearAssetBundleSettingInfo();
        _asset_bundle_name_path = null;
        _asset_bundle_names = null;
    }

    private void OnGUI()
    {
        CopySettingData();
        SetVersion();
        SetAssetBundleOptions();
        SetResetAssetBundleName();
        SetIgnoreSuffix();
        SetUnpackPath();
        SetAssetMap();
        SetOutPath();
        SaveSetting();
    }

    void CopySettingData()
    {
        GUILayout.BeginHorizontal();

        GUILayout.Label("CopyWindows平台的设置数据");
        if (GUILayout.Button("Copy"))
        {
            CopyData();
        }

        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        GUILayout.Label("Copy安卓平台的设置数据");
        if (GUILayout.Button("Copy"))
        {
            CopyData();
        }

        GUILayout.EndHorizontal();


        GUILayout.BeginHorizontal();
        GUILayout.Label("CopyIOS平台的设置数据");
        if (GUILayout.Button("Copy"))
        {
            CopyData();
        }

        GUILayout.EndHorizontal();

        GUILayout.Space(10);
    }

    void CopyData()
    {
        AssetBundleSettingInfo info = new AssetBundleSettingInfo();
        info.LoadToFile();
        _setting_info = info;

        _asset_bundle_name_path.AddRange(_setting_info.AssetsMap.Keys);
        _asset_bundle_names.AddRange(_setting_info.AssetsMap.Values);
        _setting_info.AssetsMap.Clear();
    }

    void SetIgnoreSuffix()
    {
        GUILayout.BeginHorizontal();
        GUILayout.Label("添加打包时忽略后缀文件:", GUILayout.MaxWidth(150));
        if (GUILayout.Button("添加", GUILayout.MaxWidth(50)))
        {
            _setting_info.IgnoreSuffix.Add("");
        }
        GUILayout.EndHorizontal();

        for (int i = 0; i < _setting_info.IgnoreSuffix.Count; i++)
        {
            GUILayout.BeginHorizontal();
            _setting_info.IgnoreSuffix[i] = EditorGUILayout.TextField(_setting_info.IgnoreSuffix[i]);
            if (GUILayout.Button("X", GUILayout.MaxWidth(20)))
                _setting_info.IgnoreSuffix.Remove(_setting_info.IgnoreSuffix[i]);

            GUILayout.EndHorizontal();
        }
        GUILayout.Space(10);
    }

    void SetUnpackPath()
    {
        _unpackPathRect = GUILayout.BeginScrollView(_unpackPathRect);
        GUILayout.BeginHorizontal();
        GUILayout.Label("添加打包路径:", GUILayout.MaxWidth(100));
        if (GUILayout.Button("添加", GUILayout.MaxWidth(50)))
        {
            _setting_info.UnpackPath.Add("");
        }
        GUILayout.EndHorizontal();

        for (int i = 0; i < _setting_info.UnpackPath.Count; i++)
        {
            GUILayout.BeginHorizontal();
            _setting_info.UnpackPath[i] = EditorGUILayout.TextField(_setting_info.UnpackPath[i],GUILayout.MaxWidth(300));

            if (GUILayout.Button("X", GUILayout.MaxWidth(20)))
                _setting_info.UnpackPath.Remove(_setting_info.UnpackPath[i]);

            GUILayout.EndHorizontal();
        }
        GUILayout.EndScrollView();
        GUILayout.Space(10);
    }

    void SetAssetMap()
    {
        _assetBundleMapRect = GUILayout.BeginScrollView(_assetBundleMapRect);
        GUILayout.BeginHorizontal();
        GUILayout.Label("设置AssetBundleName(不会自动遍历子文件夹):", GUILayout.MaxWidth(250));
        if (GUILayout.Button("添加", GUILayout.MaxWidth(50)))
        {
            _asset_bundle_name_path.Add(string.Empty);
            _asset_bundle_names.Add(string.Empty);
        }
        GUILayout.EndHorizontal();

        for (int i = 0; i < _asset_bundle_name_path.Count; i++)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label("AssetBundle路径", GUILayout.MaxWidth(320));
            GUILayout.Label("AssetBundle名称", GUILayout.MaxWidth(320));
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            _asset_bundle_name_path[i] = EditorGUILayout.TextField(_asset_bundle_name_path[i], GUILayout.MaxWidth(300));
            _asset_bundle_names[i] = EditorGUILayout.TextField(_asset_bundle_names[i], GUILayout.MaxWidth(300));

            if (GUILayout.Button("X", GUILayout.MaxWidth(20)))
            {
                _asset_bundle_name_path.RemoveAt(i);
                _asset_bundle_names.RemoveAt(i);
            }
            GUILayout.EndHorizontal();
        }
        GUILayout.EndScrollView();
        GUILayout.Space(10);
    }

    void SetOutPath()
    {
        GUILayout.BeginHorizontal();
        GUILayout.Label("输出相对路径:", GUILayout.MaxWidth(200));
        _setting_info.OutPath = EditorGUILayout.TextField(_setting_info.OutPath);

        GUILayout.EndHorizontal();
    }

    void SetAssetBundleOptions()
    {
        GUILayout.BeginHorizontal();
        GUILayout.Label("Options选择:", GUILayout.MaxWidth(200));
        _setting_info.Options = (BuildAssetBundleOptions)EditorGUILayout.EnumPopup(_setting_info.Options);
        GUILayout.EndHorizontal();
    }

    void SetResetAssetBundleName()
    {
        GUILayout.BeginHorizontal();
        GUILayout.Label("是否重置AssetBundleName:", GUILayout.MaxWidth(200));
        _setting_info.ResetAssetBundleName = EditorGUILayout.Toggle(_setting_info.ResetAssetBundleName);
        GUILayout.EndHorizontal();
        GUILayout.Space(10);
    }

    void SetVersion()
    {
        GUILayout.BeginHorizontal();
        GUILayout.Label("版本号(第一个数字为分包版本号，第二个为资源版本号,只要更新就会递增):", GUILayout.MaxWidth(400));
        _setting_info.Version = EditorGUILayout.TextField(_setting_info.Version);
        GUILayout.EndHorizontal();
    }


    void SaveSetting()
    {
        GUILayout.BeginHorizontal();
        if (GUILayout.Button("保存设置"))
        {
            for (int i = 0; i < _asset_bundle_name_path.Count; i++) 
            {
                _setting_info.AssetsMap.Add(_asset_bundle_name_path[i], _asset_bundle_names[i]);
            }
            _setting_info.SaveToFile();
        }
        GUILayout.EndHorizontal();
    }





}
