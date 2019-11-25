using UnityEngine;
using System;
using System.Collections.Generic;

//using Mogo.FSM;
using Mogo.GameData;
using Mogo.Util;
using Mogo.RPC;
//using Mogo.Task;

namespace Mogo.Game
{
    public partial class EntityParent
    {
        #region 公共变量
        public bool stiff = false;
        public bool knockDown = false;
        public bool hitAir = false;
        public bool hitGround = false;
        public Quaternion preQuaternion;
        public bool charging = false;

        public uint hitTimerID = 0;
        public uint revertHitTimerID = 0;
        public uint delayAttackTimerID = 0;
        public bool breakAble = true;

        #region 属性部分也许可以放到下一层子类
        public UInt32 ID = 0;
        public UInt64 dbid = 0;

        public const string ATTR_NAME = "name";
        public const string ATTR_EXP = "PercentageExp";
        public const string ATTR_ENERGY = "PercentageEnergy";
        public const string ATTR_LEVEL = "level";
        public const string ATTR_VIP_LEVEL = "VipLevel";
        public const string ATTR_HP_COUNT = "hpCount";
        public const string ATTR_CHARGE_SUM = "chargeSum";
        public const string ATTR_HP = "PercentageHp";
        public const string ATTR_HPSTRING = "HpString";
        public const string ATTR_ENERGYSTRING = "EnergyString";
        public const string ATTR_SPEED = "speed";
        public const string ATTR_HEAD_ICON = "HeadIcon";
        public const string ATTR_ARENIC_GRADE = "arenicGrade";
        public const string ATTR_ARENIC_CREDIT = "arenicCredit";
        public const string ATTR_FIGHT_FORCE = "fightForce";
        public const string ATTR_GOLD_METALLURGY_LAST_TIMES = "goldMetallurgyLastTimes";
        public const string ATTR_GLOD = "gold";
        public const string ATTR_DIAMOND = "diamond";
        public const int DIZZY_STATE = 1 << 1;
        public const int IMMOBILIZE_STATE = 1 << 3;
        public const int SILENT_STATE = 1 << 4;
        public const int IMMUNITY_STATE = 1 << 10;
        public const int NO_HIT_STATE = 1 << 11;

        /// <summary>
        /// 用于特殊道具是否已经初始化用，继而用于统一显示获得某个物品
        /// </summary>
        protected HashSet<int> m_propHasInitSet = new HashSet<int>();

        private string m_name;
        private uint m_exp;
        private uint m_nextLevelExp = 1;
        private uint m_energy;
        private uint m_maxEnergy;
        private byte m_level;
        private byte m_VipLevel;
        private byte m_buyCount;
        private ushort m_arenicGrade;
        private uint m_fightForce;
        private uint m_arenicCredit;
        private byte m_hpCount;
        private uint m_chargeSum;
        protected uint m_curHp;
        protected uint m_difficulty;
        private uint m_hp = 1;
        private float m_speed;
        public int deviator = 0;

        protected ulong m_stateFlag = 0;
        public bool canCastSpell = true;
        public bool canBeHit = true;
        public bool direction = false; //移动是否反方向
        public float moveSpeedRate = 1; //移动速率
        public bool canMove = true; //能否移动
        public bool immuneShift = false;//是否免疫受击位移等不受控制状态(眩晕，定身，魅惑)

        public float gearMoveSpeedRate = 1;

        public byte m_factionFlag = 0;
        public UInt32 m_ownerEid = 0;

        public float aiRate = 1;

        public int angerStep = 0;

        public int m_spawnPointCfgId;//所在出生点配置ID

        public int m_currentSee = 0;//当前视野范围

        private Vocation m_vocation;
        public Vocation vocation
        {
            get { return m_vocation; }
            set 
            {
                m_vocation = value;
                //OnPropertyChanged(ATTR_HEAD_ICON, HeadIcon);
            }
        }


        #region 装备同步
        protected uint m_loadedWeapon;
        protected uint m_loadedCuirass;
        protected uint m_loadedArmguard;
        protected uint m_loadedLeg;
        #endregion 装备同步


