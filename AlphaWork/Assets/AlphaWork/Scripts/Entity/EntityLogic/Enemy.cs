using GameFramework.Event;
using System;
using System.Collections;
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
        private BehaviacTrigger m_trigger;
                
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
            CapsuleCollider cp = gameObject.GetComponent<CapsuleCollider>();
            if(cp)
                cp.isTrigger = true;
            //GameEntry.Event.Subscribe(MoveToTargetEventArgs.EventId, OnMoveToTarget);
            //GameEntry.Event.Subscribe(MoveToTargetEndEventArgs.EventId, OnMoveToEnd);
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
        public override void PauseMove()
        {
            m_moveTarget.Pause();
        }

#if UNITY_EDITOR
        private void OnGUI()
        {
            BaseCharacter chr = gameObject.GetComponent<BaseCharacter>();
            GUI.color = Color.red;
            GUI.Label(new Rect(Screen.width * 0.5f, Screen.height * 0.5f, 100, 100), chr.m_status.ToString());
        }
#endif

#if UNITY_2017_3_OR_NEWER
        protected override void OnShow(object userdata)
#else
        protected internal override void OnShow(object userdata)
#endif
        {
            base.OnShow(userdata);

            AddDefeatCamp(CampType.Player);
            AddDefeatCamp(CampType.Player2);
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
        private MoveTarget m_moveTarget;
        private Vector3 m_startPos;
        public Vector3 m_nextPos;

        public void MoveToTarget(bool movePause = false)
        {
            if(m_moveTarget)
                m_moveTarget.MovePause = movePause;

            StartCoroutine(_LockToMovement(0.1f));

            FaceToTarget();
            GameObject gb = Entity.Handle as GameObject;
            if (m_nextPos.x == 0 || Vector3.Distance(gb.transform.position, m_nextPos) < 0.5f)
                m_nextPos = GetTargetPos();
            
            m_startPos = gb.transform.position;
            if (m_moveTarget)
                m_moveTarget.Move(m_startPos, m_nextPos);                        
        }

        protected void FaceToTarget()
        {
            GameObject gb = Entity.Handle as GameObject;
            gb.transform.forward = m_nextPos - gb.transform.position;
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

        //留足时间切换到移动动作
        protected IEnumerator _LockToMovement(float lockTime)
        {
            yield return new WaitForSeconds(lockTime);
        }

        //protected void OnMoveToTarget(object sender, GameEventArgs e)
        //{
        //    MoveToTargetEventArgs mvArgs = e as MoveToTargetEventArgs;
        //    if (mvArgs.EId == Id)
        //    {
        //        m_moveTarget.MovePause = false;
        //    }
        //}
        //protected void OnMoveToEnd(object sender, GameEventArgs e)
        //{
        //    MoveToTargetEndEventArgs mvArgs = e as MoveToTargetEndEventArgs;
        //    if (mvArgs.parentId == m_data.Id)
        //    {
        //        m_moveTarget.MovePause  = true;
        //    }
        //}

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
