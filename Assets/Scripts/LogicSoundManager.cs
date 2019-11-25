using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Reflection;

using UnityEngine;
using Mogo.Util;
using Mogo.GameData;
using Mogo.Game;



public class LogicSoundManager
{
    public static Dictionary<int, AudioClip> avatarAudioClipBuffer = new Dictionary<int, AudioClip>();
    static LogicSoundManager()
    { }
    public static void Init()
    {
        AddListeners();
    }
    public static void AddListeners()
    {
        EventDispatcher.AddEventListener<EntityParent, int>(Events.LogicSoundEvent.OnHitYelling, OnHitYelling);
    }
    public static void RemoveListeners()
    {
        EventDispatcher.RemoveEventListener<EntityParent, int>(Events.LogicSoundEvent.OnHitYelling, OnHitYelling);
    }
    public static void MyselfLogicPlaySound(AudioSource ownerSource, int soundID)
    {
        if (avatarAudioClipBuffer.ContainsKey(soundID))
        {
            EventDispatcher.TriggerEvent<AudioSource, AudioClip>(SettingEvent.LogicPlaySoundByClip, ownerSource, avatarAudioClipBuffer[soundID]);
            return;
        }
        if (!SoundData.dataMap.ContainsKey(soundID))
        {
            return;
        }
        AssetCacheMgr.GetResourceAutoRelease(
        SoundData.dataMap[soundID].path, (obj) =>
        {
            UnityEngine.Object.DontDestroyOnLoad(obj);
            if (obj is AudioClip)
            {
                EventDispatcher.TriggerEvent<AudioSource, AudioClip>(SettingEvent.LogicPlaySoundByClip, ownerSource, obj as AudioClip);
                if (!avatarAudioClipBuffer.ContainsKey(soundID))
                    avatarAudioClipBuffer.Add(soundID, obj as AudioClip);
            }
        });
    }
    protected static void LogicPlaySound(AudioSource ownerSource, int soundID)
    {
        EventDispatcher.TriggerEvent<int, AudioSource>(SettingEvent.LogicPlaySoundByID, soundID, ownerSource);
    }
    protected static void OnHitYelling(EntityParent entity, int action)
    {

    }
}