using GameFramework.Event;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityGameFramework.Runtime;

namespace AlphaWork
{
    public enum ESensorType
    {
        ESensor_Circle,
        ESensor_Pan,
    }

    public struct SenseResult
    {
        public SensorAI m_sensor;
        public List<EntityObject> m_results;

        public SenseResult(SensorAI sr, ref List<EntityObject> results)
        {
            m_sensor = sr;
            m_results = results;
        }
    }

    public class SensorAI : MonoBehaviour
    {
        protected ESensorType m_sensorType;
        protected int m_parentEntId;
        public int ParentId
        {
            get { return m_parentEntId; }
            set { m_parentEntId = value; }
        }

        public ESensorType SensorType
        {
            get { return m_sensorType; }
        }

        public void Start()
        {
               
        }

        public virtual void Init()
        {

        }

        public virtual void ExecSensor()
        {
            
        }

        protected virtual void _search(ref List<EntityObject> results)
        {

        }

        protected EntityObject GetEntityOfHashCode(Int32 code)
        {
            UnityGameFramework.Runtime.Entity[] enties = GameEntry.Entity.GetAllLoadedEntities();
            for (int i = 0; i < enties.Length; ++i)
            {
                if (enties[i].Handle.GetHashCode() == code)
                    return enties[i].Logic as EntityObject;
            }
            return null;
        }
                

    }
}
