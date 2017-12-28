using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace AlphaWork
{
    public class SensorAICircle : SensorAI
    {
        private List<int> results;
        private float m_radius = 5;

        public float Radius
        {
            get { return m_radius; }
            set { m_radius = value; }
        }
        SensorAICircle()
        {
            m_sensorType = ESensorType.ESensor_Circle;
            results = new List<int>();
        }
        SensorAICircle(float radius)
        {
            m_sensorType = ESensorType.ESensor_Circle;
            m_radius = radius;
            results = new List<int>();
        }

        public override void ExecSensor(int agentId)
        {
            results.Clear();
            _search(ref results);
            GameEntry.Sensor.RegisterSense(new SenseResult(agentId, results));
        }

        protected override void _search(ref List<int> results)
        {
            Collider[] cols = Physics.OverlapSphere(transform.position, m_radius);
            for (int i = 0; i < cols.Length; ++i)
            {
                int tCode = cols[i].gameObject.GetHashCode();
                int parentCode = GameEntry.Entity.GetEntity(m_parentEntId).Handle.GetHashCode();
                if (tCode != parentCode)
                {                   
                    int Id = GetEntityIdOfHashCode(tCode);
                    if((Id != 0) && !(results.Contains(Id)))
                    {
                        results.Add(Id);                        
                    }                        
                }
            }
        }
        
    }
}
