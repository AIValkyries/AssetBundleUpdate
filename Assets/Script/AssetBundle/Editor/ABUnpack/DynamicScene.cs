using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.SceneManagement;
using UnityEditor.SceneManagement;
using System.IO;
using System.Xml;

public class DynamicScene
{
    // 生成场景配置文件
    public static void GenerateAllSceneFile()
    {
        if (Selection.objects == null || Selection.objects.Length <= 0)
            return;

        for (int i = 0; i < Selection.objects.Length; i++)
        {
            string path = AssetDatabase.GetAssetPath(Selection.objects[i]);
            if (!File.Exists(path))
                continue;
            string ext = Path.GetExtension(path);
            if (ext.Equals(".unity"))
            {
                Scene scene = EditorSceneManager.OpenScene(path);
                GenerateSceneFile(scene);
            }
        }

        AssetDatabase.Refresh();
    }

    public static void GenerateSceneFile(Scene scene)
    {
        string sceneName = scene.name;
        string xmlFileName = PathConstant.PathName.DYNAMIC_SCENE_CONFIG_FILE_PATH + sceneName + ".xml";
        string sceneFolder = PathConstant.PathName.DYNAMIC_SCENE_PREFAB_PATH + sceneName;
        if (!Directory.Exists(sceneFolder))
        {
            Directory.CreateDirectory(sceneFolder);
            AssetDatabase.Refresh();
        }

        XmlDocument xmldoc = new XmlDocument();
        XmlDeclaration declaration = xmldoc.CreateXmlDeclaration("1.0", "utf-8", string.Empty);
        xmldoc.AppendChild(declaration);

        XmlElement root = xmldoc.CreateElement("SceneInfo");
        xmldoc.AppendChild(root);

        XmlElement element = xmldoc.CreateElement("SceneName");
        element.InnerText = sceneName;
        root.AppendChild(element);

        GameObject[] objects = scene.GetRootGameObjects();
        SceneConfig sceneConfig = new SceneConfig();
        List<GameObject> gameObjects = new List<GameObject>();
        for (int j = 0; j < objects.Length; j++)
        {
            GameObject @object = objects[j];
            SceneConfig.SceneObject sceneObject = new SceneConfig.SceneObject();
            sceneObject.Px = @object.transform.position.x;
            sceneObject.Py = @object.transform.position.y;
            sceneObject.Pz = @object.transform.position.z;

            sceneObject.Rx = @object.transform.eulerAngles.x;
            sceneObject.Ry = @object.transform.eulerAngles.y;
            sceneObject.Rz = @object.transform.eulerAngles.z;

            sceneObject.Sx = @object.transform.localScale.x;
            sceneObject.Sy = @object.transform.localScale.y;
            sceneObject.Sz = @object.transform.localScale.z;

            Object prefab = PrefabUtility.GetPrefabObject(@object);
            if (prefab == null)
            {
                string prefabPath = sceneFolder + "/" + @object.name + ".prefab";
                sceneObject.Path = prefabPath.Substring("Assets/".Length, prefabPath.Length - "Assets/".Length);
                PrefabUtility.CreatePrefab(prefabPath, @object);
            }

            sceneConfig.SceneName = sceneName;
            sceneConfig.Objects.Add(sceneObject);

            XmlElement objectsXml = xmldoc.CreateElement("Object");
            objectsXml.SetAttribute("Name", @object.name);
            objectsXml.SetAttribute("Path", sceneObject.Path);
            root.AppendChild(objectsXml);

            XmlElement position = xmldoc.CreateElement("Position");
            objectsXml.AppendChild(position);
            position.SetAttribute("X", sceneObject.Px.ToString());
            position.SetAttribute("Y", sceneObject.Py.ToString());
            position.SetAttribute("Z", sceneObject.Pz.ToString());

            XmlElement rotate = xmldoc.CreateElement("Rotate");
            objectsXml.AppendChild(rotate);

            rotate.SetAttribute("X", sceneObject.Rx.ToString());
            rotate.SetAttribute("Y", sceneObject.Ry.ToString());
            rotate.SetAttribute("Z", sceneObject.Rz.ToString());


            XmlElement scale = xmldoc.CreateElement("Scale");
            objectsXml.AppendChild(scale);

            scale.SetAttribute("X", sceneObject.Sx.ToString());
            scale.SetAttribute("Y", sceneObject.Sy.ToString());
            scale.SetAttribute("Z", sceneObject.Sz.ToString());

            gameObjects.Add(@object);
        }


        EditorSceneManager.SaveScene(scene);
        xmldoc.Save(xmlFileName);
 
        for (int i = 0; i < gameObjects.Count; i++) 
        {
            GameObject.DestroyImmediate(gameObjects[i]);
        }

        AssetDatabase.Refresh();
    }

    public static void RecoveryScene()
    {
        Scene scene = EditorSceneManager.GetActiveScene();
        string fileName = PathConstant.PathName.DYNAMIC_SCENE_CONFIG_FILE_PATH + scene.name + ".xml";

        XmlDocument xmldoc = new XmlDocument();
        xmldoc.Load(fileName);

        XmlNodeList sceneInfo = xmldoc.GetElementsByTagName("SceneInfo");

        Dictionary<XmlNode, XmlNodeList> table1 = new Dictionary<XmlNode, XmlNodeList>();
        Dictionary<XmlNode, XmlNodeList> table2 = new Dictionary<XmlNode, XmlNodeList>();

        for (int i = 0; i < sceneInfo.Count; i++)
        {
            XmlNode childNode = sceneInfo[i];
            table1.Add(childNode, childNode.ChildNodes);
        }

        var itr = table1.Keys.GetEnumerator();
        while (itr.MoveNext())
        {
            XmlNodeList nodeList = table1[itr.Current];

            foreach (XmlNode node in nodeList)
            {
                if (node.Name.Equals("SceneName"))
                    continue;
                table2.Add(node, node.ChildNodes);
            }
        }
        itr.Dispose();

        itr = table2.Keys.GetEnumerator();
        while (itr.MoveNext())  // 遍历Object的节点
        {
            XmlElement objectNode = itr.Current as XmlElement;
            string objectPath = "Assets/" + objectNode.GetAttribute("Path");
            string objectName = objectNode.GetAttribute("Name");

            Object @object = AssetDatabase.LoadAssetAtPath(objectPath, typeof(Object));
            GameObject gameObject = GameObject.Instantiate(@object) as GameObject;
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
        EditorSceneManager.SaveScene(scene);
        AssetDatabase.Refresh();
    }

    public static void ClearScene()
    {
        Scene scene = EditorSceneManager.GetActiveScene();
        string fileName = PathConstant.PathName.DYNAMIC_SCENE_CONFIG_FILE_PATH + scene.name + ".xml";
        if (!File.Exists(fileName))
        {
            Debug.Log("没有配置文件哦，请谨慎删除!");
            return;
        }
        GameObject[] gameObjects = scene.GetRootGameObjects();

        for (int i = 0; i < gameObjects.Length; i++)
        {
            GameObject.DestroyImmediate(gameObjects[i]);
        }

        EditorSceneManager.SaveScene(scene);
    }


}
