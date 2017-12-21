using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityStandardAssets.Characters.ThirdPerson;

namespace AlphaWork
{
    public partial class LevelManager
    {
        public Transform trans;

        public void RefreshAgent(Vector3 pos)
        {
            return;
            GameObject gbAgent = ObjectUtility.GetAssistObject() as GameObject;
            if (gbAgent)
            {
                GameObject transObjet = ObjectUtility.GetTargetAgent() as GameObject;
                trans = transObjet.transform;
                trans.position = new Vector3(pos.x, UnityEngine.Random.Range(pos.y, pos.y + 5), pos.z);

                AICharacterControl ctl = gbAgent.GetComponent<AICharacterControl>();
                if (ctl)
                {
                    ctl.target = trans;
                }
            }
        }

        public void RefreshEnemy(GameObject target, GameObject dest, Vector3 pos)
        {
            if (target && dest)
            {
                trans = dest.transform;
                trans.position = new Vector3(
                    UnityEngine.Random.Range(pos.x - 5, pos.x + 5),
                    pos.y,
                    UnityEngine.Random.Range(pos.z - 5, pos.z + 5));

                MoveTarget ctl = target.GetComponent<MoveTarget>();
                if (ctl)
                {
                    ctl.SetTarget(trans.position);
                }
            }
        }

    }
}
