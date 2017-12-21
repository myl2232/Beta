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
            SenseAIEventArgs se = e as SenseAIEventArgs;
            if (se != null)
            {
                if (se.ResultHashCode == m_parent.Entity.Handle.GetHashCode())
                {
                    EntityObject sensor = GameEntry.Entity.GetEntity(se.SensorId).Logic as EntityObject;

                }
            }
        }
    }
}
