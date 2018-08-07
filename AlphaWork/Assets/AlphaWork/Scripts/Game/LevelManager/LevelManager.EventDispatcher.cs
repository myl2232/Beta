using GameFramework.Event;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

namespace AlphaWork
{
    public partial class LevelManager
    {
        protected Transform m_HitTransform;

        public void OnGameToLogin(object sender, GameEventArgs e)
        {
            ClearStructures();
        }

        public void OnGameStart(object sender, GameEventArgs e)
        {
            GetDefaultTerrain();
            LoadGameObjects();
            //BuildBlocks();
            
        }

        public void OnUIAlpha(object sender, GameEventArgs e)
        {            

        }

        public void OnAIGo(object sender, GameEventArgs e)
        {
           

        }

    }
}
