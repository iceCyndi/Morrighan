/*----------------------------------------------------------------
// Copyright (C) 2013 广州，爱游
//
// 模块名：EntityMyself
// 创建者：Steven Yang
// 修改者列表：
// 创建日期：2013.2.1
// 模块描述：角色控制对像
//----------------------------------------------------------------*/

using System;
using System.Collections.Generic;
using UnityEngine;
//using Mogo.Task;
//using Mogo.Mission;
using Mogo.GameData;
using Mogo.Util;
using Mogo.RPC;
// using Mogo.FSM;
// 
// using Mogo.AI;
// using Mogo.AI.BT;

public enum VipRealStateEnum
{
    DAILY_GOLD_METALLURGY_TIMES = 1, // 每日已炼金次数
    DAILY_RUNE_WISH_TIMES = 2, // 符文已许愿次数
    DAILY_ENERGY_BUY_TIMES = 3, // 体力已购买次数
    DAILY_EXTRA_CHALLENGE_TIMES = 4, // 每日额外挑战次数
    DAILY_HARD_MOD_RESET_TIMES = 5, // 困难副本进入已重置次数
    DAILY_RAID_SWEEP_TIMES = 6, // 剧情关卡已扫荡次数
    DAILY_TOWER_SWEEP_TIMES = 7, // 试炼之塔已扫荡次数
    DAILY_TIME_STAMP = 8, // 每日玩家时间戳记录
    DAILY_ITEM_CAN_BUY_ENTER_SDTIMES = 9, // 圣域守卫战额外购买次数
    DAILY_MISSION_TIMES = 10,// 每天副本的进入次数
}

namespace Mogo.Game
{
    public partial class EntityMyself : EntityPlayer
    {
        public const string ON_TASK_GUIDE = "EntityMyself.ON_TASK_GUIDE";
        public const string ON_END_TASK_GUIDE = "EntityMyself.ON_END_TASK_GUIDE";
        public const string ON_TASK_MISSION = "EntityMyself.ON_TASK_MISSION";
        public const string ON_VIP_REAL_STATE = "EntityMyself.ON_VIP_REAL_STATE";

        public static float preSkillTime = 0;//上次使用技能时间

        public bool isInCity;

        private int m_iAiId;

        private GameObject m_roundLight;




        protected bool TimeLimitEventHasOpen = false;



        private bool isNewPlayer;
        public bool IsNewPlayer
        {
            get { return isNewPlayer; }
            set { isNewPlayer = value; }
        }

        protected int applyMissionID = 0;
        public int ApplyMissionID
        {
            get { return applyMissionID; }
            set { applyMissionID = value; }
        }

        protected int applyMissionLevel = 0;
        public int ApplyMissionLevel
        {
            get { return applyMissionLevel; }
            set { applyMissionLevel = value; }
        }

        protected int curMissionID = 0;
        public int CurMissionID
        {
            get { return curMissionID; }
            set { curMissionID = value; }
        }

        protected int curMissionLevel = 0;
        public int CurMissionLevel
        {
            get { return curMissionLevel; }
            set { curMissionLevel = value; }
        }

        #region 构造函数

        public EntityMyself()
        {
            entityType = "Avatar";
            AddListener();
// 
//             VIPInfoUILogicManager.Instance.ItemSource = this;
//             MainUILogicManager.Instance.ItemSource = this;
//             NormalMainUILogicManager.Instance.ItemSource = this;
//             MenuUILogicManager.Instance.ItemSource = this;
//             ArenaUILogicManager.Instance.ItemSource = this;
//             EnergyUILogicManager.Instance.ItemSource = this;
//             DiamondToGoldUILogicManager.Instance.ItemSource = this;
//             EnergyNoEnoughUILogicManager.Instance.ItemSource = this;
//             LevelNoEnoughUILogicManager.Instance.ItemSource = this;
//             UpgradePowerUILogicManager.Instance.ItemSource = this;
//             DragonMatchUILogicManager.Instance.ItemSource = this;
//             ChooseDragonUILogicManager.Instance.ItemSource = this;
        }

