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
        private RaycastHit[] hitResults;

        public float Radius
        {
            get { return m_radius; }
            set { m_radius = value; }
        }
        SensorAICircle()
        {
            m_sensorType = ESensorType.ESensor_Circle;
            results = new List<int>(30);
            hitResults = new RaycastHit[10];
        }
        SensorAICircle(float radius)
        {
            m_sensorType = ESensorType.ESensor_Circle;
            m_radius = radius;
            results = new List<int>();
            hitResults = new RaycastHit[10];
        }

        public override void ExecSensor(int agentId)
        {
            results.Clear();
            _search(ref results);
         
            GameEntry.Sensor.RegisterSense(new SenseResult(agentId, results));
        }

        protected override void _search(ref List<int> results)
        {
            int nHit = Physics.SphereCastNonAlloc(transform.position, m_radius, transform.forward,
                hitResults, m_radius);

//             Collider[] cols = Physics.OverlapSphere(transform.position, m_radius);
//             for (int i = 0; i < cols.Length; ++i)
//             {
//                 int tCode = cols[i].gameObject.GetHashCode();
//                 int parentCode = GameEntry.Entity.GetEntity(m_parentEntId).Handle.GetHashCode();
//                 if (tCode != parentCode)
//                 {                   
//                     int Id = GetEntityIdOfHashCode(tCode);
//                     if((Id != 0) && !(results.Contains(Id)))
//                     {
//                         results.Add(Id);                        
//                     }                        
//                 }
//             }

            for(int k = 0; k < hitResults.Length; ++k)
            {
                if(hitResults[k].collider != null)
                {
                    int tCode = hitResults[k].collider.gameObject.GetHashCode();
                    int parentCode = GameEntry.Entity.GetEntity(m_parentEntId).Handle.GetHashCode();
                    if (tCode != parentCode)
                    {
                        int Id = GetEntityIdOfHashCode(tCode);
                        if ((Id != 0) && !(results.Contains(Id)))
                        {
                            results.Add(Id);
                        }
                    }                    
                }
            }            
        }
        
    }
}
