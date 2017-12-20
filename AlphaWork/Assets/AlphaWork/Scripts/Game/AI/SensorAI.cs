using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace AlphaWork
{
    public enum ESensorType
    {
        ESensor_Circle,
        ESensor_Pan,
    }

    public struct SenseResult
    {
        private SensorAI m_sensor;
        private List<EntityObject> m_results;

        public SenseResult(SensorAI sr, ref List<EntityObject> results)
        {
            m_sensor = sr;
            m_results = results;
        }
    }

    public class SensorAI : MonoBehaviour
    {
        protected ESensorType m_sensorType;

        public ESensorType SensorType
        {
            get { return m_sensorType; }
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
    }
}
