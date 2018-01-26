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
        private EntityObject m_parent;
        public EntityObject Parent
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

        private bool ValidSense(EntityObject sensor,int result)
        {
            AvatarData dt = sensor.Data as AvatarData;
            Entity etResult = GameEntry.Entity.GetEntity(result);
            if (etResult == null)
                return false;
            EntityObject obResult = etResult.Logic as EntityObject;
            TargetableObjectData trData = obResult.Data as TargetableObjectData;
            if (trData == null)
                return false;
            if (dt.Camp != trData.Camp)
                return true;

            return false;
        }

        public void OnSensorAI(EntityObject sensor, int result)
        {
            if (!ValidSense(sensor, result))
                return;

            AvatarEntity et = sensor.Entity.Logic as AvatarEntity;
            Enemy etEnemy = sensor.Entity.Logic as Enemy;

            if (et && et.Agent._get_bAwakeSense())
            {
                et.Agent.SenseResult = result;
            }
            else if(etEnemy && etEnemy.Agent._get_bAwakeSense())
            {
                etEnemy.Agent.SenseResult = result;
            }
        }

    }
}
