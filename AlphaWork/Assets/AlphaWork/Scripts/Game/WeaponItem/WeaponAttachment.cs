using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
            
        }

    }
}

