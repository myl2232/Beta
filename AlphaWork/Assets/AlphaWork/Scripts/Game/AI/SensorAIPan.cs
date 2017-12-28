using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace AlphaWork
{
    public class SensorAIPan : SensorAI
    {
        private float m_angle;
        private float m_radius;
        private Vector3 m_center;
        private Vector3 m_dir;
        private List<int> results;
        SensorAIPan()
        {
            m_sensorType = ESensorType.ESensor_Pan;
            results = new List<int>();
        }
        SensorAIPan(Vector3 center, Vector3 dir, float angle,float radius)
        {        
            m_sensorType = ESensorType.ESensor_Pan;
            m_angle = angle;
            m_radius = radius;
            m_center = center;
            m_dir = dir;
            results = new List<int>();
        }

        public override void ExecSensor(int agentId)
        {
            if(results != null)
            {
                results.Clear();
                _search(ref results);
                GameEntry.Sensor.RegisterSense(new SenseResult(agentId, results));
            }
        }

        protected override void _search(ref List<int> results)
        {
            for(int i = 0; i < m_angle; ++i)
            {
                Vector3 dir = Quaternion.AngleAxis(i-m_angle/2, Vector3.up) * m_dir;
                RaycastHit hit;
                Physics.Raycast(m_center, dir, out hit, m_radius);
                int Id = GetEntityIdOfHashCode(hit.collider.gameObject.GetHashCode());
                if(!results.Contains(Id))
                    results.Add(Id);
            }
        }

        
    }
}