        #endregion
        /*
        public int GetEquipSubType()
        {
            int subtype = 0;
            if (InventoryManager.Instance == null || InventoryManager.Instance.EquipOnDic == null)
            {
                return subtype;
            }
            var equip = InventoryManager.Instance.EquipOnDic.Get((int)EquipSlot.Weapon);
            if (equip == null)
                return subtype;
            if (!ItemEquipmentData.dataMap.ContainsKey((int)(equip.templateId)))
            {
                switch (vocation)
                {
                    case Vocation.Warrior:
                        subtype = (int)(WeaponSubType.blade);
                        break;
                    case Vocation.Archer:
                        subtype = (int)(WeaponSubType.bow);
                        break;
                    case Vocation.Assassin:
                        subtype = (int)(WeaponSubType.twinblade);
                        break;
                    case Vocation.Mage:
                        subtype = (int)(WeaponSubType.staff);
                        break;
                    case Vocation.Dragon:
                        subtype = (int)(WeaponSubType.staff);
                        break;
                    default:
                        LoggerHelper.Error("vocation Error:" + vocation);
                        return subtype;
                }
            }
            else
            {
                subtype = ItemEquipmentData.dataMap.Get((int)(equip.templateId)).subtype;
            }
            return subtype;
        }
        */
        #region 重写方法
        // 对象进入场景，在这里初始化各种数据， 资源， 模型等
        // 传入数据。
        override public void OnEnterWorld()
        {
            base.OnEnterWorld();
            //LoggerHelper.Info("Avatar name: " + name);
            // 在调用该函数前， 数据已经通过EntityAttach 和 EntityCellAttach 同步完毕
            CreateModel();
            /*
            inventoryManager = new InventoryManager(this);
            bodyenhanceManager = new BodyEnhanceManager(this);
            skillManager = new PlayerSkillManager(this);
            battleManger = new PlayerBattleManager(this, skillManager as PlayerSkillManager);

            doorOfBurySystem = new DoorOfBurySystem();
            runeManager = new RuneManager(this);
            towerManager = new TowerManager(this);
            missionManager = new MissionManager(this);
            taskManager = new TaskManager(this, (int)taskMain);
            marketManager = new MarketManager(this);
            friendManager = new FriendManager(this);
            operationSystem = new OperationSystem(this);
            sanctuaryManager = new SanctuaryManager(this);
            arenaManager = new ArenaManager(this);
            dailyEventSystem = new DailyEventSystem(this);
            rankManager = new RankManager(this);
            campaignSystem = new CampaignSystem(this);
            wingManager = new WingManager(this);
            rewardManager = new RewardManager(this);
            occupyTowerSystem = new OccupyTowerSystem(this);

            TipManager.Init();
            DragonMatchManager.Init();
            fumoManager = new FumoManager(this);

            MailManager.Instance.IsMailInfoDirty = true;
            TongManager.Instance.Init();
            GuideSystem.Instance.AddListeners();
            StoryManager.Instance.AddListeners();
            EventDispatcher.AddEventListener(Events.UIBattleEvent.OnNormalAttack, NormalAttack);
            EventDispatcher.AddEventListener(Events.UIBattleEvent.OnPowerChargeStart, PowerChargeStart);
            EventDispatcher.AddEventListener(Events.UIBattleEvent.OnPowerChargeInterrupt, PowerChargeInterrupt);
            EventDispatcher.AddEventListener(Events.UIBattleEvent.OnPowerChargeComplete, PowerChargeComplete);
            EventDispatcher.AddEventListener(Events.UIBattleEvent.OnSpellOneAttack, SpellOneAttack);
            EventDispatcher.AddEventListener(Events.UIBattleEvent.OnSpellTwoAttack, SpellTwoAttack);
            EventDispatcher.AddEventListener(Events.UIBattleEvent.OnSpellThreeAttack, SpellThreeAttack);
            EventDispatcher.AddEventListener(Events.UIBattleEvent.OnSpellXPAttack, SpecialAttack);
            EventDispatcher.AddEventListener<int>(Events.UIBattleEvent.OnSpriteSkill, OnSpriteSkill);

            EventDispatcher.AddEventListener<uint>(Events.GearEvent.Teleport, Teleport);
            EventDispatcher.AddEventListener<uint, int, int, int>(Events.GearEvent.Damage, SetDamage);

            EventDispatcher.AddEventListener<int, bool>(Events.InstanceEvent.InstanceLoaded, InstanceLoaded);
            EventDispatcher.AddEventListener<ushort>(Events.OtherEvent.MapIdChanged, OnMapChanged);
            EventDispatcher.AddEventListener<ulong>(Events.OtherEvent.Withdraw, Withdraw);
            EventDispatcher.AddEventListener(Events.OtherEvent.DiamondMine, DiamondMine);
            EventDispatcher.AddEventListener(Events.OtherEvent.CheckCharge, CheckCharge);
            EventDispatcher.AddEventListener(Events.OtherEvent.Charge, Charge);

            EventDispatcher.AddEventListener(ON_TASK_GUIDE, TaskGuide);
            EventDispatcher.AddEventListener(ON_END_TASK_GUIDE, EndTaskGuide);
            EventDispatcher.AddEventListener<int, int>(ON_TASK_MISSION, MissionOpen);

            EventDispatcher.AddEventListener(Events.AIEvent.DummyThink, DummyThink);
            EventDispatcher.AddEventListener(Events.StoryEvent.CGBegin, ProcCGBegin);
            EventDispatcher.AddEventListener(Events.StoryEvent.CGEnd, ProcCGEnd);
            EventDispatcher.AddEventListener<string>(Events.GearEvent.TrapBegin, ProcTrapBegin);
            EventDispatcher.AddEventListener<string>(Events.GearEvent.TrapEnd, ProcTrapEnd);
            EventDispatcher.AddEventListener(Events.GearEvent.LiftEnter, ProcLiftEnter);
            EventDispatcher.AddEventListener<int>(Events.GearEvent.PathPointTrigger, PathPointTrigger);
            EventDispatcher.AddEventListener(Events.DirecterEvent.DirActive, DirActive);

            EventDispatcher.AddEventListener<int>(Events.EnergyEvent.BuyEnergy, BuyEnergy);
            EventDispatcher.AddEventListener(ON_VIP_REAL_STATE, OnVIPRealState);

            EventDispatcher.AddEventListener<int>(Events.DiamondToGoldEvent.GoldMetallurgy, GoldMetallurgy);

            EventDispatcher.AddEventListener(MainUIDict.MainUIEvent.AFFECTUP, OnBattleBtnPressed);
            EventDispatcher.AddEventListener(MainUIDict.MainUIEvent.OUTPUTUP, OnBattleBtnPressed);
            EventDispatcher.AddEventListener(MainUIDict.MainUIEvent.MOVEUP, OnBattleBtnPressed);
            EventDispatcher.AddEventListener(MainUIDict.MainUIEvent.NORMALATTACK, OnBattleBtnPressed);
            EventDispatcher.AddEventListener(MainUIDict.MainUIEvent.PLAYERINFOBGUP, OnBattleBtnPressed);
            EventDispatcher.AddEventListener("MainUIControllStickPressed", OnBattleBtnPressed);

            EventDispatcher.AddEventListener<Vector3>(Events.GearEvent.CrockBroken, CrockBroken);
            EventDispatcher.AddEventListener<bool, bool, Vector3>(Events.GearEvent.ChestBroken, ChestBroken);

            EventDispatcher.AddEventListener<GameObject, Vector3, float>(MogoMotor.ON_MOVE_TO_FALSE, OnMoveToFalse);
            EventDispatcher.AddEventListener(Events.OtherEvent.BossDie, BossDie);

            EventDispatcher.AddEventListener<int, bool>(Events.InstanceEvent.InstanceLoaded, SetCampControl);

            timerID = TimerHeap.AddTimer<bool>(1000, 100, SyncPos, true);
            checkDmgID = TimerHeap.AddTimer(0, 1000, CheckDmgBase);
            syncHpTimerID = TimerHeap.AddTimer(10000, 5000, SyncHp);
            skillFailoverTimer = TimerHeap.AddTimer(1000, 3000, SkillFailover);
            TimerHeap.AddTimer(5000, 0, GetServerTickReq);
            //rateTimer = TimerHeap.AddTimer(1000, 3000, CheckRate);
            CheckCharge();
            GetWingBag();
            MogoTime.Instance.InitTimeFromServer();

            MogoUIManager.Instance.LoadUIResources();
            TimerHeap.AddTimer(500, 0, EventDispatcher.TriggerEvent, Events.RuneEvent.GetBodyRunes);
            TimerHeap.AddTimer(500, 0, marketManager.GiftRecordReq);

            if (IsNewPlayer)
            {
                CurMissionID = 10100;
                CurMissionLevel = 1;

                missionManager.EnterMissionReq(CurMissionID, CurMissionLevel);
            }


            if (PlatformSdkManager.Instance)
                TimerHeap.AddTimer(1000, 60000, PlatformSdkManager.Instance.OnSetupNotification);

            */
            MogoUIManager.Instance.LoadUIResource();
        }

