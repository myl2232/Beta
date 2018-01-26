using GameFramework.Event;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.AI;

namespace AlphaWork
{
    public class Enemy: NPC
    {
        private EnemyAgent m_agent;//agent for behaviour tree
        public EnemyAgent Agent
        {
            get { return m_agent; }
        }
        private float m_lastTime;
        // Use this for initialization
        void Start()
        {
            m_lastTime = 0;
            //ai
            m_agent = new EnemyAgent();
            m_agent.btload("EnemyAvatar");
            m_agent.btsetcurrent("EnemyAvatar");
            m_agent.ParentId = Id;
            m_agent._set_senseRadius((Data as NPCData).SenseRadius);
            m_agent._set_attackRadius((Data as NPCData).AttackRadius);
            m_agent.InitAI();
        }

        // Update is called once per frame
        void Update()
        {
            //ai
            behaviac.EBTStatus status = behaviac.EBTStatus.BT_RUNNING;
            while ((status == behaviac.EBTStatus.BT_RUNNING) && (m_agent != null)
                && (Time.realtimeSinceStartup - m_lastTime > 0.3))
            {
                status = m_agent.btexec();
                m_lastTime = Time.realtimeSinceStartup;
                m_agent._set_bAwakeSense(true);
            }
        }

        protected internal override void OnShow(object userdata)
        {
            base.OnShow(userdata);

            GameObject gb = Entity.Handle as GameObject;
            RPGCharacterControllerFREE ctl = gb.GetComponent<RPGCharacterControllerFREE>();
            if (ctl)
            {
                ctl.sceneCamera = Camera.main;
            }
            
        }

        public void PauseMove()
        {
        }

        public void AttackSkill01()
        {
            int attId = GameEntry.Entity.GenerateSerialId();
            Vector3 vDir = GameEntry.Entity.GetEntity(Agent.SenseResult).transform.position - transform.position;
            GameEntry.Entity.ShowEffect(new EffectData(vDir, transform, attId, 60001)
            {
               
            },1);
        }

    }
}
