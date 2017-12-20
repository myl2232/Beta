using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityGameFramework.Runtime;

namespace AlphaWork
{
    public  class SenseDispatcherComponent: GameFrameworkComponent
    {
        private  List<SenseResult> m_senses;

        public void RegisteSense(SenseResult sense)
        {
            m_senses.Add(sense);
        }

        public void UnRegisteSense(SenseResult sense)
        {
            m_senses.Remove(sense);
        }

    }
}
