using UnityEngine;

namespace AlphaWork
{
    /// <summary>
    /// 游戏入口。
    /// </summary>
    public partial class GameEntry
    {
        public static ConfigComponent Config
        {
            get;
            private set;
        }
        /// <summary>
        /// 获取感知组件。
        /// </summary>
        public static SenseDispatcherComponent Sensor
        {
            get;
            private set;
        }
        /// <summary>
        /// 获取行为树组件。
        /// </summary>
        public static BehaviacComponent Behaviac
        {
            get;
            private set;
        }
        //寻路组件
        public static NavGridComponent NavGrid
        {
            get;
            private set;
        }
        //脚本组件
        public static LuaScriptComponent LuaScriptEngine
        {
            get;
            private set;
        }
        //数据库组件
        public static DataBaseComponent DataBase
        {
            get;
            private set;
        }

        private static void InitCustomComponents()
        {
            Config = UnityGameFramework.Runtime.GameEntry.GetComponent<ConfigComponent>();
            Sensor = UnityGameFramework.Runtime.GameEntry.GetComponent<SenseDispatcherComponent>();
            Behaviac = UnityGameFramework.Runtime.GameEntry.GetComponent<BehaviacComponent>();
            NavGrid = UnityGameFramework.Runtime.GameEntry.GetComponent<NavGridComponent>();
            LuaScriptEngine = UnityGameFramework.Runtime.GameEntry.GetComponent<LuaScriptComponent>();
            DataBase = UnityGameFramework.Runtime.GameEntry.GetComponent<DataBaseComponent>();
        }
      
    }
}
