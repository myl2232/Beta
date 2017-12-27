using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityGameFramework.Runtime;

namespace AlphaWork
{
    public  class SenseDispatcherComponent: GameFrameworkComponent
    {
        private List<string> m_sensors;
        private List<SenseResult> m_senses;
        private float m_lastTime;
        //behaviac.Agent ag = behaviac.Agent.GetInstance(m_agent.name);
        public void AddSensor(string name)
        {
            if (m_sensors.Contains(name))
                return;
            m_sensors.Add(name);
        }
        public void RemoveSensor(string name)
        {
            if (m_sensors.Contains(name))
                m_sensors.Remove(name);
        }

        public void RegisterSense(SenseResult sense)
        {
            if(!m_senses.Contains(sense))
                m_senses.Add(sense);
        }

        public void UnRegisterSense(SenseResult sense)
        {
            m_senses.Remove(sense);
        }

        public void Start()
        {
            m_senses = new List<SenseResult>();
            m_sensors = new List<string>();
            m_lastTime = Time.realtimeSinceStartup;
        }

        public void Update()
        {
            for (int i = 0; i < m_senses.Count; ++i)
            {
                for (int k = 0; k < m_senses[i].m_results.Count; ++k)
                {
                    GameEntry.Event.Fire(this, new SenseAIEventArgs(m_senses[i].m_sensor));
                }
            }            
        }

    }
}
