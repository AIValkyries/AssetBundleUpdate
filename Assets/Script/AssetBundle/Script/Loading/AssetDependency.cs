/****************************************************
	文件：AssetDependency.cs
	作者：Lonely
	github：https://github.com/AIValkyries/AssetBundleUpdate
	日期：2020/07/29 22:27:32
	功能：Nothing
*****************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AssetDependency
{
    Dictionary<string, List<string>> _dependency;

    public List<string> this[string assetBundleName]
    {
        get
        {
            if (_dependency.ContainsKey(assetBundleName))
            {
                return _dependency[assetBundleName];
            }
            return null;
        }
    }

    public AssetDependency()
    {
        _dependency = new Dictionary<string, List<string>>();
    }

    public void Add(string maiABName, string[] assetBundleName)
    {
        if (_dependency.ContainsKey(maiABName))
        {
            _dependency[maiABName].AddRange(assetBundleName);
        }
        else
        {
            List<string> abs = new List<string>();
            abs.AddRange(assetBundleName);
            _dependency.Add(maiABName, abs);
        }
    }

    public void Add(string maiABName, string assetBundleName)
    {
        if (_dependency.ContainsKey(maiABName))
        {
            if (!_dependency[maiABName].Contains(assetBundleName))
                _dependency[maiABName].Add(assetBundleName);
        }
        else
        {
            _dependency.Add(maiABName,new List<string>());
            _dependency[maiABName].Add(assetBundleName);
        }
    }

    public void Add(string maiABName,List<string> assetBundleName)
    {
        if (_dependency.ContainsKey(maiABName))
        {
            _dependency[maiABName].AddRange(assetBundleName);
        }
        else
        {
            _dependency.Add(maiABName, assetBundleName);
        }
    }

    public List<string> GetDependency(string mainABName)
    {
        List<string> dependecy = null;
        if (_dependency.TryGetValue(mainABName,out dependecy))
        {
            return dependecy;
        }
        return null;
    }

}
