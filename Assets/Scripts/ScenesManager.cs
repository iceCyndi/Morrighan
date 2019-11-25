using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System.Text.RegularExpressions;
using HMF;
using System.Collections;
using Mogo.Util;
using Mogo.GameData;

namespace Mogo.Game
{
    public class ScenesManager
    {
        private Int32 m_lastScene = -1;
        private String m_lastSceneResourceName = String.Empty;
        private MapData m_currentMap;
        private List<UnityEngine.Object> m_sceneObjects;
        private UnityEngine.Object m_lightmap;
        private UnityEngine.Object m_lightProbes;
        private UnityEngine.Object m_globalUI;
        private UnityEngine.GameObject m_mainUI;

        public UnityEngine.GameObject MainUI
        {
            get { return m_mainUI; }
        }
        public ScenesManager()
        {
            m_sceneObjects = new List<UnityEngine.Object>();
        }
        public void AddListeners()
        {

        }
        public void RemoveListeners()
        {

        }
        public bool CheckSameScene(int id)
        {
            if (m_lastScene == id)
            {
                LoggerHelper.Debug("Same scene: " + id);
                return true;
            }
            else
                return false;
        }
        public void LoadHomeScene(Action<Boolean> loaded, Action<int> process = null)
        {

            AssetCacheMgr.GetInstanceAutoRelease("Main_Camera.prefab", (prefab, id, go) =>
            {
                //if (m_currentMap != null)
                {
                    go.name = "Main_Camera";
                    var obj = go as GameObject;
                    obj.AddComponent<MogoMainCamera>();
                    var camera = obj.camera;
                    UnityEngine.Object.Destroy(camera.GetComponent<AudioListener>());
                   // camera.backgroundColor = m_currentMap.cameraColor;
                    camera.far = m_currentMap.cameraFar;
                //    if (data != null &&
                //        data.layerList != null && data.layerList.Count != 0 &&
                //        data.distanceList != null && data.distanceList.Count != 0 &&
                 //       data.layerList.Count == data.distanceList.Count)
                    {
                        //                         var cull = (go as GameObject).AddComponent<MogoCameraCullByLayer>();
                        //                         cull.LayerList = data.layerList;
                        //                         cull.DistanceList = data.distanceList;
                    }
                   // EventDispatcher.TriggerEvent(SettingEvent.BuildSoundEnvironment, missionID);
                }
          
                TimerHeap.AddTimer(500, 0, null);
            });
        }
        public void LoadCharacterScene(Action loaded, Action<int> process = null, bool CreateCharacter = false, bool hideReturnBtn = false)
        {
            Action action = () =>
            {
                //var serverInfo = SystemConfig.GetSelectedServerInfo();
                if (CreateCharacter)
                {
                    NewLoginUILogicManager.Instance.LoadCreateCharacter(() =>
                    {
                        if (loaded != null)
                            loaded();
                        NewLoginUIViewManager.Instance.ShowCreateCharacterUI();
                        NewLoginUIViewManager.Instance.ShowCreateCharacterUIBackBtn(!hideReturnBtn);
                        //if (serverInfo != null)
                        //   NewLoginUIViewManager.Instance.SetCreateCharacterServerBtnName(serverInfo.name);
                    });
                }
                else 
                {
                    NewLoginUIViewManager.Instance.ShowChooseCharacterUI();
                    if (loaded != null)
                        loaded();
                }
            };
            LoadScene(MogoWorld.globalSetting.chooseCharaterScene, (isLoadScene, data) =>
            {
                if (isLoadScene)
                {
                    if (LoginUIViewManager.Instance)
                    {
                        LoginUIViewManager.Instance.ReleaseUIAndResources();
                    }
                    LoadMainUI(() =>
                    {
                        MogoUIManager.Instance.ShowNewLoginUI(action);
                    });
                }
                else
                {
                    action();
                }
            }, null, false);
        }
        public void LoadLoginScene(Action<Boolean> loaded, Action<int> process = null)
        {
            LoadScene(MogoWorld.globalSetting.loginScene, (isLoadScene, data) =>
            {
                GameObject longRoot = GameObject.Find("longRoot");
                if (longRoot != null && longRoot.GetComponent<LoginLong>() == null)
                {
                    GameObject.Find("longRoot").AddComponent<LoginLong>();
                }
                if (isLoadScene)
                {
                    LoadMainUI(() =>
                    {
                        MogoUIManager.Instance.ShowMogoLoginUI(() =>
                        {
                            if (loaded != null)
                                loaded(isLoadScene);
                        },
                        (progress) =>
                        {
                            process((int)(10 * progress + 90));
                        });
                    },
                    (progress) =>
                    {
                        process((int)(10 * progress + 80));
                    });
                }
            },
            (progress) =>
            {
                process((int)(30 * progress / 100 + 50));
            }, true);
        }
        public void LoadInstance(Int32 id, Action<Boolean> loaded, Action<int> progress = null)
        {
      //      LoadScene(id, (isLoadScene, data) =>
           // {
               // if (isLoadScene)
                   // LoadMainUI(() =>
                //    {
                        if (loaded != null) ;
                           // loaded(isLoadScene);
                        LoadCamera((int)id, new MapData());
                      //  MogoUIManager.Instance.ShowMogoBattleMainUI();
                 //   });
     //       }, null, true);
        }
        public void UnLoadScene(Action callBack)
        {
            if (m_currentMap != null)
            {
                try
                {

                }
                catch (Exception ex)
                {
                    LoggerHelper.Except(ex);
                }

                foreach (var item in  m_sceneObjects)
                {
                    AssetCacheMgr.ReleaseInstance(item);
                }
                m_sceneObjects.Clear();
                AssetCacheMgr.ReleaseResource(m_lightmap);
                m_lightmap = null;
                AssetCacheMgr.ReleaseResource(m_lightProbes);
                m_lightProbes = null;
                SubAssetCacheMgr.ReleaseCharacterResources();
                SubAssetCacheMgr.ReleaseGearResources();
                if (!String.IsNullOrEmpty(m_lastSceneResourceName))
                    AssetCacheMgr.ReleaseResource(m_lastSceneResourceName);
                if (callBack != null)
                    callBack();
            }
            else
            {
                if (callBack != null)
                    callBack();
            }
        }
        private IEnumerator UnloadUnusedAssets(Action callBack)
        {
            yield return Resources.UnloadUnusedAssets();
            callBack();
        }
        private void preloadResource(int id, Action<int> process = null, Action action = null) 
        {
            if (action != null)
                action();
        }
        public void LoadScene(int id, Action<Boolean, MapData> loaded, Action<int> process = null, bool needLoading = true) 
        {
            if (needLoading)
            {

            }

            if (m_lastScene == id)
            {
                LoggerHelper.Debug("Same scene in LoadScene: " + id);
                if (loaded != null)
                {
                    try
                    {
                        loaded(false, null);
                    }
                    catch (Exception ex)
                    {
                        LoggerHelper.Except(ex);
                    }
                    if (needLoading)
                    {

                    }
                }
                return;
            }

            if (m_currentMap != null) ;
                //ResourceWatcher.Instance.SceneID = id;
            
            UnLoadScene(() =>
            {
                MapData data;
                bool flag = MapData.dataMap.TryGetValue(id, out data);
                if (!flag)
                {
                    LoggerHelper.Error("map_setting id not exist: " + id);
                    if (loaded != null)
                    {
                        try
                        {
                            loaded(false, null);
                        }
                        catch (Exception ex)
                        {
                            LoggerHelper.Except(ex);
                        }
                        if (needLoading)
                        {

                        }
                    }
                    return;
                }
                m_lastScene = id;
                m_currentMap = data;
            
                if (id == MogoWorld.globalSetting.loginScene)
                    EventDispatcher.TriggerEvent(SettingEvent.BuildSoundEnvironment, id);
                
                Action loadScene = () =>
                {
                    LoadScene(data, (isLoadScene) =>
                    {
                        LoggerHelper.Debug("RenderSetting: " + id);
                        if (loaded != null)
                        {
                            try
                            {
                                if (process != null)
                                    process(80);
                                if (MogoWorld.thePlayer != null) ;
                                    //MogoWorld.thePlayer.SetLightVisible(data.characherLight == GameData.LightType.Normal);
                                preloadResource(id, process, () =>
                                {
                                    if (process != null)
                                        process(99);
                                    loaded(isLoadScene, null);
                                    if (process != null)
                                        process(100);
                                });
                            }
                            catch (Exception ex)
                            {
                                LoggerHelper.Except(ex);
                            }
                        }
                    }, data.sceneName, process);
                };
                if (MogoWorld.thePlayer != null)
                {
                    MogoWorld.thePlayer.GotoPreparePosition();
                }
                loadScene();
            });

        }
        public void SwitchLightMapFog(int sceneId, Action<Boolean> loaded)
        {
            var mapData = MapData.dataMap.GetValueOrDefault(sceneId, null);
            if (mapData != null)
            {
                SwitchLightMapFog(mapData, loaded);
            }
            else
            {
                LoggerHelper.Error("MapData not exist: " + sceneId);
                if (loaded != null)
                    loaded(false);
            }
        }
        private void SwitchLightMapFog(MapData data, Action<Boolean> loaded)
        {
            RenderSettings.fog = data.fog;
            RenderSettings.fogColor = data.fogColor;
            RenderSettings.fogMode = data.fogMode;
            RenderSettings.fogStartDistance = data.linearFogStart;
            RenderSettings.fogEndDistance = data.linearFogEnd;
            RenderSettings.ambientLight = data.ambientLight;

            if (String.IsNullOrEmpty(data.lightmap))
            {
                if (loaded != null)
                    loaded(true);
            }
            else
            {
                AssetCacheMgr.GetSceneResource(data.lightmap, (lm) =>
                {
                    AssetCacheMgr.UnloadAssetbundle(data.lightmap);
                    m_lightmap = lm;
                    LightmapData lmData = new LightmapData();
                    lmData.lightmapFar = lm as Texture2D;
                    LightmapSettings.lightmaps = new LightmapData[1] { lmData };
                    if (loaded != null)
                        loaded(true);
                });
                if (!String.IsNullOrEmpty(data.lightProbes))
                    AssetCacheMgr.GetSceneResource(data.lightProbes, (lp) =>
                    {
                        AssetCacheMgr.UnloadAssetbundle(data.lightProbes);
                        m_lightProbes = lp;
                        LightmapSettings.lightProbes = lp as LightProbes;
                    });
            }
        }
        private void LoadCamera(int missionID, MapData data, Action loaded = null) 
        {

            AssetCacheMgr.GetInstanceAutoRelease("Main_Camera.prefab", (prefab, id, go) =>
            {
                if (m_currentMap != null)
                {
                    go.name = "Main_Camera";
                    var obj = go as GameObject;
                    obj.AddComponent<MogoMainCamera>();
                    var camera = obj.camera;
                    UnityEngine.Object.Destroy(camera.GetComponent<AudioListener>());
                    camera.backgroundColor = m_currentMap.cameraColor;
                    camera.far = m_currentMap.cameraFar;
                    if (data != null && 
                        data.layerList != null && data.layerList.Count != 0 &&
                        data.distanceList != null && data.distanceList.Count != 0 &&
                        data.layerList.Count == data.distanceList.Count)
                    {
//                         var cull = (go as GameObject).AddComponent<MogoCameraCullByLayer>();
//                         cull.LayerList = data.layerList;
//                         cull.DistanceList = data.distanceList;
                    }
                    EventDispatcher.TriggerEvent(SettingEvent.BuildSoundEnvironment, missionID);
                }
                if (loaded != null)
                    loaded();
                TimerHeap.AddTimer(500, 0, null);
            });
        }
        private void LoadGlobalUI(Action loaded) { }
        public void UnloadedMainUI()
        {
            if (m_mainUI)
            {
                try 
                {
                    GameObject.Destroy(m_mainUI);
                    m_mainUI = null;
                }
                catch (Exception ex)
                {
                    LoggerHelper.Except(ex);
                }
            }
        }
        private void LoadMainUI(Action loaded, Action<float> progress = null) 
        {
            if (m_mainUI)
            {
                if (loaded != null)
                    loaded();
            }
            else
                AssetCacheMgr.GetUIInstance(MogoWorld.globalSetting.homeUI, (prefab, guid, go) =>
                {
                    go.name = Mogo.Util.Utils.GetFileNameWithoutExtention(MogoWorld.globalSetting.homeUI);
                    GameObject.DontDestroyOnLoad(go);
                    var ui = go as GameObject;
                    ui.transform.localPosition = new Vector3(5000, 5000, 0);
                    if (ui)
                        ui.AddComponent<PrefabScriptManager>();
                    m_mainUI = ui;
                    if (loaded != null)
                        loaded();
                }, progress);
        }
        private void LoadScene(MapData data, Action<Boolean> loaded, string sceneName, Action<int> process = null)
        {
            try
            {
                Action sceneWasLoaded = () =>
                {
                    Action LevelWasLoaded = () =>
                    {
                        LoggerHelper.Debug("LevelWasLoaded: " + sceneName);
                       if (data.modelName.Count == 0)
                            if (loaded != null)
                                loaded(true);
                        LoggerHelper.Debug("modelName Count: " + data.modelName.Count);
                        if (process != null)
                            process(20);
                        var models = data.modelName.Keys.ToList();
                        for (int i = 0; i < models.Count; i++)
                        {
                            var currentOrder = i;
                            LoggerHelper.Debug("modelName order: " + currentOrder);
                            AssetCacheMgr.GetSceneInstance(models[currentOrder], 
                            (prefab, id, go) =>
                            {
                                go.name = Utils.GetFileNameWithoutExtention(models[currentOrder]);
                                if (data.modelName[models[currentOrder]])
                                    StaticBatchingUtility.Combine(go as GameObject);
                                LoggerHelper.Debug("sceneLoaded: " + go.name);
                                m_sceneObjects.Add(go);
                                if (currentOrder == data.modelName.Count - 1)
                                {
                                    AssetCacheMgr.UnloadAssetbundles(models.ToArray());
                                    SwitchLightMapFog(data, loaded);
                                }
                            }, (progress) =>
                            {
                                float cur = 60 * ((progress + currentOrder) / models.Count) + 20;
                                if (progress != null)
                                    process((int)(cur));
                            });
                        }
                        Driver.Instance.LevelWasLoaded = null;
                    };
                    if (sceneName != "10002_Login")
                    {
                        Driver.Instance.LevelWasLoaded = () =>
                        {
                            Driver.Instance.StartCoroutine(UnloadUnusedAssets(() =>
                            {
                                GC.Collect();
                                LevelWasLoaded();
                            }));
                        };
                        //
                        Application.LoadLevel(sceneName);
                    }
                    else
                    {
                        LevelWasLoaded();
                    }
                    LoggerHelper.Debug("LoadLevel: " + sceneName);
                };

                if (true)
                {
                    if (process != null)
                        process(5);
                    m_lastSceneResourceName = string.Concat(sceneName, ".unity");
                    AssetCacheMgr.GetResource(m_lastSceneResourceName,
                        (scene) =>
                        {
                            sceneWasLoaded();
                        },
                        (progress) =>
                        {
                            float cur = 15 * progress + 5;
                            if (process != null)
                                process((int)(cur));
                        });
                }
                else
                    sceneWasLoaded();
            }
            catch (Exception ex)
            {
                LoggerHelper.Except(ex);
            }
        }

    }
}
