using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace AlphaWork.Editor
{
    public class SettingFactory
    {
        public static string SettingFolder = "Assets/AlphaWork/GenerateData/GameSettings/";
        public static string PlayerFolder = "Assets/AlphaWork/GenerateData/UserData/";

        private static string GetNextName(string preStr = "Setting")
        {
            DateTime now = DateTime.Now;
            string newname = now.ToString();
            newname = newname.Replace(" ", "");
            newname = newname.Replace("-", "");
            newname = newname.Replace(":", "");
            newname = newname.Replace("/", "_");
            return preStr + newname + ".asset";
        }

        [MenuItem("Assets/Create/Settings/GameSetting")]
        public static void CreateGameSetting()
        {
            GameSetting setting = ScriptableObject.CreateInstance<GameSetting>();
            string strPath = SettingFolder + GetNextName("GameSetting-");
            AssetDatabase.CreateAsset(setting, strPath);
        }       
        
        //[MenuItem("Assets/Create/Settings/PlayerSetting")]
        //public static void CreatePlayerSetting()
        //{
        //    PlayerSetting setting = ScriptableObject.CreateInstance<PlayerSetting>();
        //    string strPath = PlayerFolder + GetNextName("PlayerSetting-");
        //    AssetDatabase.CreateAsset(setting, strPath);
        //}
    }

    
}
