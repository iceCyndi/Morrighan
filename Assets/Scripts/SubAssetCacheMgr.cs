using UnityEngine;
using System.Collections.Generic;
using System;
using Object = UnityEngine.Object;
using System.Linq;
using System.Text;

public class SubAssetCacheMgr
{
    #region Gear
    private static HashSet<string> m_loadedGearResources = new HashSet<string>();
    public static void GetGearInstance(string resourceName, Action<String, int, Object> loaded)
    {
        AssetCacheMgr.GetNoCacheInstance(resourceName, (resource, id, go) =>
        {
            m_loadedGearResources.Add(resourceName);
            if (loaded != null)
                loaded(resource, id, go);
        });
    }
    public static void GetGearResource(string resourceName, Action<Object> loaded)
    {
        AssetCacheMgr.GetNoCacheResource(resourceName, (obj) =>
        {
            m_loadedGearResources.Add(resourceName);
            if (loaded != null)
                loaded(obj);
        });
    }
    public static void ReleaseGearResources()
    {
        foreach (var item in m_loadedGearResources)
        {
            AssetCacheMgr.ReleaseResourceImmediate(item);
        }
    }
    #endregion
    #region Character
    private static HashSet<String> m_loadedCharacterResources = new HashSet<string>();
    public static void GetPlayerInstance(string resourceName, Action<String, int, Object> loaded)
    {
        AssetCacheMgr.GetInstance(resourceName, loaded);
    }
    public static void GetCharacterInstance(string resourceName, Action<String, int, Object> loaded)
    {
        AssetCacheMgr.GetNoCacheInstance(resourceName,
        (prefab, id, go) =>
        {
            m_loadedCharacterResources.Add(resourceName);
            if (loaded != null)
                loaded(prefab, id, go);
        });
    }
    public static void GetCharacterResourcesAutoRelease(string[] resourcesName, Action<Object[]> loaded, Action<float> progress = null)
    {
        foreach (var item in resourcesName)
        {
            m_loadedCharacterResources.Add(item);
        }
        AssetCacheMgr.GetResourcesAutoRelease(resourcesName, loaded, progress);
    }
    public static void ReleaseCharacterResources()
    {
        foreach (var item in m_loadedCharacterResources)
        {
            AssetCacheMgr.ReleaseResourceImmediate(item);
        }
    }
    #endregion
}