        // 对象从场景中删除， 在这里释放资源
        override public void OnLeaveWorld()
        {
            /*
            EventDispatcher.RemoveEventListener(Events.UIBattleEvent.OnNormalAttack, NormalAttack);
            EventDispatcher.RemoveEventListener(Events.UIBattleEvent.OnPowerChargeStart, PowerChargeStart);
            EventDispatcher.RemoveEventListener(Events.UIBattleEvent.OnPowerChargeInterrupt, PowerChargeInterrupt);
            EventDispatcher.RemoveEventListener(Events.UIBattleEvent.OnPowerChargeComplete, PowerChargeComplete);
            EventDispatcher.RemoveEventListener(Events.UIBattleEvent.OnSpellOneAttack, SpellOneAttack);
            EventDispatcher.RemoveEventListener(Events.UIBattleEvent.OnSpellTwoAttack, SpellTwoAttack);
            EventDispatcher.RemoveEventListener(Events.UIBattleEvent.OnSpellThreeAttack, SpellThreeAttack);
            EventDispatcher.RemoveEventListener(Events.UIBattleEvent.OnSpellXPAttack, SpecialAttack);
            EventDispatcher.RemoveEventListener<int>(Events.UIBattleEvent.OnSpriteSkill, OnSpriteSkill);

            EventDispatcher.RemoveEventListener<uint>(Events.GearEvent.Teleport, Teleport);
            EventDispatcher.RemoveEventListener<uint, int, int, int>(Events.GearEvent.Damage, SetDamage);

            EventDispatcher.RemoveEventListener<int, bool>(Events.InstanceEvent.InstanceLoaded, InstanceLoaded);
            EventDispatcher.RemoveEventListener<ushort>(Events.OtherEvent.MapIdChanged, OnMapChanged);
            EventDispatcher.RemoveEventListener<ulong>(Events.OtherEvent.Withdraw, Withdraw);
            EventDispatcher.RemoveEventListener(Events.OtherEvent.DiamondMine, DiamondMine);
            EventDispatcher.RemoveEventListener(Events.OtherEvent.CheckCharge, CheckCharge);
            EventDispatcher.RemoveEventListener(Events.OtherEvent.Charge, Charge);

            EventDispatcher.RemoveEventListener(ON_TASK_GUIDE, TaskGuide);
            EventDispatcher.RemoveEventListener(ON_END_TASK_GUIDE, EndTaskGuide);
            EventDispatcher.RemoveEventListener<int, int>(ON_TASK_MISSION, MissionOpen);

            EventDispatcher.RemoveEventListener(Events.AIEvent.DummyThink, DummyThink);
            EventDispatcher.RemoveEventListener(Events.StoryEvent.CGBegin, ProcCGBegin);
            EventDispatcher.RemoveEventListener(Events.StoryEvent.CGEnd, ProcCGEnd);
            EventDispatcher.RemoveEventListener<string>(Events.GearEvent.TrapBegin, ProcTrapBegin);
            EventDispatcher.RemoveEventListener<string>(Events.GearEvent.TrapEnd, ProcTrapEnd);
            EventDispatcher.AddEventListener(Events.GearEvent.LiftEnter, ProcLiftEnter);
            EventDispatcher.RemoveEventListener<int>(Events.GearEvent.PathPointTrigger, PathPointTrigger);
            EventDispatcher.RemoveEventListener(Events.DirecterEvent.DirActive, DirActive);

            EventDispatcher.RemoveEventListener<int>(Events.EnergyEvent.BuyEnergy, BuyEnergy);
            EventDispatcher.RemoveEventListener(ON_VIP_REAL_STATE, OnVIPRealState);

            EventDispatcher.RemoveEventListener<int>(Events.DiamondToGoldEvent.GoldMetallurgy, GoldMetallurgy);

            EventDispatcher.RemoveEventListener(MainUIDict.MainUIEvent.AFFECTUP, OnBattleBtnPressed);
            EventDispatcher.RemoveEventListener(MainUIDict.MainUIEvent.OUTPUTUP, OnBattleBtnPressed);
            EventDispatcher.RemoveEventListener(MainUIDict.MainUIEvent.MOVEUP, OnBattleBtnPressed);
            EventDispatcher.RemoveEventListener(MainUIDict.MainUIEvent.NORMALATTACK, OnBattleBtnPressed);
            EventDispatcher.RemoveEventListener(MainUIDict.MainUIEvent.PLAYERINFOBGUP, OnBattleBtnPressed);
            EventDispatcher.RemoveEventListener("MainUIControllStickPressed", OnBattleBtnPressed);

            EventDispatcher.RemoveEventListener<Vector3>(Events.GearEvent.CrockBroken, CrockBroken);
            EventDispatcher.RemoveEventListener<bool, bool, Vector3>(Events.GearEvent.ChestBroken, ChestBroken);

            EventDispatcher.RemoveEventListener<GameObject, Vector3, float>(MogoMotor.ON_MOVE_TO_FALSE, OnMoveToFalse);
            EventDispatcher.RemoveEventListener(Events.OtherEvent.BossDie, BossDie);

            EventDispatcher.RemoveEventListener<int, bool>(Events.InstanceEvent.InstanceLoaded, SetCampControl);

            GuideSystem.Instance.RemoveListeners();
            StoryManager.Instance.RemoveListeners();
            missionManager.RemoveListeners();
            taskManager.RemoveListeners();
            operationSystem.RemoveListeners();

            doorOfBurySystem.RemoveListener();
            inventoryManager.RemoveListeners();
            bodyenhanceManager.RemoveListener();
            friendManager.RemoveListener();
            sanctuaryManager.RemoveListeners();
            arenaManager.RemoveListeners();
            dailyEventSystem.RemoveListeners();
            rankManager.RemoveListeners();

            campaignSystem.RemoveListeners();

            wingManager.Clean();
            rewardManager.Clean();
            runeManager.Clean();
            marketManager.Clean();
            skillManager.Clean();
            battleManger.Clean();
            MailManager.Instance.Clean();

            TimerHeap.DelTimer(timerID);

            TimerHeap.DelTimer(syncHpTimerID);
            TimerHeap.DelTimer(skillFailoverTimer);

            MogoTime.Instance.ReleaseMogoTimeData();

            MogoUIManager.Instance.ReleaseUIResources();

            TongManager.Instance.Release();
            TipManager.Instance.RemoveListener();
            */
            base.OnLeaveWorld();
        }

