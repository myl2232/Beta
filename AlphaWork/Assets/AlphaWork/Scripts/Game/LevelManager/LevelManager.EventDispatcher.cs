using GameFramework.Event;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

namespace AlphaWork
{
    public partial class LevelManager
    {
        protected Transform m_HitTransform;

        public void OnGameToLogin(object sender, GameEventArgs e)
        {
            ClearStructures();
        }

        public void OnGameStart(object sender, GameEventArgs e)
        {
            GetDefaultTerrain();
            LoadGameObjects();
            //BuildBlocks();
                        
        }

        public void OnUIOccupy(object sender, GameEventArgs e)
        {
            
            //Vector3 mainPos = new Vector3();
            //GameBase.GetMainPos(out mainPos);

            //int h = (int)mainPos.x / m_blockSize;
            //int w = (int)mainPos.z / m_blockSize;

            //int Id = m_blocks[w][h];
            
            //BlockInfo info = new BlockInfo();
            //info.EntityId = Id;
            //m_blocks[w][h] = Id;

            //UnityGameFramework.Runtime.Entity block = GameEntry.Entity.GetEntity(Id);
            //if (!block)
            //    return;

            //Structure structEnt = block.Logic as Structure;
            //structEnt.Occupyed = !structEnt.Occupyed;
            //GameObject gb = block.Handle as GameObject;
            //if (gb)
            //{
            //    //string texStr = structEnt.GetReplaceTex();
            //    GameEntry.Resource.LoadAsset(structEnt.GetReplaceTex(),
            //        m_loadForOccupyCallbacks, block);
            //}
        }
   //     protected bool HitTestWithType(ARPoint point, ARHitTestResultType resultTypes)
   //     {
   //         List<ARHitTestResult> hitResults = UnityARSessionNativeInterface.GetARSessionNativeInterface().HitTest(point, resultTypes);
   //         if (hitResults.Count > 0)
   //         {
   //             foreach (var hitResult in hitResults)
   //             {
   //                 Debug.Log("Got hit!");
   //                 m_HitTransform.position = UnityARMatrixOps.GetPosition(hitResult.worldTransform);
   //                 m_HitTransform.rotation = UnityARMatrixOps.GetRotation(hitResult.worldTransform);
   //                 Debug.Log(string.Format("x:{0:0.######} y:{1:0.######} z:{2:0.######}", m_HitTransform.position.x, m_HitTransform.position.y, m_HitTransform.position.z));
   //                 return true;
   //             }
   //         }
   //         return false;
   //     }
   //     public void OnAIGo(object sender, GameEventArgs e)
   //     {
   //         //if (Input.touchCount > 0)
   //         {
			////	var touch = Input.GetTouch(0);
   //         //    if (touch.phase == TouchPhase.Began)
   //             {
		 //          Vector3 updataPos = Camera.main.transform.position + Camera.main.transform.forward * 200;
			//		GameObject gb = ObjectUtility.GetFellow("BaseExtent") as GameObject;
			//		if (gb)
			//		{
   //                     gb.transform.position = updataPos;
			//		}

			//		RaycastHit hitInfo;
			//		Physics.Raycast(Camera.main.transform.position, updataPos, out hitInfo, 10000000);
			//		var screenPosition = Camera.main.ScreenToViewportPoint(updataPos);
   //                 ARPoint point = new ARPoint
   //                 {
   //                     x = screenPosition.x,
   //                     y = screenPosition.y
   //                 };

			//		// prioritize reults types
			//		ARHitTestResultType[] resultTypes = {
			//			ARHitTestResultType.ARHitTestResultTypeExistingPlaneUsingExtent, 
   //                     // if you want to use infinite planes use this:
   //                     //ARHitTestResultType.ARHitTestResultTypeExistingPlane,
   //                     ARHitTestResultType.ARHitTestResultTypeHorizontalPlane,
			//			ARHitTestResultType.ARHitTestResultTypeFeaturePoint
			//		};

   //                 foreach (ARHitTestResultType resultType in resultTypes)
   //                 {
			//			if (HitTestWithType(point, resultType))
			//			{
   //                         GameObject baseOb = ObjectUtility.GetFellow("BaseExtent") as GameObject;
   //                         if(baseOb)
   //                         {
   //                             baseOb.transform.position = m_HitTransform.position;
			//				}

   //                         //记录当前射线位置用于放置主角
   //                         m_parent.m_MainEthanTransform = m_HitTransform;
   //                         GameEntry.Event.Fire(this, new RefreshMainPosArgs(m_HitTransform));
			//			}
   //                 }
   //             }
   //         }
   //     }

    }
}
