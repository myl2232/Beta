using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AlphaWork
{
    public class SenseObject
    {
        private SensorAI m_sensor;
        private List<EntityObject> m_results;
        
        public SenseObject(SensorAI sr,ref List<EntityObject> results)
        {
            m_sensor = sr;
            m_results = results;
        }

    }
}
