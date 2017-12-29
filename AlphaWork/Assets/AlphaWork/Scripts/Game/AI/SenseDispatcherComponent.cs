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
        private Dictionary<int,SenseResult> m_senses;
        private float m_lastTime;

        public void RegisterSense(SenseResult sense)
        {
            SenseResult result;
            if(!m_senses.TryGetValue(sense.m_sensor,out result))
            {
                m_senses[sense.m_sensor] = sense;
            }
        }

        public void UnRegisterSense(SenseResult sense)
        {
            m_senses.Remove(sense.m_sensor);
        }

        public void Start()
        {
            m_senses = new Dictionary<int, SenseResult>();
            m_lastTime = Time.realtimeSinceStartup;
        }
        
        public void Update()
        {
            foreach(KeyValuePair< int, SenseResult > item in m_senses)
            {
                for (int i = 0; i < item.Value.m_results.Count; ++i)
                {
                    OnBehaviourTrigger(item.Key, item.Value.m_results[i]);
                }
            }

            m_senses.Clear();
        }

        protected void OnBehaviourTrigger(int sensor, int result)
        {
            EntityObject etSensor = GameEntry.Entity.GetEntity(sensor).Logic as EntityObject;
            GameObject gbResult = GameEntry.Entity.GetEntity(sensor).Handle as GameObject;

            gbResult.GetComponent<BehaviacTrigger>().OnSensorAI(etSensor);
        }

    }
}
