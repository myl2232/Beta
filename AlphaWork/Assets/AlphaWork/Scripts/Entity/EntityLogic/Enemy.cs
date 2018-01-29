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

        private MoveTarget m_moveTarget;
        private BehaviacTrigger m_trigger;
        //private Vector3 m_nextTarget;
        private Vector3 m_MoveStartPos = new Vector3();

        private float m_lastTime;
        // Use this for initialization
        void Start()
        {  
            m_lastTime = 0;

            BaseCharacter chr = gameObject.GetComponent<BaseCharacter>();
            if (chr)
                chr.ParentId = Id;

            //ai
            NPCData data = Data as NPCData;
            if(data != null)
            {
                m_agent = new EnemyAgent();
                m_agent.btload(data.AI);
                m_agent.btsetcurrent(data.AI);
                m_agent.ParentId = Id;
                m_agent.InitAI();
            }
            m_trigger = gameObject.GetOrAddComponent<BehaviacTrigger>();
            m_trigger.Parent = Entity.Logic as TargetableObject;
            gameObject.GetOrAddComponent<BehaviourShakeHit>();
            m_moveTarget = gameObject.GetOrAddComponent<MoveTarget>();
            GameEntry.Event.Subscribe(MoveToTargetEventArgs.EventId, OnMoveToTarget);
        }

        // Update is called once per frame
        void Update()
        {
            //ai
            if (m_agent == null)
                return;

            behaviac.EBTStatus status = behaviac.EBTStatus.BT_RUNNING;
            while ((status == behaviac.EBTStatus.BT_RUNNING) && (m_agent != null)
                && (Time.realtimeSinceStartup - m_lastTime > 0.3))
            {
                status = m_agent.btexec();
                m_lastTime = Time.realtimeSinceStartup;
                m_agent._set_bAwakeSense(true);
            }
        }

        public void SetSpeed(float speed)
        {
            m_moveTarget.Speed = speed;
        }
        public void PauseMove()
        {
            m_moveTarget.Pause();
        }
        protected internal override void OnShow(object userdata)
        {
            base.OnShow(userdata);  
        }

        protected override void OnDead()
        {
            GetComponentInParent<BaseCharacter>().ActionDead();
        }
        protected override void OnHurt()
        {
            GetComponentInParent<BaseCharacter>().ActionHurt();
        }

        #region 
        /*移动调用流程*/
       
        public void MoveToTarget()
        {
            FaceToTarget();
            GameObject gb = Entity.Handle as GameObject;
            if (m_nextTarget.x == 0 || Vector3.Distance(gb.transform.position, m_nextTarget) < 0.5f)
                m_nextTarget = GetTargetPos();

            GameEntry.Event.Fire(this, new MoveToTargetEventArgs(Id, m_nextTarget));
        }

        protected void FaceToTarget()
        {
            GameObject gb = Entity.Handle as GameObject;
            gb.transform.forward = m_nextTarget - gb.transform.position;
        }

        protected void OnMoveToTarget(object sender, GameEventArgs e)
        {
            MoveToTargetEventArgs mvArgs = e as MoveToTargetEventArgs;
            if (mvArgs.EId == Id)
            {
                GameObject gb = Entity.Handle as GameObject;
                m_MoveStartPos = gb.transform.position;

                if (m_moveTarget)
                    m_moveTarget.Move(m_MoveStartPos, mvArgs.MovePos);
            }
        }

        protected Vector3 GetTargetPos()
        {
            UnityGameFramework.Runtime.Entity etEnemy = GameEntry.Entity.GetEntity(m_agent.SenseResult);
            if (etEnemy != null)
            {
                return etEnemy.transform.position;
            }
            else
            {
                GameObject target = new GameObject();
                GameEntry.Behaviac.GetNextTarget(transform.position, ref target);
                return target.transform.position;
            }
        }

        #endregion

        //test from animation event
        //public void AttackSkill01()
        //{
        //    int attId = GameEntry.Entity.GenerateSerialId();
        //    Vector3 vDir = GameEntry.Entity.GetEntity(Agent.SenseResult).transform.position - transform.position;
        //    GameEntry.Entity.ShowEffect(new EffectData(vDir, transform, attId, 60001)
        //    {

        //    },1);
        //}
    }
}
