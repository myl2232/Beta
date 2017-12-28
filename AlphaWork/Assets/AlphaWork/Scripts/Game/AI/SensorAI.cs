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
        public int m_sensor;
        public List<int> m_results;

        public SenseResult(int sr, List<int> results)
        {
            m_sensor = sr;
            m_results = results;
        }
//         public override bool Equals(object obj)
//         {
//             SenseResult res = (SenseResult)obj;
// 
//             return (m_sensor == res.m_sensor);
//         }
//         public override int GetHashCode()
//         {
//             return m_sensor.GetHashCode();
//         }
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
        private UnityGameFramework.Runtime.Entity[] enties;

        public void Start()
        {
            FlushEntityRecord();
            GameEntry.Event.Subscribe(ShowEntitySuccessEventArgs.EventId, OnShowEntitySuccess);
            GameEntry.Event.Subscribe(ShowEntityFailureEventArgs.EventId, OnShowEntityFailure);
        }

        public virtual void Init()
        {

        }

        public virtual void ExecSensor(int agentId)
        {

        }

        protected virtual void _search(ref List<int> results)
        {

        }

        private void FlushEntityRecord()
        {
            enties = GameEntry.Entity.GetAllLoadedEntities();
        }

        protected int GetEntityIdOfHashCode(Int32 code)
        {
            for (int i = 0; i < enties.Length; ++i)
            {
                if (enties[i].Handle.GetHashCode() == code)
                    return enties[i].Id;
            }
            return 0;
        }

//         public override bool Equals(object other)
//         {
//             SensorAI ai = (SensorAI)other;
//             return (m_sensorType == ai.m_sensorType) &&
//                 (m_parentEntId == ai.m_parentEntId);
//         }
//         public override int GetHashCode()
//         {
//             return base.GetHashCode();
//         }

        protected virtual void OnShowEntitySuccess(object sender, GameEventArgs e)
        {
            FlushEntityRecord();
        }
        protected virtual void OnShowEntityFailure(object sender, GameEventArgs e)
        {
            FlushEntityRecord();
        }

    }
}
