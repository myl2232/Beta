using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using SQLite4Unity3d;
using UnityEngine;

#if !UNITY_EDITOR
using System.Collections;
using System.IO;
#endif

namespace AlphaWork
{
    public class SQLiteDataDevice : IDataDevice
    {
        private SQLiteConnection _connection;

        public SQLiteDataDevice(string DatabaseName)
        {

#if UNITY_EDITOR
            var dbPath = string.Format(@"Assets/StreamingAssets/{0}", DatabaseName);
#else
        // check if file exists in Application.persistentDataPath
        var filepath = string.Format("{0}/{1}", Application.persistentDataPath, DatabaseName);

        if (!File.Exists(filepath))
        {
            Debug.Log("Database not in Persistent path");
            // if it doesn't ->
            // open StreamingAssets directory and load the db ->

#if UNITY_ANDROID 
            var loadDb = new WWW("jar:file://" + Application.dataPath + "!/assets/" + DatabaseName);  // this is the path to your StreamingAssets in android
            while (!loadDb.isDone) { }  // CAREFUL here, for safety reasons you shouldn't let this while loop unattended, place a timer and error check
            // then save to Application.persistentDataPath
            File.WriteAllBytes(filepath, loadDb.bytes);
#elif UNITY_IOS
                 var loadDb = Application.dataPath + "/Raw/" + DatabaseName;  // this is the path to your StreamingAssets in iOS
                // then save to Application.persistentDataPath
                File.Copy(loadDb, filepath);
#elif UNITY_WP8
                var loadDb = Application.dataPath + "/StreamingAssets/" + DatabaseName;  // this is the path to your StreamingAssets in iOS
                // then save to Application.persistentDataPath
                File.Copy(loadDb, filepath);

#elif UNITY_WINRT
		var loadDb = Application.dataPath + "/StreamingAssets/" + DatabaseName;  // this is the path to your StreamingAssets in iOS
		// then save to Application.persistentDataPath
		File.Copy(loadDb, filepath);
		
#elif UNITY_STANDALONE_OSX
		var loadDb = Application.dataPath + "/Resources/Data/StreamingAssets/" + DatabaseName;  // this is the path to your StreamingAssets in iOS
		// then save to Application.persistentDataPath
		File.Copy(loadDb, filepath);
#else
	var loadDb = Application.dataPath + "/StreamingAssets/" + DatabaseName;  // this is the path to your StreamingAssets in iOS
	// then save to Application.persistentDataPath
	File.Copy(loadDb, filepath);

#endif

            Debug.Log("Database written");
        }

        var dbPath = filepath;
#endif
            _connection = new SQLiteConnection(dbPath, SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.Create);
            Debug.Log("Final PATH: " + dbPath);

        }

        public void CreateTable<T>(bool bclear = false) where T : ITable
        {
            if(bclear)
                _connection.DropTable<T>();
            _connection.CreateTable<T>();
        }

        public void ClearTable<T>() where T : ITable
        {
            _connection.DropTable<T>();
        }

        public void AddData<T>(T data) where T : ITable
        {
            _connection.Insert(data,typeof(T));
        }

        public IEnumerable<UPlayer> GetPlayers()
        { 
            return _connection.Table<UPlayer>();
        }

        public IEnumerator<T> GetData<T>() where T : ITable, new()
        {
            return _connection.Table<T>().GetEnumerator();
        }

        public void GetDataByKey<T>(string name, out IEnumerator<T> dataEnumerator) where T : ITable,new()
        {
            Func<T, bool> findMethod = delegate(T item)
            {
                return item.KeyName == name;
            };

            TableQuery<T> query = _connection.Table<T>();
            dataEnumerator = query.Where(x=> x.KeyName == "aaa"/*findMethod(x)*/).GetEnumerator();
        }
       
        public void Close()
        {
            _connection.Close();
        }

    }
}
