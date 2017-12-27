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
        private string m_sensor;
        private int m_resultHashCode;

        public SenseAIEventArgs(string sensor/*,int hashcode*/)
        {
            m_sensor = sensor;
            //m_resultHashCode = hashcode;
        }

        public override int Id
        {
            get
            {
                return EventId;
            }
        }
        public string Sensor
        {
            get { return m_sensor; }
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
