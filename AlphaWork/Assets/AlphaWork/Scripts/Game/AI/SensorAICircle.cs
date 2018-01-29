using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityGameFramework.Runtime;

namespace AlphaWork
{
    public class SensorAICircle : SensorAI
    {
        private List<int> results;
        private float m_radius = 5;
        private Vector3 m_rExtent;
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
            m_rExtent = new Vector3();
        }
        SensorAICircle(float radius)
        {
            m_sensorType = ESensorType.ESensor_Circle;
            m_radius = radius;
            results = new List<int>();
            hitResults = new RaycastHit[10];
            m_rExtent = new Vector3();
        }

        public override void ExecSensor(int agentId)
        {
            results.Clear();
            _search(ref results);
         
            GameEntry.Sensor.RegisterSense(new SenseResult(agentId, results));
        }

        private void Update()
        {
            
        }

        protected override void _search(ref List<int> results)
        {
            //总体看，还是数学上的简易计算更方便快捷准确。

            /*            int nHit = Physics.SphereCastNonAlloc(transform.position, m_radius, transform.forward, hitResults);*///据说消耗比较大

            //int nHit = Physics.BoxCastNonAlloc(transform.position, m_rExtent, transform.forward, hitResults);//消耗稍小，但是hitresults无法清空，导致逻辑容易出错

            //有一定GC。但从表现上看，没有影响后续的逻辑
            Collider[] cols = Physics.OverlapSphere(transform.position, m_radius);
            for (int i = 0; i < cols.Length; ++i)
            {
                //int tCode = cols[i].gameObject.GetHashCode();
                //int parentCode = GameEntry.Entity.GetEntity(m_parentEntId).Handle.GetHashCode();
                //if (tCode != parentCode && !(GameEntry.Entity.GetEntity(m_parentEntId).Logic as EffectEntity))
                //{
                //    int Id = GetEntityIdOfHashCode(tCode);
                //    if ((Id != 0) && !(results.Contains(Id)))
                //    {
                //        results.Add(Id);
                //    }
                //}

                Entity etCol = cols[i].gameObject.GetComponentInParent<Entity>();
                if (etCol == null)
                    continue;

                if(m_parentEntId != etCol.Id && (etCol.Id != 0) && !(results.Contains(etCol.Id)))
                {
                    results.Add(etCol.Id);
                }
            }

            //for (int k = 0; k < hitResults.Length; ++k)
            //{
            //    if(hitResults[k].collider != null)
            //    {
            //        int tCode = hitResults[k].collider.gameObject.GetHashCode();
            //        int parentCode = GameEntry.Entity.GetEntity(m_parentEntId).Handle.GetHashCode();
            //        if (tCode != parentCode)
            //        {
            //            int Id = GetEntityIdOfHashCode(tCode);
            //            if ((Id != 0) && !(results.Contains(Id)))
            //            {
            //                results.Add(Id);
            //            }
            //        }                    
            //    }
            //}            
        }
        
    }
}
