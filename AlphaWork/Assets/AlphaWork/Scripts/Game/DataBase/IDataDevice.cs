using System;
using System.Collections.Generic;

namespace AlphaWork
{
    public interface IDataDevice
    {
        void CreateTable<T>(bool bClear = false) where T : ITable;
        void ClearTable<T>() where T : ITable;
        void AddData<T>(T data) where T : ITable;
        void GetDataByKey<T>(string keyName, out IEnumerator<T> dataEnumerator) where T : ITable, new();
        IEnumerator<T> GetData<T>() where T : ITable, new();
        void Close();
    }

    public interface IDataFactory
    {
        void CreateDevice<T>() where T : IDataDevice;
    }

}
