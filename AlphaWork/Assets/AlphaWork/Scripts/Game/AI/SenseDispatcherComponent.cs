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
        private List<SenseResult> m_senses;
        private float m_lastTime;

        public void RegisterSense(SenseResult sense)
        {
            if (m_senses.Count == 0)
                m_senses.Add(sense);
            else
            {
                for (int i = 0; i < m_senses.Count; ++i)
                {
                    if (m_senses[i].m_sensor == sense.m_sensor)
                        return;
                }
                m_senses.Add(sense);
            }
        }

        public void UnRegisterSense(SenseResult sense)
        {
            m_senses.Remove(sense);
        }

        public void Start()
        {
            m_senses = new List<SenseResult>();
            m_lastTime = Time.realtimeSinceStartup;
        }
        
        public void Update()
        {
            for (int i = 0; i < m_senses.Count; ++i)
            {
                for (int k = 0; k < m_senses[i].m_results.Count; ++k)
                {
                    GameEntry.Event.FireNow(this, new SenseAIEventArgs(m_senses[i].m_sensor, m_senses[i].m_results[k]));
                }
            }
            m_senses.Clear();
        }

    }
}