        override public void CreateModel()
        {
            //CreateDeafaultModel();
            CreateActualModel();
        }

        override public void CreateActualModel()
        {
            isCreatingModel = true;
            AvatarModelData data = AvatarModelData.dataMap.Get((int)Vocation.Warrior);

            AssetCacheMgr.GetInstanceAutoRelease(data.prefabName, (prefab, id, go) =>
            {
                //this.Actor.Release();
                //AssetCacheMgr.ReleaseLocalInstance(Transform.gameObject);
                var gameObject = go as GameObject;
                gameObject.tag = "Player";
                ActorMyself actor = gameObject.AddComponent<ActorMyself>();

                motor = gameObject.AddComponent<MogoMotorMyself>();
                animator = gameObject.GetComponent<Animator>();
               // sfxHandler = gameObject.AddComponent<SfxHandler>();

                audioSource = gameObject.AddComponent<AudioSource>();
                audioSource.rolloffMode = AudioRolloffMode.Custom;

                actor.mogoMotor = motor;
                actor.theEntity = this;
                GameObject = gameObject;

                Transform = gameObject.transform;

                // Debug.LogError(GameObject.name + " " + ID + " Myself CreateActualModel: " + Transform.position);

                Transform.gameObject.layer = 8;
                UpdatePosition();
//                 if (data.scale > 0)
//                 {
//                     Transform.localScale = new Vector3(data.scale, data.scale, data.scale);
//                 }

                this.Actor = actor;

                //重新穿上装备
                //actor.RemoveOld();
    //            if (InventoryManager.Instance != null)
                {
    //                foreach (ItemEquipment equip in InventoryManager.Instance.EquipOnDic.Values)
                    {
     //                   Equip(equip.templateId);
                    }
                }
                gameObject.SetActive(false);
                gameObject.SetActive(true);

                AssetCacheMgr.GetInstanceAutoRelease("RoundLight.prefab", (p, i, light) =>
                {
                    m_roundLight = light as GameObject;
                    m_roundLight.transform.parent = Transform;
                    m_roundLight.transform.localPosition = new Vector3(0, 1, 0);
                });
                try
                {
                    if (MogoMainCamera.Instance != null)
                    {
                        var slot = Mogo.Util.MogoUtils.GetChild(GameObject.transform, "slot_camera");
                        MogoMainCamera.Instance.target = slot;
                    }
                }
                catch (Exception ex)
                {
                    LoggerHelper.Except(ex);
                }
                
                isCreatingModel = false;
              //  Actor.ActChangeHandle = ActionChange;
               // UpdateDressWing();
            });

        }

