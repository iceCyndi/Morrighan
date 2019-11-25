using System;
using System.Collections;
using Mogo.Util;
//using Mogo.Game;
using Mogo.GameData;
using System.Collections.Generic;


public class MainUILogicManager : UILogicManager
{
    private static MainUILogicManager m_instance;
    public static MainUILogicManager Instance
    {
        get
        {
            if (m_instance == null)
                m_instance = new MainUILogicManager();

            return MainUILogicManager.m_instance;
        }
    }

    public bool isNormalAttackPowerUp = false;
    public bool hasShowBossBlood = false;
    private void OnPowerChargeStart() {}
    private void OnPowerChargeInterrupt() { }
    private void OnPowerChargeComplete() { }
    private void OnNormalAttack() { }
    private void OnTaskInfoUp() { }
    private void OnOutputUp() { }
    void OnAffectUp() { }
    void OnMoveUp() { }
    void OnSpecialUp() { }
    void OnUseHpBottle() { }
    void OnCommunityUp() { }

    public void Initialize()
    { }
    protected bool isAttackable = true;
    public bool IsAttackable
    {
        get { return isAttackable; }
        set { isAttackable = value; }
    }
}