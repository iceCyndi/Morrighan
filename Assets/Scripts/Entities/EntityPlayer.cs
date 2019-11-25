/*----------------------------------------------------------------
// Copyright (C) 2013 广州，爱游
//
// 模块名：EntityPlayer
// 创建者：Steven Yang
// 修改者列表：
// 创建日期：2013.1.31
// 模块描述：玩家对象
//----------------------------------------------------------------*/

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//using Mogo.FSM;
using Mogo.Util;
using Mogo.GameData;

namespace Mogo.Game
{
    public class EntityPlayer : EntityParent
    {
        private uint idleTimer = 0;

  

        private byte m_deathFlag;
        public byte deathFlag
        {
            get
            {
                return m_deathFlag;
            }
            set
            {
                m_deathFlag = value;
                if (m_deathFlag > 0)
                {
                   // this.OnDeath(-1);
                    //GuideSystem.Instance.TriggerEvent(GlobalEvents.Death);
                }
            }
        }

        public byte factionFlag
        {
            get 
            {
                return m_factionFlag;
            }

            set
            {
                m_factionFlag = value;

                if (MogoWorld.thePlayer != null)
                    return;

                if (!Transform)
                    return;

                if (!GameObject)
                    return;

                //SetBillBoard();
            }
        }

        private uint m_loadedWingId;
        public uint loadedWingId
        {
            get
            {
                return m_loadedWingId;
            }
            set
            {
                m_loadedWingId = value;
                //UpdateDressWing();
            }
        }

        /// <summary>
        /// 宝石特效ID
        /// </summary>
        private uint m_loadedJewelId;

        public uint loadedJewelId
        {
            get { return m_loadedJewelId; }
            set
            {
                m_loadedJewelId = value;
                if (GameObject != null)
                {
                  
                }
            }
        }

        /// <summary>
        /// 装备特效ID
        /// </summary>
        private uint m_loadedEquipId;

        public uint loadedEquipId
        {
            get { return m_loadedEquipId; }
            set
            {
                m_loadedEquipId = value;
                if (GameObject != null)
                {
                   
                }
            }
        }

        /// <summary>
        /// 强化特效ID
        /// </summary>
        private uint m_loadedStrengthenId;

        public uint loadedStrengthenId
        {
            get { return m_loadedStrengthenId; }
            set
            {
                m_loadedStrengthenId = value;
                if (GameObject != null)
                {
     
                }
            }
        }

        public void Revive()
        {

            SetSpeed(0);
            motor.SetSpeed(0);
            TimerHeap.AddTimer(500, 0, Stand);
        }

        private void Stand()
        {
            if (!GameObject)
                return;
            if (MogoWorld.inCity)
            {
                TimerHeap.AddTimer(500, 0, SetAction, -1);
            }
            motor.enableStick = true;
        }

        protected Dictionary<string, List<int>> spells = new Dictionary<string, List<int>>();

        public EntityPlayer()
        {
            //spellManager = new SpellManager(this); 不能在这里初始化，要在EnterWorld
            //battleManger = new BattleManager(this);
            //sfxManager = new SfxManager(this);
        }

        override public void CreateModel()
        {
            //CreateDeafaultModel();
            CreateActualModel();
        }

        public override void CreateActualModel()
        {
           
        }


        private int checkCnt = 0;
        private int correctCnt = 0;
        private void DelayCheckIdle()
        {
            if (!MogoWorld.inCity)
            {
                return;
            }
            if (checkCnt > 11)
            {
                TimerHeap.DelTimer(idleTimer);
                return;
            }
            if (animator == null)
            {
                idleTimer = TimerHeap.AddTimer(1000, 0, DelayCheckIdle);
                checkCnt++;
                return;
            }
            int act = animator.GetInteger("Action");
            if (act == 0)
            {
                SetAction(-1);
                idleTimer = TimerHeap.AddTimer(1000, 0, DelayCheckIdle);
                checkCnt++;
                return;
            }
            if (act == -1)
            {
                correctCnt++;
                if (correctCnt > 2)
                {
                    TimerHeap.DelTimer(idleTimer);
                    return;
                }
            }
            idleTimer = TimerHeap.AddTimer(1000, 0, DelayCheckIdle);
            checkCnt++;
        }

        private void LoadEquip()
        {
            if (m_loadedCuirass != 0)
            {
             //   Equip((int)m_loadedCuirass);
            }

            if (m_loadedArmguard != 0)
            {
              ///  Equip((int)m_loadedArmguard);
            }

            if (m_loadedLeg != 0)
            {
               // Equip((int)m_loadedLeg);
            }

        }

        public override void CreateDeafaultModel()
        {
            AvatarModelData data = AvatarModelData.dataMap[999];
            GameObject gameObject = AssetCacheMgr.GetLocalInstance(data.prefabName) as GameObject;

            ActorPlayer actor = gameObject.AddComponent<ActorPlayer>();
//             motor = gameObject.AddComponent<MogoMotorServer>();
//             animator = gameObject.GetComponent<Animator>();
// 
//             sfxHandler = gameObject.AddComponent<SfxHandler>();

            // GearEffectListener作用已经交由ActorMyself处理，不需要增加这个脚本了
            // gameObject.AddComponent<GearEffectListener>();

            //actor.mogoMotor = motor;
            actor.theEntity = this;
            GameObject = gameObject;
            Transform = gameObject.transform;

            Debug.LogError(GameObject.name + " " + ID + " Player CreateDeafaultModel: " + Transform.position);

            this.Actor = actor;
            Transform.localScale = scale;
            Transform.tag = "OtherPlayer";
            UpdatePosition();
            base.CreateModel();

        }

        //private void SceneLoaded(int sceneId, bool isInstance)
        //{
        //    CreateModel();
        //}

        // 对象进入场景，在这里初始化各种数据， 资源， 模型等
        // 传入数据。
        override public void OnEnterWorld()
        {
            // todo: 这里会加入数据解析

            base.OnEnterWorld();
   
        }


        // 对象从场景中删除， 在这里释放资源
        override public void OnLeaveWorld()
        {
            // todo: 这里会释放资源
            correctCnt = 0;
            checkCnt = 0;
            TimerHeap.DelTimer(idleTimer);
            if (Actor)
                Actor.ActChangeHandle = null;
            base.OnLeaveWorld();

        }

        override public void MainCameraCompleted()
        {
            base.MainCameraCompleted();
          // SetBillBoard();
          //  BillboardLogicManager.Instance.SetHead(this);
            //EventDispatcher.TriggerEvent<float, uint>
            //      (BillboardLogicManager.BillboardLogicEvent.SETBILLBOARDBLOOD, PercentageHp, ID);
            //EventDispatcher.TriggerEvent<string, uint>
            //     (BillboardLogicManager.BillboardLogicEvent.UPDATEBILLBOARDNAME, name, ID);
        }


        public void OnPowerCharge()
        {
           // ChangeMotionState(MotionState.CHARGING);
        }

        public void OnPowerChargeInterrupt()
        {
           // ChangeMotionState(MotionState.IDLE);
        }

        public virtual void FixedErr()
        {//修正错误状态，用于回城容
            if (!MogoWorld.inCity)
            {
                return;
            }
            if (animator == null)
            {
                return;
            }
            int act = animator.GetInteger("Action");
            if (act == 0)
            {
                SetAction(-1);
            }
        }



    }
}