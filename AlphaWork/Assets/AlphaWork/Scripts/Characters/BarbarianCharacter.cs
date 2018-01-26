using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace AlphaWork
{
    //legecy格式的角色，动画和mesh在一起
    public class BarbarianCharacter : BaseCharacter
    {
        public Animator m_animator;
        public Rigidbody rbody;
        private LogicStatus m_status;

        private void Start()
        {
            m_animator = GetComponent<Animator>();
            rbody = GetComponent<Rigidbody>();

            rbody.mass = 100;
        }

        public override void SyncStatus(int status)
        {
            m_status = (LogicStatus)status;
            if (m_animator != null)
                m_animator.SetInteger("status", (int)m_status);
        }

        public override void ActionAttack(float attackParam)
        {
            if (m_animator != null)
            {
                m_animator.SetTrigger("Attack");
                m_animator.SetFloat("AttackBlend", attackParam);                
            }
        }
        public override void ActionPatrol(float speed)
        {
            if (m_animator != null)
            {
                m_animator.SetTrigger("Move");
                m_animator.SetFloat("MoveBlend", speed);
            }
        }
        public override void ActionIdle()
        {
            m_animator.SetBool("Dead", false);
        }
        public override void ActionHurt()
        {
            m_animator.SetTrigger("Hurt");
        }
        public override void ActionDead()
        {
            m_animator.SetBool("Dead",true);
        }
    }

}
