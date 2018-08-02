using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace AlphaWork.Editor
{
    public class GameSettingFactory
    {
        public static string SettingFolder = "Assets/AlphaWork/GenerateData/GameSettings/";

        [MenuItem("Assets/Create/GameSetting")]
        public static void CreateGameSetting()
        {
            GameSetting setting = ScriptableObject.CreateInstance<GameSetting>();
            string strPath = SettingFolder + GetNextName();
            AssetDatabase.CreateAsset(setting, strPath);
        }       

        private static string GetNextName()
        {
            DateTime now = DateTime.Now;
            string newname = now.ToString();
            newname = newname.Replace(" ", "");
            newname = newname.Replace("-", "");
            newname = newname.Replace(":", "");
            newname = newname.Replace("/", "_");
            return "GameSetting-"+newname+".asset";              
        }
    }
}
