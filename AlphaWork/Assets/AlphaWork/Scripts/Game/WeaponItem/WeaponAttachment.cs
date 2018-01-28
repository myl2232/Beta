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
            
            if (hitCom)
                hitCom.OnHit(hitHP);
        }

    }
}

