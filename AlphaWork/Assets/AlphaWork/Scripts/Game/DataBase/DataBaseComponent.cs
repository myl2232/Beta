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
                m_ds = new SQLiteDataDevice("tempDatabase.db");
                m_ds.CreateTable<UPlayer>(ClearPlayer);
            }
        }

        private void OnDestroy()
        {
            m_ds.Close();
        }
    }
}
