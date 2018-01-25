using System;
using UnityEngine;

namespace AlphaWork
{
    [Serializable]
    public abstract class TargetableObjectData : EntityData
    {
        [SerializeField]
        private CampType m_Camp = CampType.Unknown;

        [SerializeField]
        private int m_HP = 0;
        [SerializeField]
        protected float m_SenseRadius = 10;
        [SerializeField]
        protected float m_attackRadius = 5;

        public TargetableObjectData(int entityId, int typeId, CampType camp)
            : base(entityId, typeId)
        {
            m_Camp = camp;
            m_HP = 0;
        }

        /// <summary>
        /// 角色阵营。
        /// </summary>
        public CampType Camp
        {
            get
            {
                return m_Camp;
            }
        }

        /// <summary>
        /// 当前生命。
        /// </summary>
        public int HP
        {
            get
            {
                return m_HP;
            }
            set
            {
                m_HP = value;
            }
        }

        /// <summary>
        /// 最大生命。
        /// </summary>
        public int MaxHP
        {
            get;set;
        }
        public float SenseRadius
        {
            get { return m_SenseRadius; }
            set { m_SenseRadius = value; }
        }
        public float AttackRadius
        {
            get { return m_attackRadius; }
            set { m_attackRadius = value; }
        }
        /// <summary>
        /// AI行为树。
        /// </summary>
        public string AI;
        public float walkSpeed;
        public float runSpeed;
        public float sprintSpeed;
        /// <summary>
        /// 生命百分比。
        /// </summary>
        public float HPRatio
        {
            get
            {
                return MaxHP > 0 ? (float)HP / MaxHP : 0f;
            }
        }
    }
}
