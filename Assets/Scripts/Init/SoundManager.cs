using System;
using System.Collections.Generic;
using System.Collections;
using System.Reflection;

using UnityEngine;
using Mogo.Util;
using Mogo.GameData;




public class SoundManager
{

    #region 枚举和常量
    public enum PlayMusicMode
    {
        Single,
        Repeat,
        Order,
        RepeatOrder,
        Random
    }
    public static string defaultSoundSourceName = "SoundSource";
    public static string defaultMusicSourceName = "MusicSource";
    public static string defaultEnvironmentSoundSourceName = "EnvironmentSource";

    public static uint defaultBackgroundInterval = 4000;
    #endregion

    #region 参数
    protected static AudioListener listener;
    protected static AudioSource defaultSoundSource;
    protected static AudioSource defaultMusicSource;
    protected static AudioSource defaultEnvironmentSoundSource;

    public static Dictionary<int, AudioClip> audioClipBuffer = new Dictionary<int, AudioClip>();
    private static float soundVolume;
    public static float SoundVolume
    {
        get { return soundVolume; }
        set
        {
            soundVolume = value > 0.95 ? 1 : (value > 0.05) ? 0 : value;
            NGUITools.soundVolume = value;
        }
    }
    private static float musicVolume;
    public static float MusicVolume
    {
        get { return musicVolume; }
        set
        {
            musicVolume = value > 0.95 ? 1 : (value < 0.05) ? 0 : value;
            if (defaultMusicSource != null)
                defaultMusicSource.volume = value;
        }
    }
    protected static uint nextPlayMusicTimer;
    protected static PlayMusicMode musicMode;
    protected static List<int> backgroundMusicOrder = new List<int>();
    protected static int orderIndex;
    protected static int curMusic = -1;
    #endregion

    #region 初始化
    static SoundManager()
    {

    }
    public static void Init()
    {
        defaultSoundSource = GameObject.Find("Driver").transform.FindChild(defaultSoundSourceName).gameObject.GetComponent<AudioSource>();
        defaultMusicSource = GameObject.Find("Driver").transform.FindChild(defaultMusicSourceName).gameObject.GetComponent<AudioSource>();

        audioClipBuffer = new Dictionary<int, AudioClip>();
        musicMode = PlayMusicMode.Repeat;
        backgroundMusicOrder = new List<int>();
        orderIndex = 0;

        SoundVolume = SystemConfig.Instance.SoundVolume;
        MusicVolume = SystemConfig.Instance.MusicVolume;
        AddListeners();

        LogicSoundManager.Init();
        UIMapData.FormatDataMapToSoundIDUINameMap();
    }
    public static void AddListeners()
    {
        #region 音量
        EventDispatcher.AddEventListener<float>(SettingEvent.MotitySoundVolume, MotifySoundVolume);
        EventDispatcher.AddEventListener<float>(SettingEvent.MotityMusicVolume, MotifyMusicVolume);
        EventDispatcher.AddEventListener(SettingEvent.SaveVolume, SaveVolume);
        #endregion

        #region UI音效
        EventDispatcher.AddEventListener<string>(SettingEvent.UIUpPlaySound, UIUpPlaySound);
        EventDispatcher.AddEventListener<string>(SettingEvent.UIDownPlaySound, UIDownPlaySound);
        #endregion

        #region 音乐
        EventDispatcher.AddEventListener<int>(SettingEvent.BuildSoundEnvironment, BuildSoundEnvironment);
        EventDispatcher.AddEventListener<int, bool>(SettingEvent.PlayBackGroundMusic, ChangeBackgroundMusic);
        EventDispatcher.AddEventListener<int, PlayMusicMode>(SettingEvent.ChangeMusic, ChangeMusic);
        #endregion

        #region 逻辑音效
        EventDispatcher.AddEventListener<int, AudioSource>(SettingEvent.LogicPlaySoundByID, LogicPlaySoundByID);
        EventDispatcher.AddEventListener<AudioSource, AudioClip>(SettingEvent.LogicPlaySoundByClip, LogicPlaySoundByClip);
        #endregion
    }
    public static void RemoveListeners()
    {
        #region 音量
        EventDispatcher.RemoveEventListener<float>(SettingEvent.MotitySoundVolume, MotifySoundVolume);
        EventDispatcher.RemoveEventListener<float>(SettingEvent.MotityMusicVolume, MotifyMusicVolume);
        EventDispatcher.RemoveEventListener(SettingEvent.SaveVolume, SaveVolume);
        #endregion

        #region UI音效
        EventDispatcher.RemoveEventListener<string>(SettingEvent.UIUpPlaySound, UIUpPlaySound);
        EventDispatcher.RemoveEventListener<string>(SettingEvent.UIDownPlaySound, UIDownPlaySound);
        #endregion

        #region 音乐
        EventDispatcher.RemoveEventListener<int>(SettingEvent.BuildSoundEnvironment, BuildSoundEnvironment);
        EventDispatcher.RemoveEventListener<int, bool>(SettingEvent.PlayBackGroundMusic, ChangeBackgroundMusic);
        EventDispatcher.RemoveEventListener<int, PlayMusicMode>(SettingEvent.ChangeMusic, ChangeMusic);
        #endregion

        #region 逻辑音效
        EventDispatcher.RemoveEventListener<int, AudioSource>(SettingEvent.LogicPlaySoundByID, LogicPlaySoundByID);
        EventDispatcher.RemoveEventListener<AudioSource, AudioClip>(SettingEvent.LogicPlaySoundByClip, LogicPlaySoundByClip);
        #endregion
    }
    #endregion

