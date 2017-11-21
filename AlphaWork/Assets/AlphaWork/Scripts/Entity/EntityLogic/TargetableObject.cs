using GameFramework;
using UnityEngine;

namespace AlphaWork
{
    /// <summary>
    /// 可作为目标的实体类。
    /// </summary>
    public abstract class TargetableObject : EntityObject
    {
        [SerializeField]
        private TargetableObjectData m_TargetableObjectData = null;

        public bool IsDead
        {
            get
            {
                return m_TargetableObjectData.HP <= 0;
            }
        }

        public abstract ImpactData GetImpactData();

        public void ApplyDamage(EntityObject attacker, int damageHP)
        {
            float fromHPRatio = m_TargetableObjectData.HPRatio;
            m_TargetableObjectData.HP -= damageHP;
            float toHPRatio = m_TargetableObjectData.HPRatio;
            if (fromHPRatio > toHPRatio)
            {
                //GameEntry.HPBar.ShowHPBar(this, fromHPRatio, toHPRatio);
            }

            if (m_TargetableObjectData.HP <= 0)
            {
                OnDead(attacker);
            }
        }

        protected internal override void OnInit(object userData)
        {
            base.OnInit(userData);
            CachedTransform.SetLayerRecursively(Constant.Layer.TargetableObjectLayerId);
        }

        protected internal override void OnShow(object userData)
        {
            base.OnShow(userData);

            m_TargetableObjectData = userData as TargetableObjectData;
            if (m_TargetableObjectData == null)
            {
                Log.Error("Targetable object data is invalid.");
                return;
            }
        }

        protected virtual void OnDead(EntityObject attacker)
        {
            GameEntry.Entity.HideEntity(this);
        }

        private void OnTriggerEnter(Collider other)
        {
            EntityObject entity = other.gameObject.GetComponent<EntityObject>();
            if (entity == null)
            {
                return;
            }

            if (entity is TargetableObject && entity.Id >= Id)
            {
                // 碰撞事件由 Id 小的一方处理，避免重复处理
                return;
            }

            //AIUtility.PerformCollision(this, entity);
        }
    }
}