        override public void CreateDeafaultModel()
        {
            AvatarModelData data = AvatarModelData.dataMap[999];

            GameObject gameObject = AssetCacheMgr.GetLocalInstance(data.prefabName) as GameObject;

            gameObject.tag = "Player";
            ActorMyself actor = gameObject.AddComponent<ActorMyself>();
            actor.isNeedInitEquip = false;
            motor = gameObject.AddComponent<MogoMotorMyself>();
            animator = gameObject.GetComponent<Animator>();
            //sfxHandler = gameObject.AddComponent<SfxHandler>();

            actor.mogoMotor = motor;
            actor.theEntity = this;
            GameObject = gameObject;
            Transform = gameObject.transform;

            // Debug.LogError(GameObject.name + " " + ID + " Myself CreateDeafaultModel: " + Transform.position);

            UpdatePosition();
            this.Actor = actor;
        }

        public override void UpdatePosition()
        {
            if (Transform)
                GameObject.layer = 8;
            base.UpdatePosition();

            if (!MogoWorld.IsClientMission)
                MogoWorld.IsClientPositionSync = true;
        }

        public override void UpdateView()
        {
            base.UpdateView();
         
        }

        public void SetLightVisible(bool isActive)
        {
            if (m_roundLight)
                m_roundLight.SetActive(isActive);
        }

