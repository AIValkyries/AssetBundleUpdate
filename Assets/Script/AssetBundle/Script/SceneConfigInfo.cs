using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneConfig
{
    public SceneConfig()
    {
        Objects = new List<SceneObject>();
    }

    public class SceneObject
    {
        public float Px;
        public float Py;
        public float Pz;
        public float Rx;
        public float Ry;
        public float Rz;
        public float Sx;
        public float Sy;
        public float Sz;
        public string Path;
    }

    public string SceneName;
    public List<SceneObject> Objects;
}
