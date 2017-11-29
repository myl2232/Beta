using GameFramework.Event;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

namespace AlphaWork
{
    public class Ethan : EntityObject
	{
        private int testskill = 0;
        // Use this for initialization
        void Start()
		{
            GameEntry.Event.Subscribe(UIAlphaEventArgs.EventId, OnShrink);
            GameEntry.Event.Subscribe(UIBetaEventArgs.EventId, OnTestBeta);
        }

        // Update is called once per frame
        void Update()
		{         

        }

        private void LateUpdate()
        {

        }
        
        void OnShrink(object sender, GameEventArgs arg)
        {
            GetComponent<ShakeHit>().OnShake();
        }

        void OnTestBeta(object sender, GameEventArgs arg)
        {
            if (testskill == 0)
            {
                CrossPlatformInputManager.SetButtonDown("skill1");
                testskill = 1;
            }
            else if(testskill ==1)
            {
                CrossPlatformInputManager.SetButtonDown("noskill");
                testskill = 0;
            }

        }
    }
}

