using System;
using System.Collections;
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
                //StartCoroutine(_LockMovementAndAttack(0, .25f));
                m_animator.SetBool("Move", false);
            }
        }
        public override void ActionPatrol(float speed)
        {
            if (m_animator != null)
            {
                m_animator.SetBool("Move",true);
                m_animator.SetFloat("MoveBlend", speed);
            }
        }
        public override void ActionIdle()
        {
            m_animator.SetBool("Dead", false);
            m_animator.SetBool("Move", false);
        }
        public override void ActionHurt()
        {
            m_animator.SetTrigger("Hurt");
            m_animator.SetBool("Move", false);
        }
        public override void ActionDead()
        {
            m_animator.SetBool("Dead",true);
            m_animator.SetBool("Move", false);
        }

        //method to keep character from moveing while attacking, etc
        protected IEnumerator _LockMovementAndAttack(float delayTime, float lockTime)
        {
            //yield return new WaitForSeconds(delayTime);
           
            m_animator.SetBool("Moving", false);
            rbody.velocity = Vector3.zero;
            rbody.angularVelocity = Vector3.zero;            
            m_animator.applyRootMotion = true;
            yield return new WaitForSeconds(lockTime);
            m_animator.applyRootMotion = false;
        }
    }

}
