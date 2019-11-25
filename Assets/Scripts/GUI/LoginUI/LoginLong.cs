using UnityEngine;
using System.Collections;
using Mogo.Util;

public class LoginLong : MonoBehaviour
{
    private GameObject m_onHitPoint;
    private GameObject m_lastFx = null;
    void Start()
    {
        InvokeRepeating("playAni", 0f, 20f);
    }

    void playAni()
    {
        if (animation != null)
        {
            animation.Play(AnimationPlayMode.Stop);
            Invoke("onHit", 8.1f);
        }
    }
    void onHit()
    {
        AssetCacheMgr.GetInstance("fx_scenes_long01.prefab", (prefab, id, obj) =>
        {
            if (this == null) return;
            if (obj == null) return;
            GameObject go = obj as GameObject;
            Utils.MountToSomeObjWithoutPosChange(go.transform, m_onHitPoint.transform);

            if (m_lastFx != null)
            {
                AssetCacheMgr.ReleaseInstance(m_lastFx);
                m_lastFx = null;
            }
            m_lastFx = go;
        });
    }
    void OnDestroy()
    {
        if (m_lastFx != null)
        {
            AssetCacheMgr.ReleaseInstance(m_lastFx);
            m_lastFx = null;
        }
        m_onHitPoint = null;
    }
}
