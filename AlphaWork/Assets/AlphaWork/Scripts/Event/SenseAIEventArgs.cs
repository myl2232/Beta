using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using GameFramework.Event;

namespace AlphaWork
{
    class SenseAIEventArgs: GameEventArgs
    {
        public static readonly int EventId = typeof(SenseAIEventArgs).GetHashCode();
        private int m_sensorId;
        private int m_resultHashCode;

        public SenseAIEventArgs(int sensorEntId,int hashcode)
        {
            m_sensorId = sensorEntId;
            m_resultHashCode = hashcode;
        }

        public override int Id
        {
            get
            {
                return EventId;
            }
        }
        public int SensorId
        {
            get { return m_sensorId; }
        }
        public int ResultHashCode
        {
            get { return m_resultHashCode; }
        }

        public override void Clear()
        {

        }
    }
}
