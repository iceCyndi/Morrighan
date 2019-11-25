using System;
using System.Collections.Generic;
using UnityEngine;
//using Mogo.Task;
//using Mogo.Mission;
using Mogo.Util;
using Mogo.GameData;

namespace Mogo.Game
{
    public partial class EntityMyself
    {
        #region 私有属性

//         private DoorOfBurySystem doorOfBurySystem;
//         private BodyEnhanceManager bodyenhanceManager;
//         public TaskManager taskManager;
//         private MissionManager missionManager;
//         private RuneManager runeManager;
//         private FriendManager friendManager;
//         public MarketManager marketManager;
//         private TowerManager towerManager;
//         private SanctuaryManager sanctuaryManager;
//         private ArenaManager arenaManager;
//         private OperationSystem operationSystem;
//         private DailyEventSystem dailyEventSystem;
//         private RankManager rankManager;
//         public CampaignSystem campaignSystem;
//         public FumoManager fumoManager;
//         public WingManager wingManager;
//         public RewardManager rewardManager;
//         private OccupyTowerSystem occupyTowerSystem;

        //private int lastRollAttackActionID = 0;
        //private float lastRollAttackTime = 0.0f;
        // 为去除警告暂时屏蔽以下代码
        //private float lastKeyDownTime = 0.0f;
        private Vector3 prePos = Vector3.zero;
        private uint timerID = uint.MaxValue;
        private uint checkDmgID;
        private uint syncHpTimerID = uint.MaxValue;
        private uint skillFailoverTimer = uint.MaxValue;
        // 为去除警告暂时屏蔽以下代码
        //private uint rateTimer = 0;

        private uint m_gold;
        private uint m_diamond;

        private uint m_airDamage;
        // private uint m_nextLevelExp;
        private uint m_airDefense;
        private uint m_allElementsDamage;
        private uint m_allElementsDefense;
        private uint m_antiCrit;
        private uint m_atk;
        private uint m_antiDefense;
        private uint m_antiTrueStrike;
        private uint m_crit;
        private uint m_critExtraAttack;
        private uint m_cdReduce;
        // private uint m_curHp;
        private uint m_def;
        private uint m_earthDamage;
        private uint m_fireDamage;
        private uint m_earthDefense;
        private uint m_fireDefense;
        private uint m_hit;
        private uint m_maxEnery;
        private uint m_pvpAddition;
        private uint m_pvpAnti;
        private uint m_speedAddRate;
        private uint m_trueStrike;
        private uint m_waterDamage;
        private uint m_waterDefense;

        private ushort m_sceneId;
        private ushort m_imap_id;
        private byte m_deathFlag;
        private uint m_anger;
        private uint m_curTask;

        private byte m_loginDays;

        private byte m_loginIsReward;

        private byte m_baseflag;

        private UInt32 m_buyHotSalesLastTime;

        private LuaTable m_wingBag;
        // 为去除警告暂时屏蔽以下代码
        //private System.UInt64 dummyThinkCDEnd = 0;

        #endregion


    }
}
