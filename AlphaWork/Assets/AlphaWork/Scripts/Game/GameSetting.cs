using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace AlphaWork
{
    public class GameSetting : ScriptableObject
    {   
        [SerializeField]
        public GameMode gameMode = GameMode.Survival;
        [SerializeField]
        public bool ArMode = false;
        [SerializeField]
        public bool UseNavGrid = true;
        [SerializeField]
        public IGameController gameContrller = null;

        private void OnEnable()
        {
            if (gameMode == GameMode.Survival)
                gameContrller = new SurvivalController();
            else if(gameMode == GameMode.Strategy)
                gameContrller = new StrategyController();
        }


    }
}