        public void SetModelInfo(bool isInstance = false)
        {
            UpdatePosition();
        }

        public void NormalAttack()
        {
            if (!canCastSpell) return;
          //  ((PlayerBattleManager)battleManger).NormalAttack();
        }

        public void PowerChargeStart()
        {
            if (!canCastSpell) return;
          //  ((PlayerBattleManager)battleManger).PowerChargeStart();
        }

        public void PowerChargeInterrupt()
        {
         //   ((PlayerBattleManager)battleManger).PowerChargeInterrupt();
        }

        public void PowerChargeComplete()
        {
         //   ((PlayerBattleManager)battleManger).PowerChargeComplete();
        }

        public void SpellOneAttack()
        {
            if (!canCastSpell) return;
          //  ((PlayerBattleManager)battleManger).SpellOneAttack();
        }

        public void SpellTwoAttack()
        {
            if (!canCastSpell) return;
          //  ((PlayerBattleManager)battleManger).SpellTwoAttack();
        }

        public void SpellThreeAttack()
        {
            if (!canCastSpell) return;
           // ((PlayerBattleManager)battleManger).SpellThreeAttack();
        }

        public void SpecialAttack()
        {
          //  EnterAngerSt();
        }

        /// <summary>
        /// 通过手势施放精灵技能
        /// </summary>
        /// <param name="gestureNameEnum"></param>
        public void OnSpriteSkill(int gestureNameEnum)
        {
            LoggerHelper.Debug(gestureNameEnum);
        }

