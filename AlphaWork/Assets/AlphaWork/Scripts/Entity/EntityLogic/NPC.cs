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
        }

        // Update is called once per frame
        void Update()
        {            
            
        }
        

        protected internal override void OnShow(object userdata)
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

            //GameObject gb = GameEntry.Entity.GetEntity(Id).Handle as GameObject;
            
        }
    }
}
