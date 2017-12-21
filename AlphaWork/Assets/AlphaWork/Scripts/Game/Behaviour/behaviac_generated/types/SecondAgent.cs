﻿// -------------------------------------------------------------------------------
// THIS FILE IS ORIGINALLY GENERATED BY THE DESIGNER.
// YOU ARE ONLY ALLOWED TO MODIFY CODE BETWEEN '///<<< BEGIN' AND '///<<< END'.
// PLEASE MODIFY AND REGENERETE IT IN THE DESIGNER FOR CLASS/MEMBERS/METHODS, ETC.
// -------------------------------------------------------------------------------

using System;
using System.Collections;
using System.Collections.Generic;

///<<< BEGIN WRITING YOUR CODE FILE_INIT
using UnityEngine;
using AlphaWork;
//using UnityGameFramework.Runtime;
///<<< END WRITING YOUR CODE

public class SecondAgent : FirstAgent
///<<< BEGIN WRITING YOUR CODE SecondAgent
///<<< END WRITING YOUR CODE
{
	public int p2 = 0;

	public void Attack(float attackParam)
	{
///<<< BEGIN WRITING YOUR CODE Attack
        GameObject gb = m_parent.Entity.Handle as GameObject;
        if (gb)
        {
            Animator animator = gb.GetComponent<Animator>();
            LogicStatus status = LogicStatus.ELogic_ATTACK | ~basicStatus;
            animator.SetLayerWeight(1, 0.5f);
            animator.SetInteger("status",(int)status);
            animator.SetFloat("BlendAttack",attackParam);
        }
///<<< END WRITING YOUR CODE
	}

	public void FlushSensor()
	{
///<<< BEGIN WRITING YOUR CODE FlushSensor
        if(m_ai != null)
            m_ai.ExecSensor();
        ///<<< END WRITING YOUR CODE
	}

	public void MakeIdle()
	{
///<<< BEGIN WRITING YOUR CODE MakeIdle
        ///
        GameObject gb = m_parent.Entity.Handle as GameObject;
        if (gb)
        {
            Animator animator = gb.GetComponent<Animator>();
            LogicStatus status = LogicStatus.ELogic_IDLE | ~basicStatus;
            animator.SetLayerWeight(1, 0.0f);
            animator.SetInteger("status", (int)status);
        }
        ///<<< END WRITING YOUR CODE
	}

	public void Patrol()
	{
///<<< BEGIN WRITING YOUR CODE Patrol
        GameObject gb = m_parent.Entity.Handle as GameObject;
        if (gb)
        {
            Animator animator = gb.GetComponent<Animator>();
            LogicStatus status = LogicStatus.ELogic_PATROL | ~basicStatus;
            animator.SetInteger("status", (int)status);
            
        }
        ///<<< END WRITING YOUR CODE
	}

///<<< BEGIN WRITING YOUR CODE CLASS_PART
    LogicStatus basicStatus = LogicStatus.ELogic_PATROL | LogicStatus.ELogic_ATTACK | LogicStatus.ELogic_IDLE | 
        LogicStatus.ELogic_AIR | LogicStatus.ELogic_Hurt| LogicStatus.ELogic_Dead |LogicStatus.ELogic_Jump;

    private SensorAICircle m_ai;
    private BehaviacTrigger m_trigger;
    private BehaviourMove m_move;
    public void InitAI(float aiRadius)
    {
        GameObject gb = m_parent.Entity.Handle as GameObject;

        m_ai = gb.AddComponent<SensorAICircle>();
        m_ai.Radius = aiRadius;
        m_ai.ParentId = m_parent.Id;
        m_trigger = gb.AddComponent<BehaviacTrigger>();
        m_trigger.Parent = m_parent.Entity.Logic as EntityObject;

        _initMove();//temperary,will be removed
    }

    public void _initMove()
    {
        m_move = (m_parent.Entity.Handle as GameObject).AddComponent<BehaviourMove>();
        m_move.Parent = m_parent;
    }
    
    ///<<< END WRITING YOUR CODE

}

///<<< BEGIN WRITING YOUR CODE FILE_UNINIT

///<<< END WRITING YOUR CODE

