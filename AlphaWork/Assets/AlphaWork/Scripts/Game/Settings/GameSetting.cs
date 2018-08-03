using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace AlphaWork
{
    /// <summary>
    /// 游戏模式。
    /// </summary>
    public enum GameMode
    {
        /// <summary>
        /// 生存模式。
        /// </summary>
        Survival,
        //策略
        Strategy,
    }

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
        [SerializeField]
        public List<PlayerSetting> Users;    

        private PlayerSetting m_currentPlayer;
        public PlayerSetting PlayerInfo
        {
            get { return m_currentPlayer; }
            set { m_currentPlayer = value; }
        }

        private void OnEnable()
        {
            if (gameMode == GameMode.Survival)
                gameContrller = new SurvivalController();
            else if(gameMode == GameMode.Strategy)
                gameContrller = new StrategyController();
        }


    }
}