        public void UpdateSkillToManager()
        {
            object skills = null;
            ObjectAttrs.TryGetValue("skillBag", out skills);
            if (skills == null)
            {
                return;
            }
        }

        public void Revive()
        {

        }

        private void Stand()
        {
            if (MogoWorld.inCity)
            {
                TimerHeap.AddTimer(500, 0, SetAction, -1);
            }
            motor.enableStick = true;
        }

//         public override void OnMoveTo(GameObject arg1, Vector3 arg2)
//         {
// 
//         }


        #endregion

        #region 私有方法

        private GameObject flag;

        private uint DmgBase = 0;

        public void CalcuDmgBase()
        {
            //DmgBase = curHp + def + atk;
        }


        private uint preHp = 0;

        // 为去除警告暂时屏蔽以下代码
        //private ulong clientChkPreTime = 0;
        //private ulong biosChkPreTime = 0;
        private void CheckRate()
        {
            return;
        }


        public void EndTaskGuide()
        {
           // ChangeMotionState(MotionState.IDLE);
        }

        private void MissionOpen(int theMission, int theLevel)
        {
            //missionManager.MissionOpen(theMission, theLevel);
        }

        private void CrockBroken(Vector3 position)
        {
            ContainerBroken(6, position);
        }

        private void ChestBroken(bool easyType, bool hardType, Vector3 position)
        {
            bool judgementType = false;
            switch (curMissionLevel)
            {
                case 1:
                    judgementType = easyType;
                    break;
                default:
                    judgementType = hardType;
                    break;
            }

            if (judgementType)
                ContainerBroken(8, position);
            else
                ContainerBroken(7, position);
        }

        private void ContainerBroken(int type, Vector3 position)
        {
           // missionManager.ContainerBroken(type, position);
        }

        private void GetWingBag()
        {
            RpcCall("SyncWingBagReq");
        }

        private void Withdraw(ulong ord_dbid)
        {
            RpcCall("WithdrawReq", ord_dbid);
        }

        private void DiamondMine()
        {
            RpcCall("DiamondMineReq");
        }

        private void CheckCharge()
        {
            RpcCall("CheckChargeReq");
        }

        private void Charge()
        {
        }

        private void OnMapChanged(ushort mapId)
        {
            UpdatePosition();
        }

        private void InstanceLoaded(int missionID, bool isInstance)
        {
            UpdatePosition();
            UpdateView();

            if (motor != null)
            {

                //(motor as MogoMotorMyself).SwitchSetting(false);
            }
            else
            {
                Mogo.Util.LoggerHelper.Debug("motor == null");
            }

            isInCity = !isInstance;
            if (isInCity)
            {
                //检测指引消失/出现
                EventDispatcher.TriggerEvent(Events.DirecterEvent.DirActive);

                
            }
            //if (isInCity)
            //{
            //    SetAction(-1);
            //}
            //else
            //{
            //    SetAction(0);
            //}

            LoggerHelper.Debug("JewlMailReq");
            TimerHeap.AddTimer(2000, 0, SendJewlMailReq);
        }

        public void SendJewlMailReq()
        {
            RpcCall("JewlMailReq");
        }

        override public void MainCameraCompleted()
        {
            base.MainCameraCompleted();
            //BillboardLogicManager.Instance.AddInfoBillboard(ID, Transform, false, this);
            //BillboardLogicManager.Instance.SetHead(this);
            //EventDispatcher.TriggerEvent<string, uint>
            //     (BillboardLogicManager.BillboardLogicEvent.UPDATEBILLBOARDNAME, name, ID);//Base里面已经加过 先注释掉这里 MaiFeo

            //MogoFXManager.Instance.AddShadow(Transform.gameObject, ID);
        }

        protected void Teleport(uint targetSceneId)
        {
            LoggerHelper.Debug("Teleporting");
            RpcCall("EnterTeleportpointReq", (UInt32)targetSceneId);
        }
        #endregion
    }
}