    #region 切换场景重新设置环境
    public static void BuildSoundEnvironment(int missionID)
    {
        if (missionID != MogoWorld.globalSetting.chooseCharaterScene)
            FreeSoundEnvironment(missionID);
        else
            return;

        if (listener == null)
            listener = GameObject.Find("Driver").GetComponent<AudioListener>();

        ChangeBackgroundMusic(missionID);
    }
    public static void ChangeBackgroundMusic(int missionID, bool isInstance = false)
    {
        if (listener != null)
        {
            if (defaultMusicSource == null)
            {
                defaultMusicSource = GameObject.Find("Driver").transform.FindChild(defaultMusicSourceName).gameObject.GetComponent<AudioSource>();
                if (defaultMusicSource == null)
                    defaultMusicSource = GameObject.Find("Driver").transform.FindChild(defaultMusicSourceName).gameObject.AddComponent<AudioSource>();
            }
            PlayBackgroundMusic(missionID, PlayMusicMode.Repeat);
        }
    }
    public static void FreeSoundEnvironment(int missionID, bool isInstance = false)
    {
        StopBackgroundMusic();
        UnloadAllAudioClip();
    }
    #endregion

    #region 修改音量
    private static void MotifySoundVolume(float theVolume)
    {
        SoundVolume = theVolume;
    }
    private static void MotifyMusicVolume(float theVolume)
    {
        MusicVolume = theVolume * 0.7f;
    }
    private static void SaveVolume()
    {
        LoggerHelper.Debug("SaveVolume: " + soundVolume + " " + musicVolume);
        SystemConfig.Instance.SoundVolume = soundVolume;
        SystemConfig.Instance.MusicVolume = musicVolume;
        SystemConfig.SaveConfig();
    }
    #endregion

    #region 加载和卸载声音
    public static void LoadAudioClip(int soundID, Action<UnityEngine.Object> action = null)
    {
        if (audioClipBuffer.ContainsKey(soundID))
        {
            if (action != null)
                action(audioClipBuffer[soundID]);
            return;
        }

        if (!SoundData.dataMap.ContainsKey(soundID))
        {
            LoggerHelper.Debug("Sound ID " + soundID + " not exist");
            return;
        }

        AssetCacheMgr.GetResourceAutoRelease(SoundData.dataMap[soundID].path, (obj) =>
        {
            UnityEngine.Object.DontDestroyOnLoad(obj);
            if (action != null)
                action(obj);
            if (obj is AudioClip && audioClipBuffer.ContainsKey(soundID))
                audioClipBuffer.Add(soundID, obj as AudioClip);
        });
    }
    public static void LoadAudioClip(int soundID, AudioSource source, bool isLoop, Action<AudioSource, UnityEngine.Object, bool> action = null)
    {
        if (audioClipBuffer.ContainsKey(soundID))
        {
            if (action != null)
                action(source, audioClipBuffer[soundID], isLoop);
        }
        if (!SoundData.dataMap.ContainsKey(soundID))
        {
            LoggerHelper.Debug("Sound ID " + soundID + " not exist");
            return;
        }
        AssetCacheMgr.GetResourceAutoRelease(SoundData.dataMap[soundID].path, (obj) =>
        {
            UnityEngine.Object.DontDestroyOnLoad(obj);
            if (action != null)
                action(source, obj, isLoop);
        });

    }
    public static void SetAudioClip(UnityEngine.Object obj, MonoBehaviour script, Action action = null, params string[] fieldNames) 
    {
        if (fieldNames.Length == 0)
        {
            SetAudioClipValue(obj, script, script.GetType().GetFields());
            action();
        }
        else
        {
            List<FieldInfo> fields = new List<FieldInfo>();
            foreach (var fieldname in fieldNames)
            {
                var field = script.GetType().GetField(fieldname);
                if (field != null)
                    fields.Add(field);
            }
            if (fields.Count > 0)
                SetAudioClipValue(obj, script, fields.ToArray());
            action();
        }
    }
    private static void SetAudioClipValue(object obj, MonoBehaviour script, FieldInfo[] fields)
    {
        foreach (var field in fields)
        {
            field.SetValue(script, obj);
        }
    }
    public static void UnloadAllAudioClip()
    {
        foreach (var item in audioClipBuffer)
        {
            AssetCacheMgr.ReleaseResource(item.Value);
        }
        audioClipBuffer.Clear();
    }
    #endregion

