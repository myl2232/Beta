using LuaInterface;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityGameFramework.Runtime;

namespace AlphaWork
{
    public enum DataBaseType
    {
        SQLite,
    }

    public class DataBaseComponent : GameFrameworkComponent, IDataFactory
    {
        public DataBaseType DataBaseType;
        public bool ClearPlayer = false;
        private IDataDevice m_ds;
        private List<UPlayer> m_Players;

        public List<UPlayer> Players
        {
            get { return m_Players; }
            private set {}
        }

        public IDataDevice DataDevice
        {
            get { return m_ds; }
            private set { }
        }

        // Use this for initialization
        private void OnEnable()
        {
            StartSync();
        }

        private void StartSync()
        {
            if (DataBaseType == DataBaseType.SQLite)
            {
                CreateDevice<SQLiteDataDevice>();
            }
        }

        public void CreateDevice<T>() where T : IDataDevice
        {
            if (typeof(SQLiteDataDevice) == typeof(T))
            {
                if(m_ds != null && m_ds.GetType() == typeof(SQLiteDataDevice))
                {
                    return;
                }
                m_ds = new SQLiteDataDevice("tempDatabase.db");
                m_ds.CreateTable<UPlayer>(ClearPlayer);
                //初始化数据
                m_Players = new List<UPlayer>();                
            }
        }
        public List<UPlayer> GetPlayerByName(string name)
        {
            GameEntry.DataBase.DataDevice.GetDataByKey<UPlayer>(name, out m_Players);
            return m_Players;
        }

        public void FetchPlayersByName(string name)
        {
            GameEntry.DataBase.DataDevice.GetDataByKey<UPlayer>(name, out m_Players);

            //LuaTable tb = GameEntry.LuaScriptEngine.LuaState.Require<LuaTable>(GUIDefine.UILoginModule);
            //LuaFunction func = tb.GetLuaFunction("FillData");
            //func.BeginPCall();
            //func.Push(new List<UPlayer>());
            //func.Push(m_Players);
            //func.PCall();
            //func.EndPCall();
            //func.Dispose();
            //func = null;
        }

        public void AddPlayer(UPlayer player)
        {
            m_ds.AddData<UPlayer>(player);
        }

        private void OnDestroy()
        {
            m_ds.Close();
        }
    }
}
