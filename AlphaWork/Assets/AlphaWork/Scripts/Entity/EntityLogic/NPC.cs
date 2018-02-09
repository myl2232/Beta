using GameFramework.Event;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.AI;

namespace AlphaWork
{
    public class NPC: TargetableObject
    {
        protected Vector3 lockPos;
        protected NPCData m_data = null;
        protected float m_TimeSpawn;
        protected float m_RecordTime;
       
        // Use this for initialization
        void Start()
        {
            BaseCharacter chr = gameObject.GetComponent<BaseCharacter>();
            if(chr)
                chr.ParentId = Id;
        }

        // Update is called once per frame
        void Update()
        {            
            
        }

#if UNITY_2017_3_OR_NEWER
        protected override void OnShow(object userdata)
#else
        protected internal override void OnShow(object userdata)
#endif
        {
            base.OnShow(userdata);
            m_data = userdata as NPCData;

            CachedTransform.position = m_data.Position;

            SphereCollider sp = GetComponent<SphereCollider>();
            if(sp)
            {
                CachedTransform.position += new Vector3(0, sp.radius, 0);
            }
     
            m_TimeSpawn = Time.realtimeSinceStartup;
                        
        }
    }
}