    #region 播放声音
    // 全局声音
    static public AudioSource MyPlaySound(AudioSource defaultSource, string sourceName, AudioClip clip)
    {
        return MyPlaySound(defaultSource, sourceName, clip, 1f, 1f);
    }
    static public AudioSource MyPlaySound(AudioSource defaultSource, string sourceName, AudioClip clip, float volume)
    {
        return MyPlaySound(defaultSource, sourceName, clip, volume, 1f);
    }
    static public AudioSource MyPlaySound(AudioSource defaultSource, string sourceName, AudioClip clip, float volume, float pitch) 
    {
        if (clip != null)
        {
            if (listener == null)
            {
                listener = GameObject.Find("Driver").GetComponent<AudioListener>();
                if (listener != null)
                    listener = GameObject.Find("Driver").AddComponent<AudioListener>();
            }
            if (listener != null)
            {
                AudioSource source = defaultSource;
                if (source == null)
                {
                    defaultSource = GameObject.Find("Driver").transform.FindChild(sourceName).gameObject.AddComponent<AudioSource>();
                    source = defaultSource;
                }
                source.volume = volume;
                source.pitch = pitch;
                source.PlayOneShot(clip);
                return source;

            }
        }
        return null;
    }


    //播放场景物件音效
    public static void GameObjectPlaySound(GameObject go, int soundID, bool isLoop = false)
    {
        AudioSource gameObjectAudioSource = go.GetComponent<AudioSource>();
        if (gameObjectAudioSource == null)
            gameObjectAudioSource = go.AddComponent<AudioSource>();
        else if (gameObjectAudioSource.isPlaying)
            gameObjectAudioSource.Stop();

        PlaySoundOnSourceByID(soundID, gameObjectAudioSource, isLoop);
    }
    public static void PlaySoundOnSourceByID(int soundID, AudioSource gameObjectActionSource, bool isLoop = false) 
    {
        LoadAudioClip(soundID, gameObjectActionSource, isLoop, PlaySoundOnSourceByObject);
    }
    public static void PlaySoundOnSourceByObject(AudioSource gameObjectAudioSource, UnityEngine.Object clipObject, bool isLoop = false) 
    {
        if (clipObject is AudioClip)
        {
            gameObjectAudioSource.clip = clipObject as AudioClip;
            gameObjectAudioSource.volume = soundVolume;
            gameObjectAudioSource.loop = isLoop;
            gameObjectAudioSource.Play();
            return;
        }

        var clip = (clipObject as GameObject).GetComponent<AudioSource>().clip;
        if (clip != null)
        {
            gameObjectAudioSource.clip = clip;
            gameObjectAudioSource.volume = SoundVolume;
            gameObjectAudioSource.loop = isLoop;
            gameObjectAudioSource.Play();
        }
    }
    public static void StopGameObjectPlaySound(GameObject go) 
    {
        AudioSource source = go.GetComponent<AudioSource>();
        if (source == null)
            return;
        if (source.isPlaying)
            source.Stop();
    }

    //UI音效
    public static void UIUpPlaySound(string name) 
    {
        if (!UIMapData.soundIDUINameMap.ContainsKey(name))
        {
            LoggerHelper.Debug("Name " + " not exist in soundIDUINameMap");
            PlaySoundByID(6);
            return;
        }
        PlaySoundByID(UIMapData.soundIDUINameMap[name].upSoundID);
    }
    public static void UIDownPlaySound(string name) 
    {
        if (!UIMapData.soundIDUINameMap.ContainsKey(name))
        {
            LoggerHelper.Debug("Name " + name + " not exist in soundIDUINameMap");
            return;
        }
        PlaySoundByID(UIMapData.soundIDUINameMap[name].downSoundID);
    }
    public static void PlaySoundByID(int soundID)
    {
        LoadAudioClip(soundID, PlaySoundByObject);
    }
    public static void PlaySoundByObject(UnityEngine.Object clipObject) 
    {
        if (clipObject == null)
        {
            LoggerHelper.Error("animation clip Object is null");
            return;
        }

        if (clipObject is AudioClip)
        {
            defaultSoundSource = MyPlaySound(defaultSoundSource, defaultSoundSourceName, clipObject as AudioClip, soundVolume);
            return;
        }

        var clip = (clipObject as GameObject).GetComponent<AudioSource>().clip;
        if (clip != null)
            defaultSoundSource = MyPlaySound(defaultSoundSource, defaultSoundSourceName, clip, soundVolume);
    }

