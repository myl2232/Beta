using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace AlphaWork
{
    public class NPCData : TargetableObjectData
    {        
        [SerializeField]
        private int m_MaxHP = 0;

        public NPCData(int entityId = -1, int typeId = -1, CampType camp = CampType.Enemy)
            : base(entityId, typeId, camp)
        {
            
        }
        // Use this for initialization
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }
        
    }
}
