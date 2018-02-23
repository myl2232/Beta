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
                object tg = ObjectUtility.GetFellow("TargetMain");         
                //add main actor
                Vector3 mainPos = Camera.main.transform.position + Camera.main.transform.forward * 20;
                if (tg != null)
                    mainPos = (tg as GameObject).transform.position + Vector3.up*10;
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
                        Vector3 sPt = new Vector3(hit.point.x + 2*i, hit.point.y, hit.point.z + 2*j);
                        int id = UnityEngine.Random.Range(0, 4);
                        GameEntry.Entity.ShowNPC(new NPCData(GameEntry.Entity.GenerateSerialId(), 50004 + id, CampType.Neutral)
                        {
                            Position = sPt,
                        });

                        GameEntry.Entity.ShowNPC(new NPCData(GameEntry.Entity.GenerateSerialId(), 50024, CampType.Neutral)
                        {
                            Position = sPt + new Vector3(0, 0, 10),
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
                TargetPoint Tg = ObjectUtility.GetAnyObjectofType<TargetPoint>();
                if(Tg != null)
                {
                    GameEntry.Entity.ShowEnemy(new NPCData(GameEntry.Entity.GenerateSerialId(),
                        50009 /*+ UnityEngine.Random.Range(0,11)*/, CampType.Enemy)
                    {
                        Position = Tg.transform.position,
                    });
                }
            }
        }

    }
}
