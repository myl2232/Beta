using GameFramework.DataTable;
using GameFramework.Event;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace AlphaWork
{
    public partial class LevelManager
    {
        protected GameFramework.Resource.LoadAssetCallbacks m_loadForOccupyCallbacks;

        protected Dictionary<string, Texture> m_Textures = new Dictionary<string, Texture>();

        public void LoadGameObjects()
        {
            if (!GameBase.HasMainActor())
            {
                //add main actor
				Vector3 mainPos = Camera.main.transform.position + Camera.main.transform.forward * 20;			
				RaycastHit hit;
                Physics.Raycast(mainPos, Vector3.down, out hit, 1000);
                GameEntry.Entity.ShowEthan(new EthanData(GameEntry.Entity.GenerateSerialId(), 80001/*80000*/, CampType.Player)
                {
                    Position = hit.point,
                });

                //test to add npc for Efficiency
                for (int i = 0; i < 4; ++i)
                {
                    for (int j = 0; j < 5; ++j)
                    {
                        //GameEntry.Entity.ShowAvatar(new AvatarData(GameEntry.Entity.GenerateSerialId(), 10001, CampType.Enemy)
                        //{
                        //    Position = sPt,
                        //});

                        Vector3 sPt = new Vector3(hit.point.x + i + 3, hit.point.y, hit.point.z + j + 3);
                        int id = UnityEngine.Random.Range(0, 4);
                        GameEntry.Entity.ShowNPC(new NPCData(GameEntry.Entity.GenerateSerialId(), 50004 + id, CampType.Neutral)
                        {
                            Position = sPt,
                        });

                        GameEntry.Entity.ShowNPC(new NPCData(GameEntry.Entity.GenerateSerialId(), 50024, CampType.Neutral)
                        {
                            Position = sPt + new Vector3(5, 0, 5),
                            Scale = new Vector3(30, 30, 30)
                        });
                    }
                }
            }
        }

        //test 
        public void LoadResourceForOccupySuccessCallback(string assetName, object asset, float duration, object userData)
        {
            //Process Occupy Behaviour from load Replace resource
            Texture tex = asset as Texture;
            if (tex)
            {
                Texture texTemp;
                m_Textures.TryGetValue(assetName,out texTemp);
                if(texTemp)
                    OccupyBehaviour(asset, assetName, userData, m_StructureCallbacks);
                else
                {
                    m_Textures.Add(assetName, tex);
                    OccupyBehaviour(asset, assetName, userData, m_StructureCallbacks);
                }
            }
        }
        
        //test for add enemy
        public void OnThetaUI(object sender,GameEventArgs arg)
        {
            UIThetaEventArgs thetaArg = arg as UIThetaEventArgs;
            if(thetaArg != null)
            {
                Vector3 pos = new Vector3();
                GameBase.GetMainPos(out pos);
                pos += new Vector3(40, 0, 40);
                GameEntry.Entity.ShowEnemy(new NPCData(GameEntry.Entity.GenerateSerialId(), 
                    50009 /*+ UnityEngine.Random.Range(0,11)*/, CampType.Enemy)
                { 
                    Position = pos,
                    
                });
            }
        }


    }
}
