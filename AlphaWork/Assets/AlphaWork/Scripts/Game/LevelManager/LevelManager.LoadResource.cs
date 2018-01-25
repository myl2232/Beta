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
				Vector3 mainPos = Camera.main.transform.position + Camera.main.transform.forward * 20;
				//new Vector3(33, 100, 48);
				RaycastHit hit;
                Physics.Raycast(mainPos, Vector3.down, out hit, 1000);

                GameEntry.Entity.ShowEthan(new EthanData(GameEntry.Entity.GenerateSerialId(), 50004/*80000*/, CampType.Player)
                {
                    Position = hit.point,
                });

                //GameEntry.Entity.ShowNPC(new NPCData(GameEntry.Entity.GenerateSerialId(), 50004, CampType.Player2)
                //{
                //    Position = hit.point + new Vector3(3.0f, 3.0f, 3.0f),//new Vector3(26, 2, 20),
                //});

                //test for Efficiency
                Vector3 sPt = new Vector3(UnityEngine.Random.Range(hit.point.x, hit.point.x + 30),
                    hit.point.y, UnityEngine.Random.Range(hit.point.z, hit.point.x + 30));

                for (int i = 0; i < 1; ++i)
                {
                    for (int j = 0; j < 1; ++j)
                    {
                        //sPt = new Vector3(128 + i * 8 + j * 5, 0.5f, 128 + j * 4);
                        GameEntry.Entity.ShowAvatar(new AvatarData(GameEntry.Entity.GenerateSerialId(), 10001, CampType.Enemy)
                        {
                            Position = sPt,
                        });
                    }
                }

                //                 GameEntry.Entity.ShowEnemy(new NPCData(GameEntry.Entity.GenerateSerialId(), 50004, CampType.Enemy)
                //                 {
                //                     Position = sPt + new Vector3(5, 0, 5),
                //                     Scale = new Vector3(3.0f, 3.0f, 3.0f),
                //                 });

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
                GameEntry.Entity.ShowEnemy(new NPCData(GameEntry.Entity.GenerateSerialId(), 
                    50009 /*+ UnityEngine.Random.Range(0,11)*/, CampType.Enemy)
                { 
                    Position = pos + new Vector3(5,0,5),
                });
            }
        }
    }
}
