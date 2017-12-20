using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace AlphaWork
{
    public class SensorAICircle : SensorAI
    {
        private float m_radius;
        SensorAICircle(float radius)
        {
            m_sensorType = ESensorType.ESensor_Circle;
            m_radius = radius;
        }

        public override void ExecSensor()
        {
            List<EntityObject> results = new List<EntityObject>();
            _search(ref results);
            GameEntry.Sensor.RegisteSense(new SenseResult(this, ref results));
        }

        protected override void _search(ref List<EntityObject> results)
        {
            Vector3 vStart = new Vector3(1, 0, 1);
            for (int i = 0; i < 360; ++i)
            {
                Vector3 dir = Quaternion.AngleAxis(i, Vector3.up) * vStart;

                Collider[] cols = Physics.OverlapSphere(transform.position, m_radius, 0, QueryTriggerInteraction.Collide);
                for (int k = 0; k < cols.Length; ++k)
                {
                    EntityObject ob = GetEntityOfHashCode(cols[k].gameObject.GetHashCode());
                    if (ob)
                        results.Add(ob);
                }
            }
        }


    }
}
