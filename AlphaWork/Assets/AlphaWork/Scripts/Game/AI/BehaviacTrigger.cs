using GameFramework.Event;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityGameFramework.Runtime;

namespace AlphaWork
{
    public class BehaviacTrigger : MonoBehaviour
    {
        private CustomEntity m_parent;
        public CustomEntity Parent
        {
            get { return m_parent; }
            set { m_parent = value; }
        }

        public void Start()
        {
            GameEntry.Event.Subscribe(SenseAIEventArgs.EventId, OnSensor);
        }
                
        public void OnSensor(object sender, GameEventArgs e)
        {
//             SenseAIEventArgs se = e as SenseAIEventArgs;
//             if (se != null && m_parent.Id == se.Result)
//             {
//                 EntityObject sensor = GameEntry.Entity.GetEntity(se.Sensor).Logic as EntityObject;
//                 if(sensor != null)
//                 {
//                     
//                 }
// 
//             }
        }

        private bool ValidSense(EntityLogic sensor,int result)
        {
            TargetableObjectData dt = (sensor as CustomEntity).Data as TargetableObjectData;
            if (dt == null)
                return false;
            Entity etResult = GameEntry.Entity.GetEntity(result);
            if (etResult == null)
                return false;
            TargetableObjectData trData = (etResult.Logic as CustomEntity).Data as TargetableObjectData;
            if (trData == null)
                return false;
            if (dt.Camp != trData.Camp)
                return true;

            return false;
        }

        public void OnSensorAI(EntityLogic sensor, int result)
        {
            if (!ValidSense(sensor, result))
                return;

            AvatarEntity et = sensor.Entity.Logic as AvatarEntity;
            Enemy etEnemy = sensor.Entity.Logic as Enemy;

            if (et && et.Agent._get_bAwakeSense())
            {
                et.Agent.SenseResult = result;
            }
            else if (etEnemy && etEnemy.Agent._get_bAwakeSense())
            {
                etEnemy.Agent.SenseResult = result;
            }
        }

    }
}
