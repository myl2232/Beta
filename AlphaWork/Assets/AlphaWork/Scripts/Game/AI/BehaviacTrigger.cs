﻿using GameFramework.Event;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace AlphaWork
{
    public class BehaviacTrigger : MonoBehaviour
    {
        private EntityObject m_parent;
        public EntityObject Parent
        {
            get { return m_parent; }
            set { m_parent = value; }
        }

        public void Start()
        {
            GameEntry.Event.Subscribe(SenseAIEventArgs.EventId, OnSensor);
        }
                
        public void OnSensor(object sender, GameEventArgs e)
        {
//             SenseAIEventArgs se = e as SenseAIEventArgs;
//             if (se != null && m_parent.Id == se.Result)
//             {
//                 EntityObject sensor = GameEntry.Entity.GetEntity(se.Sensor).Logic as EntityObject;
//                 if(sensor != null)
//                 {
//                     
//                 }
// 
//             }
        }

        public void OnSensorAI(EntityObject sensor, int result)
        {
            AvatarEntity et = sensor.Entity.Logic as AvatarEntity;
            Enemy etEnemy = sensor.Entity.Logic as Enemy;

            if (et && et.Agent._get_bAwakeSense())
            {
                et.Agent.SenseResult = result;
            }
            else if(etEnemy && etEnemy.Agent._get_bAwakeSense())
            {
                etEnemy.Agent.SenseResult = result;
            }
        }

    }
}
