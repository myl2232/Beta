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
using GameFramework.Event;
///<<< END WRITING YOUR CODE

public class EnemyAgent : BaseAgent
///<<< BEGIN WRITING YOUR CODE EnemyAgent
///<<< END WRITING YOUR CODE
{
	private float attackParam = 0f;
	public void _set_attackParam(float value)
	{
		attackParam = value;
	}
	public float _get_attackParam()
	{
		return attackParam;
	}

	public void CheckSensor()
	{
///<<< BEGIN WRITING YOUR CODE CheckSensor
        ////本该是状态机的工作，但是目前状态机的transition不支持右值参数，只能是数值
        UnityGameFramework.Runtime.Entity etEnemy = GameEntry.Entity.GetEntity(m_senseResult);
        int logicSt = -1;
        if (etEnemy != null)
        {
            float dist = Vector3.Distance(etEnemy.transform.position, m_parent.transform.position);
            if (InRange(dist, 0, m_LogicData.AttackRadius + SqAdd(GameEntry.NavGrid.MeshSize)))
            {
                logicSt = (int)LogicStatus.ELogic_ATTACK;
                m_character.SyncStatus(logicSt);
            }
            else if (InRange(dist, m_LogicData.AttackRadius - SqAdd(GameEntry.NavGrid.MeshSize), 
                m_LogicData.TrackRadius + SqAdd(GameEntry.NavGrid.MeshSize)))              
            {
                logicSt = (int)LogicStatus.ELogic_TRACK;
                m_character.SyncStatus(logicSt);
            }
            else
            {
                logicSt = (int)LogicStatus.ELogic_PATROL;
                m_character.SyncStatus(logicSt);
            }

            m_parent.m_nextPos = etEnemy.transform.position;
        }
        else
        {
            m_character.SyncStatus((int)LogicStatus.ELogic_IDLE);
        }
        _set_logicStatus((LogicStatus)logicSt);
        DispatchActions();
        ///<<< END WRITING YOUR CODE
	}

	public void FlushSensor()
	{
///<<< BEGIN WRITING YOUR CODE FlushSensor
        if(m_ai != null)
            m_ai.ExecSensor(m_parent.Id);
        ///<<< END WRITING YOUR CODE
	}

///<<< BEGIN WRITING YOUR CODE CLASS_PART

    private SensorAICircle m_ai;
    private Enemy m_parent;
    public int m_senseResult;
    public int SenseResult
    {
		get { return m_senseResult; }
        set{ m_senseResult = value; }
    }
    //public bool MoveOn = true;
    private BaseCharacter m_character;
    TargetableObjectData m_LogicData;

    public void InitAI()
    {
        m_parent = GameEntry.Entity.GetEntity(m_ParentId).Logic as Enemy;
        GameObject gb = m_parent.Entity.Handle as GameObject;
        m_LogicData = m_parent.Data as TargetableObjectData;

        m_ai = gb.AddComponent<SensorAICircle>();
        m_ai.Radius = m_LogicData.SenseRadius;
        m_ai.ParentId = m_parent.Id;

        m_character = gb.GetComponent<BaseCharacter>();
    }

    protected void DispatchActions()
    {
        LogicStatus status = _get_logicStatus();
        if (status == LogicStatus.ELogic_ATTACK)
        {
            m_parent.PauseMove();
            m_character.ActionAttack(attackParam);
        }
        else if (status == LogicStatus.ELogic_PATROL)
        {
            m_character.ActionPatrol(m_LogicData.walkSpeed);
            m_parent.SetSpeed(m_LogicData.walkSpeed * m_LogicData.baseSpeed);
            m_parent.MoveToTarget();
        }
        else if (status == LogicStatus.ELogic_TRACK)
        {
            m_character.ActionPatrol(m_LogicData.runSpeed);
            m_parent.SetSpeed(m_LogicData.runSpeed * m_LogicData.baseSpeed);
            m_parent.MoveToTarget();
        }
        else if(status == LogicStatus.ELogic_IDLE)
        {
            m_parent.PauseMove();
            m_character.ActionIdle();
        }
        else if(status == LogicStatus.ELogic_DEAD)
        {
            m_parent.PauseMove();
            m_character.ActionDead();
        }
    }
        
    ///<<< END WRITING YOUR CODE

}

///<<< BEGIN WRITING YOUR CODE FILE_UNINIT

///<<< END WRITING YOUR CODE

