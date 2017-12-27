using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace AlphaWork
{
    public class SensorAICircle : SensorAI
    {
        private float m_radius = 5;
        public float Radius
        {
            get { return m_radius; }
            set { m_radius = value; }
        }
        SensorAICircle(float radius)
        {
            m_sensorType = ESensorType.ESensor_Circle;
            m_radius = radius;
        }

        public override void ExecSensor(string agentName)
        {
            List<int> results = new List<int>();
            _search(ref results);
            //GameEntry.Sensor.UnRegisterSense(new SenseResult(this, ref results));
            GameEntry.Sensor.RegisterSense(new SenseResult(agentName, ref results));
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
                    EntityObject ob = GetEntityOfHashCode(tCode);
                    if (ob)
                        results.Add(ob.Id);
                }
            }
        }

    }
}
