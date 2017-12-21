using GameFramework.Event;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
//using HomeSystemSpace;

namespace AlphaWork
{
    public partial class LevelManager
    {
        public SurviveGame m_parent;

        public LevelManager()
        {
            RegisterCustomEvents();
            InitCallbacks();
            
        }

        protected void RegisterCustomEvents()
        {
            GameEntry.Event.Subscribe(GameToLoginEventArgs.EventId, OnGameToLogin);
            GameEntry.Event.Subscribe(GameStartEventArgs.EventId, OnGameStart);
            //GameEntry.Event.Subscribe(UIAlphaEventArgs.EventId, OnUIAlpha);
/*            GameEntry.Event.Subscribe(UIBetaEventArgs.EventId,OnAIGo);   */         
        }
        protected void InitCallbacks()
        {
            m_loadForOccupyCallbacks = new GameFramework.Resource.LoadAssetCallbacks(
                LoadResourceForOccupySuccessCallback);
            m_StructureCallbacks = new StructureBehaviourCallback(OnOccupy, OnUnOccupy);
        }
 
    }
}