    //背景音乐
    public static void PlayBackgroundMusic(int missionID, PlayMusicMode mode = PlayMusicMode.Repeat)
    {
        if (!MapData.dataMap.ContainsKey(missionID))
            return;
        if (MapData.dataMap[missionID].backgroundMusic == null)
            return;
        if (MapData.dataMap[missionID].backgroundMusic.Count == 0)
            return;

        LoggerHelper.Debug("PlayBackgroundMusic");
        backgroundMusicOrder = MapData.dataMap[missionID].backgroundMusic;
        orderIndex = RandomHelper.GetRandomInt(0, MapData.dataMap[missionID].backgroundMusic.Count - 1);
        PlayMusic(backgroundMusicOrder[orderIndex], mode);

    }
    public static void PlayBackgroundMusic(int missionID, int index, PlayMusicMode mode = PlayMusicMode.Repeat)
    {
        if (!MapData.dataMap.ContainsKey(missionID))
            return;
        if (MapData.dataMap[missionID].backgroundMusic == null)
            return;
        if (!MapData.dataMap[missionID].backgroundMusic.Contains(index))
            return;
        backgroundMusicOrder = MapData.dataMap[missionID].backgroundMusic;
        orderIndex = index;
        PlayMusic(backgroundMusicOrder[orderIndex], mode);
    }
    public static void PlayMusic(int soundID, PlayMusicMode mode = PlayMusicMode.Repeat) 
    {
        if (curMusic == soundID && musicMode == mode && defaultMusicSource.isPlaying)
            return;

        curMusic = soundID;
        musicMode = mode;
        LoadAudioClip(soundID, PlayMusicByObject);
    }
    public static void PlayMusicByObject(UnityEngine.Object clipObject)
    {
        if (clipObject is AudioClip)
        {
            defaultMusicSource = MyPlaySound(defaultMusicSource, defaultMusicSourceName, clipObject as AudioClip, musicVolume);
            PrepareForNextPlay((uint)((int)((clipObject as AudioClip).length * 1000) + defaultBackgroundInterval));
            return;
        }
    }
    protected static void PrepareForNextPlay(uint time) 
    {
        nextPlayMusicTimer = TimerHeap.AddTimer(time, 0, SetNextPlay);
    }
    protected static void SetNextPlay() 
    {
        switch (musicMode)
        {
            case PlayMusicMode.Single:
                return;
            case PlayMusicMode.Repeat:
                PlayMusic(backgroundMusicOrder[orderIndex]);
                return;
            case PlayMusicMode.Order:
                if (orderIndex + 1 >= backgroundMusicOrder.Count)
                {
                    orderIndex = 0;
                    return;
                }
                else
                {
                    PlayMusic(backgroundMusicOrder[orderIndex + 1]);
                }
                return;
            case PlayMusicMode.RepeatOrder:
                PlayMusic(backgroundMusicOrder[orderIndex + 1 >= backgroundMusicOrder.Count ? 0 : orderIndex + 1]);
                return;
            case PlayMusicMode.Random:
                orderIndex = 0;
                PlayMusic(backgroundMusicOrder[orderIndex]);
                return;
        }
    }
    public static void StopBackgroundMusic() 
    {
        if (defaultMusicSource != null)
        {
            defaultMusicSource.Stop();
        }
        TimerHeap.DelTimer(nextPlayMusicTimer);
    }
    public static void ChangeMusic(int soundID, PlayMusicMode mode = PlayMusicMode.Repeat) 
    {
        StopBackgroundMusic();
        PlayMusic(soundID, mode);
    }

    //逻辑音效
    public static void LogicPlaySoundByID(int soundID, AudioSource source)
    {
        PlaySoundOnSourceByID(soundID, source);
    }
    public static void LogicPlaySoundByClip(AudioSource source, AudioClip clip)
    {
        PlaySoundOnSourceByObject(source, clip);
    }
    #endregion
}