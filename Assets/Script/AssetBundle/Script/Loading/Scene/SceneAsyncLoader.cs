/****************************************************
	文件：SceneAsyncLoader.cs
	作者：Lonely
	github：https://github.com/AIValkyries/AssetBundleUpdate
	日期：2020/08/02 20:32:12
	功能：Nothing
*****************************************************/

using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneAsyncLoader
{
    static Dictionary<string, bool> _uninstallable_scene = new Dictionary<string, bool>();

    public bool IsDone { get; private set; }
    public float Progress { get; private set; }

    public System.Action<SceneAsyncLoader> loadCompleted;


    string _asset_bundle_name;
    string _scene_name;
    string _xml_asset_bundle_name;
    LoadSceneMode _load_mode;

    int _current;
    int _totalCount;

    public SceneAsyncLoader(
        string assetBundleName, string sceneName,string fileName,
        LoadSceneMode loadMode)
    {
        _asset_bundle_name = assetBundleName;
        _scene_name = sceneName;
        _xml_asset_bundle_name = fileName;
        _load_mode = loadMode;
    }

    public SceneAsyncLoader(
        string assetBundleName,
        string sceneName,
        string fileName) :
        this(assetBundleName, sceneName, fileName, LoadSceneMode.Single)
    {

    }

    void UpdateProgress()
    {
        Progress = (float)(_current++) / _totalCount;
    }

    IEnumerator LoadDependency(string assetBundleName, AssetBundleLoad load)
    {
        List<string> dependencys = load.GetDependencys(assetBundleName);
        if (dependencys == null)
            yield break;
        for (int i = 0; i < dependencys.Count; i++)
        {
            AssetBundleLoaderRequest request = AssetBundleLoaderRequest.New(dependencys[i]);
            load.LoadAssetBundleAsync(request);
            while (!request.IsDone)
                yield return null;
            UpdateProgress();
        }
    }

    // 计算依赖数量长度
    void CalculationDependencyCount(string assetBundleName, AssetBundleLoad load)
    {
        List<string> dependencys = load.GetDependencys(assetBundleName);
        if (dependencys == null)
            return;

        _totalCount +=  dependencys.Count;

        for (int i = 0; i < dependencys.Count; i++) 
        {
            CalculationDependencyCount(dependencys[i],load);
        }
    }

    Dictionary<XmlNode, XmlNodeList> GetSceneObjects(TextAsset xmlInfo)
    {
        if (xmlInfo == null)
            return null;

        Dictionary<XmlNode, XmlNodeList> sceneObjects = new Dictionary<XmlNode, XmlNodeList>();

        XmlDocument xmlDoc = new XmlDocument();
        xmlDoc.LoadXml(xmlInfo.text);

        XmlNodeList sceneInfo = xmlDoc.GetElementsByTagName("SceneInfo");

        Dictionary<XmlNode, XmlNodeList> table1 = new Dictionary<XmlNode, XmlNodeList>();

        for (int i = 0; i < sceneInfo.Count; i++)
        {
            XmlNode childNode = sceneInfo[i];
            table1.Add(childNode, childNode.ChildNodes);
        }

        var itr1 = table1.Keys.GetEnumerator();
        while (itr1.MoveNext())
        {
            XmlNodeList nodeList = table1[itr1.Current];

            foreach (XmlNode node in nodeList)
            {
                if (node.Name.Equals("SceneName"))
                    continue;
                sceneObjects.Add(node, node.ChildNodes);
            }
        }
        itr1.Dispose();
        _totalCount += sceneObjects.Count;

        return sceneObjects;
    }

    public IEnumerator StartLoadSceneAsync(AssetBundleLoad load)
    {
        if(!_uninstallable_scene.ContainsKey(_scene_name))
            _uninstallable_scene.Add(_scene_name,false);

        _current = 0;
        _totalCount = 2;  //最少有2个，一个ab，一个场景加载

        CalculationDependencyCount(_asset_bundle_name,  load);

        TextAsset xmlInfo = load.LoadAsset<TextAsset>(_xml_asset_bundle_name);

        Dictionary<XmlNode, XmlNodeList> sceneObjects = GetSceneObjects(xmlInfo);

        AssetBundleLoaderRequest abRequest = AssetBundleLoaderRequest.New(_asset_bundle_name);
        load.LoadSingleAssetBundleAsync(abRequest);
        while (!abRequest.IsDone)
            yield return null;

        UpdateProgress();

        // 加载场景依赖项
        yield return LoadDependency(_asset_bundle_name, load);
        yield return SceneManager.LoadSceneAsync(_scene_name, _load_mode);
        UpdateProgress();

        // 复位Object对象
        if (sceneObjects != null) 
        {
            var itr = sceneObjects.Keys.GetEnumerator();
            while (itr.MoveNext())  // 遍历Object的节点
            {
                XmlElement objectNode = itr.Current as XmlElement;
                string objectPath = objectNode.GetAttribute("Path");
                string objectName = objectNode.GetAttribute("Name");

                AssetLoaderRequest request = AssetLoaderRequest.New(objectPath);
                load.LoadAssetAsync<Object>(request);

                while (!request.IsDone)
                    yield return null;
                UpdateProgress();

                GameObject gameObject = (GameObject)GameObject.Instantiate(request.asset);
                gameObject.name = objectName;

                foreach (XmlElement el in objectNode.ChildNodes)
                {
                    float x = float.Parse(el.GetAttribute("X"));
                    float y = float.Parse(el.GetAttribute("Y"));
                    float z = float.Parse(el.GetAttribute("Z"));

                    if (el.Name.Equals("Position"))
                    {
                        gameObject.transform.position = new Vector3(x, y, z);
                    }
                    else if (el.Name.Equals("Rotate"))
                    {
                        gameObject.transform.eulerAngles = new Vector3(x, y, z);
                    }
                    else if (el.Name.Equals("Scale"))
                    {
                        gameObject.transform.localScale = new Vector3(x, y, z);
                    }
                }
            }

            itr.Dispose();
        }

        UpdateProgress();
        IsDone = true;
        if (loadCompleted != null)
            loadCompleted(this);

        _uninstallable_scene[_scene_name] = true;
    }

    public void StartUnloadSceneAsync(AssetBundleLoad load)
    {
        if (!_uninstallable_scene.ContainsKey(_scene_name))
            return;

        if (!_uninstallable_scene[_scene_name])
            return;

        TextAsset xmlInfo = load.LoadAsset<TextAsset>(_xml_asset_bundle_name);
        Dictionary<XmlNode, XmlNodeList> sceneObjects = GetSceneObjects(xmlInfo);

        load.UninstallAssetBundle(_asset_bundle_name, true);
        if (sceneObjects != null)
        {
            var itr = sceneObjects.Keys.GetEnumerator();
            while (itr.MoveNext())  // 遍历Object的节点
            {
                XmlElement objectNode = itr.Current as XmlElement;
                string objectPath = objectNode.GetAttribute("Path");
                string objectName = objectNode.GetAttribute("Name");

                string assetBundleName = load.GetAssetBundleNameByAssetPath(objectPath);
                if (assetBundleName != string.Empty)
                    load.UninstallAssetBundle(assetBundleName, true);
            }
            itr.Dispose();
        }

        _uninstallable_scene.Remove(_scene_name);
    }

}
