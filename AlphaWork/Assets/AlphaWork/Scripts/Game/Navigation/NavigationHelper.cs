using GameFramework.Resource;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using UnityEngine;

namespace AlphaWork
{
    class NavigationHelper : MonoBehaviour
    {
        private GCHandle handle;
        private LoadAssetCallbacks loadCallbacks;

        public NavigationHelper(string levelName)
        {
            loadCallbacks = new LoadAssetCallbacks(LoadNavigationSuccessCallback, LoadNavigationFailedCallback);

            NavigationInit();
            UpdateNavDataInDatabase(levelName);
        }

        protected void NavigationInit()
        {
            if (RecastNavigationDllImports.NavGeneration_Init() == false)
            {
                Debug.Log("Error initializing Recast Navigation plugin!");
            }

        }

        protected bool UpdateNavDataInDatabase(string levelName)
        {
// #if UNITY_EDITOR
//              UnityEngine.Object asset = Resources.Load(levelName + "RecastNavmesh");
// #else
            string racastAsset = AssetUtility.GetNavigationAsset(levelName);
            GameEntry.Resource.LoadAsset(racastAsset, loadCallbacks);    
//#endif
            return true;
        }

        public void CloseNavigation()
        {
            handle.Free();
        }

        private void LoadNavigationSuccessCallback(string NavAssetName, object NavAsset, float duration, object userData)
        {
            RecastNavigationAsset navDataAsset = NavAsset as RecastNavigationAsset;
            if (navDataAsset == null)
            {
                return;
            }

            handle = GCHandle.Alloc(navDataAsset.navMeshData, GCHandleType.Pinned);
            if (RecastNavigationDllImports.LoadNavDataImmediate(handle.AddrOfPinnedObject()) == false)
            {
                Debug.Log("Could not load data");
                return;
            }
        }

        private void LoadNavigationFailedCallback(string NavAssetName, LoadResourceStatus status, string errorMessage, object userData)
        {

        }

    }
}
