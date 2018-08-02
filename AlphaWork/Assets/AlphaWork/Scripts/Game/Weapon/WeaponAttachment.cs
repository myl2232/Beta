using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityGameFramework.Runtime;

namespace AlphaWork
{
    public class WeaponAttachment : MonoBehaviour
    {
        protected bool bActive;
        public bool Active
        {
            get { return bActive; }
            set { bActive = value; }
        }

        public int hitHP;
        public int ParentId;
        
        // Use this for initialization
        void Start()
        {
            gameObject.layer = Constant.Layer.WeaponLayerId;
        }

        // Update is called once per frame
        void Update()
        {
           
        }

        void OnEnable()
        {
            
        }

        private void OnTriggerEnter(Collider other)
        {
            BehaviourShakeHit hitCom = other.GetComponentInParent<BehaviourShakeHit>();

            BaseCharacter chrOther = other.GetComponentInParent<BaseCharacter>();
            if (chrOther == null || ParentId == chrOther.ParentId)
                return;
            Entity otherEt = other.GetComponentInParent<Entity>();
            TargetableObject parentEt = GameEntry.Entity.GetEntity(ParentId).Logic as TargetableObject;
            if (hitCom && parentEt.IsDefeat(otherEt.Id))
                hitCom.OnHit(hitHP);
        }

    }
}

