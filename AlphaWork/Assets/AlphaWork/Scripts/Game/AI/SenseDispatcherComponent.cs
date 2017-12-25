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
        private  HashSet<SenseResult> m_senses;
        private float m_lastTime;

        public void RegisterSense(SenseResult sense)
        {
            m_senses.Add(sense);
        }

        public void UnRegisterSense(SenseResult sense)
        {
            m_senses.Remove(sense);
        }

        public void Start()
        {
            m_senses = new HashSet<SenseResult>();
            m_lastTime = Time.realtimeSinceStartup;
        }

        public void Update()
        {
//             float t = Time.realtimeSinceStartup;
//             if(t - m_lastTime > 1)
            {
                for (int i = 0; i < m_senses.Count; ++i)
                {
                    for (int k = 0; k < m_senses.ElementAt(i).m_results.Count; ++k)
                    {
                        GameEntry.Event.Fire(this, new SenseAIEventArgs(m_senses.ElementAt(i).m_sensor.ParentId,
                            m_senses.ElementAt(i).m_results[k].Entity.Handle.GetHashCode()));
                    }
                }

                m_senses.Clear();

 //               m_lastTime = t;
            }
            
        }
    }
}
