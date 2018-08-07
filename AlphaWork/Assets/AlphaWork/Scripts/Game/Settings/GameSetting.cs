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
        public GameBase gameContrller = null;
        [HideInInspector]
        public string CurrentUser;
        [HideInInspector]
        public int CurrentSceneId;

        private string id_str = "";
        public string UID
        { get { return id_str; } }

        private void OnEnable()
        {
            if(id_str == "")
            {
                DateTime now = DateTime.Now;
                string newname = now.ToString();
                newname = newname.Replace(" ", "");
                newname = newname.Replace("-", "");
                newname = newname.Replace(":", "");
                newname = newname.Replace("/", "_");
                id_str = "gamesetting:" + newname;
            }

            if (gameMode == GameMode.Survival)
                gameContrller = new SurvivalGame();
            else if(gameMode == GameMode.Strategy)
                gameContrller = new StrategyGame();
            
        }
       
    }
}