        /// <summary>
        /// 当前经验
        /// </summary>



        /// <summary>
        /// 当前体力
        /// </summary>



        /// <summary>
        /// 回调方法缓存
        /// </summary>
        private List<KeyValuePair<string, Action<object[]>>> m_respMethods = new List<KeyValuePair<string, Action<object[]>>>();

        #endregion

        /// <summary>
        /// 实体定义名。
        /// </summary>
        public string entityType;

        public Mogo.RPC.EntityDef entity;
        public bool isClientFirst = true;
        public Animator weaponAnimator = null;
        public Transform Transform { get; set; }
        public GameObject GameObject { get; set; }
        public Vector3 position = Vector3.zero;
        public Vector3 rotation = Vector3.zero;
        public Vector3 scale = new Vector3(1, 1, 1);
        public ActorParent Actor { get; set; }
        public Animator animator;
        public MogoMotor motor;
        //public SfxHandler sfxHandler;

        public AudioSource audioSource;

        protected bool isSrcSpeed = true;
        protected float srcSpeed;

       // public AI.BTBlackBoard blackBoard = new AI.BTBlackBoard();

        public bool isCloseMotionBlur = false;

        public short m_iEnterX;
        public short m_iEnterZ;

        //public bool hasCache = false; //是否有缓存的数据未刷新显示



        virtual public int hitStateID
        {
            get { return 0; }
        }

        public int spawnPointCfgId
        {
            get { return m_spawnPointCfgId; }
            set { m_spawnPointCfgId = value; }
        }

        public short enterX
        {
            get { return m_iEnterX; }
            set
            {
                m_iEnterX = value;
            }
        }

        public short enterZ
        {
            get { return m_iEnterZ; }
            set
            {
                m_iEnterZ = value;
            }
        }

        public int currentSee
        {
            get { return m_currentSee; }
            set
            {
                m_currentSee = value;
            }
        }

        #endregion

        #region 私有变量
        public const float EntityColiderHeight = 1.5f;
//         public SkillManager skillManager;
//         protected BattleManager battleManger;
//         protected SfxManager sfxManager;
//         protected BuffManager buffManager;
// 
//         protected FSMMotion fsmMotion = new FSMMotion();
//         protected string currentMotionState = MotionState.IDLE;

        private Dictionary<string, object> objectAttrs = new Dictionary<string, object>();
        private Dictionary<string, int> intAttrs = new Dictionary<string, int>();
        private Dictionary<string, double> doubleAttrs = new Dictionary<string, double>();
        private Dictionary<string, string> stringAttrs = new Dictionary<string, string>();

        public Dictionary<string, object> ObjectAttrs
        {
            get { return objectAttrs; }
            set { objectAttrs = value; }
        }

        public Dictionary<string, int> IntAttrs
        {
            get { return intAttrs; }
            set { intAttrs = value; }
        }

        public Dictionary<string, double> DoubleAttrs
        {
            get { return doubleAttrs; }
            set { doubleAttrs = value; }
        }

        public Dictionary<string, string> StringAttrs
        {
            get { return stringAttrs; }
            set { stringAttrs = value; }
        }
        private readonly static HashSet<TypeCode> m_intSet = new HashSet<TypeCode>() { TypeCode.SByte, TypeCode.Byte, TypeCode.Int16, TypeCode.UInt16, TypeCode.Int32 };
        private readonly static HashSet<TypeCode> m_doubleSet = new HashSet<TypeCode>() { TypeCode.UInt32, TypeCode.Int64, TypeCode.UInt64, TypeCode.Single, TypeCode.Double };

        public int currSpellID = -1; //小于等于0为当前空闲
        public int hitActionIdx = 0; //当前技能hitAction索引
        public int currHitAction = -1; //当前hitAction ID
        public List<uint> hitTimer = new List<uint>(); //技能hit的timer id
        public bool walkingCastSkill = false; //跑动中使用技能
        public string skillActName = ""; //当前技能动作名
        protected bool hadSyncProp = false; //同步完初始化属性
        #endregion
    }
}
