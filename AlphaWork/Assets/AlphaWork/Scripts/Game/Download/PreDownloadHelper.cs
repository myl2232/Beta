using GameFramework.Download;
using GameFramework.Resource;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityGameFramework.Runtime;

namespace AlphaWork
{
    public partial class PreDownloadHelper : MonoBehaviour
    {
        private Dictionary<string, string> m_downloadFiles = new Dictionary<string, string>();

        private string DownloadTable;
        private LoadAssetCallbacks loadCallbacks;
        //private string DownloadRoot = "http://localhost:81/WWW/ARAlpha/Assets/UnityARKitPlugin/Examples/Common/Textures/PlayerDiffuse.png";
        public PreDownloadHelper()
        {
            //DownloadRoot = "http://localhost:81/WWW/ARAlpha/Assets/UnityARKitPlugin/Examples/Common/Models/Characters/Player.fbx";
            //m_downloadFiles.Add(DownloadRoot);
            loadCallbacks = new LoadAssetCallbacks(OnLoadSuccessCallback,OnLoadFailureCallback);
        }

        public void Start()
        {
            DownloadTable = "Assets/AlphaWork/DataTables/DOWNLOAD.txt";
        }

        public void BeginDownload()
        {
            GameEntry.Resource.LoadAsset(DownloadTable, loadCallbacks);

            //             foreach(string file in m_downloadFiles)
            //             {
            //                 GameEntry.Download.AddDownload(FileUtility.GetStreamingDest(file),file);
            //                 //GameEntry.Download.AddDownload("F:/Unity/Private/AlphaWork/AlphaWork/Assets/DownloadTemp/1.png", DownloadRoot);
            //             }
            
        }

        public void OnLoadSuccessCallback(string assetName, object asset, float duration, object userData)
        {
            LoadDownloadTable(assetName, asset, userData);

            for (int i = 0; i < m_downloadFiles.Count; ++i)
            {
                GameEntry.Download.AddDownload(Application.streamingAssetsPath + "/" + m_downloadFiles.Values.ElementAt(i), m_downloadFiles.Keys.ElementAt(i));
            }
        }

        public void OnLoadFailureCallback(string assetName, LoadResourceStatus status, string errorMessage, object userData)
        {
            Debug.LogWarning("load downloadtalbe failed......");
        }

    }
}
