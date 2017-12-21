using GameFramework.Event;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.AI;

namespace AlphaWork
{
    public class NPC: EntityObject
    {
        protected Vector3 lockPos;
        protected NPCData m_data = null;
        protected float m_TimeSpawn;
        protected float m_RecordTime;
        protected bool m_AIGo;
        // Use this for initialization
        void Start()
        {
            GameEntry.Event.Subscribe(UIBetaEventArgs.EventId, OnAIGo);
        }

        // Update is called once per frame
        void Update()
        {            
            if(Time.realtimeSinceStartup - m_RecordTime > 5)
            {
                GameObject gb = GameEntry.Entity.GetEntity(Id).Handle as GameObject;
                if (gb && m_AIGo)
                {
                    gb.GetComponent<MoveTarget>().SetTarget(
                        new Vector3(UnityEngine.Random.Range(10, 100), 
                        0,
                        UnityEngine.Random.Range(10, 100)) );

//                     NavMeshAgent agent = gb.GetComponent<NavMeshAgent>();
//                     agent.enabled = true;
//                     gb.SetActive(true);
//                     agent.destination = new Vector3(UnityEngine.Random.Range(10, 100), 0, 
//                         UnityEngine.Random.Range(10, 100));
                }
                m_RecordTime = Time.realtimeSinceStartup;
            }
            
        }

        public void OnAIGo(object sender, GameEventArgs e)
        {
            m_AIGo = !m_AIGo;
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
