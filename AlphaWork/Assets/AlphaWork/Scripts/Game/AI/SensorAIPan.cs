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

        SensorAIPan(Vector3 center, Vector3 dir, float angle,float radius)
        {        
            m_sensorType = ESensorType.ESensor_Pan;
            m_angle = angle;
            m_radius = radius;
            m_center = center;
            m_dir = dir;
        }

        public override void ExecSensor()
        {
            List<EntityObject> results = new List<EntityObject>();
            _search(ref results);
            GameEntry.Sensor.RegisteSense(new SenseResult(this,ref results));
        }

        protected override void _search(ref List<EntityObject> results)
        {
            for(int i = 0; i < m_angle; ++i)
            {
                Vector3 dir = Quaternion.AngleAxis(i-m_angle/2, Vector3.up) * m_dir;
                RaycastHit hit;
                Physics.Raycast(m_center, dir, out hit, m_radius);
                EntityObject ob = GetEntityOfHashCode(hit.collider.gameObject.GetHashCode());
                if(ob)
                    results.Add(ob);
            }
        }

        protected EntityObject GetEntityOfHashCode(Int32 code)
        {
            UnityGameFramework.Runtime.Entity[] enties = GameEntry.Entity.GetAllLoadedEntities();
            for(int i = 0; i < enties.Length; ++i)
            {
                if (enties[i].Handle.GetHashCode() == code)
                    return enties[i].Logic as EntityObject;
            }
            return null;
        }
    }
}
