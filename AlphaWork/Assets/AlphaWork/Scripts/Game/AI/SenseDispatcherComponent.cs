using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityGameFramework.Runtime;

namespace AlphaWork
{
    public  class SenseDispatcherComponent: GameFrameworkComponent
    {
        private  List<SenseResult> m_senses;

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
            m_senses = new List<SenseResult>();
        }

        public void Update()
        {
            for(int i = 0; i < m_senses.Count; ++i)
            {                
                for(int k = 0; k < m_senses[i].m_results.Count; ++k)
                {
                    GameEntry.Event.Fire(this, new SenseAIEventArgs(m_senses[i].m_sensor.ParentId, 
                        m_senses[i].m_results[k].Entity.Handle.GetHashCode()));
                }
            }

            m_senses.Clear();
        }
    }
}
