using GameFramework.Event;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityGameFramework.Runtime;

namespace AlphaWork
{
    //legecy格式的角色，动画和mesh在一起
    public class BarbarianCharacter : BaseCharacter
    {
        public Animator m_animator;
        public Rigidbody rbody;

        private void Start()
        {
            m_animator = GetComponent<Animator>();
            rbody = GetComponent<Rigidbody>();
            rbody.mass = 100;

            if (IsMainActor())
                rbody.useGravity = true;
            else
                rbody.useGravity = false;
        }

        public override void SyncStatus(int status)
        {
            base.SyncStatus(status);

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
                m_animator.SetFloat("MoveBlend", speed);
                m_animator.SetTrigger("Move");
            }
        }

        public override void ActionIdle()
        {
            m_animator.SetBool("Dead", false);
        }

        public override void ActionHurt()
        {
            m_animator.SetBool("Hurt",true);
            //StartCoroutine(_LockMovementAndAttack(0, .667f));
        }
        public override void ActionDead()
        {
            m_animator.SetBool("Dead",true);
        }

        ////method to keep character from moveing while attacking, etc
        //protected IEnumerator _LockMovementAndAttack(float delayTime, float lockTime)
        //{
        //    yield return new WaitForSeconds(delayTime);
        //    PauseMove();
        //    rbody.velocity = Vector3.zero;
        //    rbody.angularVelocity = Vector3.zero;            
        //    m_animator.applyRootMotion = true;
        //    yield return new WaitForSeconds(lockTime);
        //    m_animator.applyRootMotion = false;
        //}               

    }

}
