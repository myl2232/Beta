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
        private int m_sensor;
        private int m_result;

        public SenseAIEventArgs(int sensor, int result)
        {
            m_sensor = sensor;
            m_result = result;
        }

        public override int Id
        {
            get
            {
                return EventId;
            }
        }
        public int Sensor
        {
            get { return m_sensor; }
        }
        public int Result
        {
            get { return m_result; }
        }

        public override void Clear()
        {

        }
    }
}
