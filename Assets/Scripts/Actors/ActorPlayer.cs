using UnityEngine;
using System.Collections;

using Mogo.Util;
using Mogo.Game;
using Mogo.GameData;

public class ActorPlayer : ActorParent<EntityPlayer> {}

public class ActorPlayer<T> : ActorParent<T> where T : EntityPlayer
{
    protected Transform m_billboardTrans;
    protected Transform m_wingBone;
    GameObject m_goWing;

    void Start() { }
    void Update() { }

    private string currWing;
    public void AddWing(string wingName, System.Action callBack) { }
    public void RemoveWing() { }
    public void SetLayer(int layer) { }
    public void SetObjectLayer(int layer, GameObject obj) { }
}