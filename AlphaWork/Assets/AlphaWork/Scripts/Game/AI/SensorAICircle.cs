using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace AlphaWork
{
    public class SensorAICircle : SensorAI
    {
        SensorAICircle()
        {
            m_sensorType = ESensorType.ESensor_Circle;
        }

        public override void ExecSensor()
        {
            List<EntityObject> results = new List<EntityObject>();
            _search(ref results);
            GameEntry.Sensor.RegisteSense(new SenseResult(this, ref results));
        }

        protected override void _search(ref List<EntityObject> results)
        {

        }
    }
}
