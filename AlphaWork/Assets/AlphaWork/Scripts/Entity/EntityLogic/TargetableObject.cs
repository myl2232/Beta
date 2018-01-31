﻿using GameFramework;
using System.Collections;
using UnityEngine;

namespace AlphaWork
{
    /// <summary>
    /// 可作为目标的实体类。
    /// </summary>
    public abstract class TargetableObject : CustomEntity
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

        //public abstract ImpactData GetImpactData();

        public void ApplyDamage(int damageHP)
        {
            float fromHPRatio = m_TargetableObjectData.HPRatio;
            m_TargetableObjectData.HP -= damageHP;
            float toHPRatio = m_TargetableObjectData.HPRatio;
            if (fromHPRatio > toHPRatio)
            {
                //GameEntry.HPBar.ShowHPBar(this, fromHPRatio, toHPRatio);
            }

            OnHurt();

            if (m_TargetableObjectData.HP <= 0)
            {
                OnDead();
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
            //记录各个角色表中的属性
            BaseCharacter chr = gameObject.GetOrAddComponent<BaseCharacter>();
            TargetableObjectData data = userData as TargetableObjectData;
            chr.walkSpeed = data.walkSpeed;
            chr.runSpeed = data.runSpeed;
            chr.sprintSpeed = data.sprintSpeed;
            chr.baseSpeed = data.baseSpeed;

            WeaponAttachment[] attaches = GetComponentsInChildren<WeaponAttachment>();
            for(int i = 0; i < attaches.Length; ++i)
            {
                attaches[i].ParentId = Id;
            }

#if UNITY_EDITOR
            aiAdd = transform.position;
#endif
            gameObject.layer = Constant.Layer.TargetableObjectLayerId;
        }

        protected virtual void OnDead()
        {
            BaseCharacter chr = GetComponentInParent<BaseCharacter>();
            if (chr != null)
            {
                PauseMove();
                chr.ActionDead();
            }
        }

        protected virtual void OnHurt()
        {
            BaseCharacter chr = GetComponentInParent<BaseCharacter>();
            if (chr != null)
            {
                PauseMove();
                chr.ActionHurt();
            }
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
        }

        public virtual void PauseMove()
        { }

#if UNITY_EDITOR
        private Vector3 aiAdd = new Vector3();
        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawCube(aiAdd, new Vector3(1, 1, 1));
        }
#endif

    }
}
