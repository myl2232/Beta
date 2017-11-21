using GameFramework.Event;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

namespace AlphaWork
{
    public class Ethan : EntityObject
	{

        // Use this for initialization
        void Start()
		{
            GameEntry.Event.Subscribe(UIOccupyEventArgs.EventId, OnShrink);          
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
             
    }
}

