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

                GameEntry.Entity.ShowEthan(new EthanData(GameEntry.Entity.GenerateSerialId(), 80000, CampType.Player)
                {
                    Position = hit.point,//new Vector3(26, 2, 20),
                });

                //test for Efficiency
                RaycastHit hitInfo;
                Physics.Raycast(mainPos, Vector3.down, out hitInfo, 1000);
                Vector3 sPt = new Vector3(UnityEngine.Random.Range(hitInfo.point.x - 10, hitInfo.point.x + 10), 
                    1.5f, UnityEngine.Random.Range(hitInfo.point.z - 10, hitInfo.point.x + 10));
                GameEntry.Entity.ShowAvatar(new AvatarData(GameEntry.Entity.GenerateSerialId(), 80002, CampType.Player)
                {
                    Position = new Vector3(120, 1.5f, 120)/*sPt*/,
                    Rotation = Quaternion.AngleAxis(90, Vector3.left),

                });
                for (int i = 0; i < 40; ++i)
                {
//                     sPt += new Vector3(UnityEngine.Random.Range(sPt.x - 0.9f, sPt.x + 0.9f), 0,
//                         UnityEngine.Random.Range(sPt.z - 0.9f, sPt.z + 0.9f));
//                     GameEntry.Entity.ShowAvatar(new AvatarData(GameEntry.Entity.GenerateSerialId(), 80002, 
//                         CampType.Player)
//                     {
//                         Position = sPt,
//                         Rotation = Quaternion.AngleAxis(90, Vector3.left),
//                     });
                    //                     GameEntry.Entity.ShowEnemy(new NPCData(GameEntry.Entity.GenerateSerialId(), 80002, CampType.Enemy)
                    //                     {
                    //                         Position = sPt,
                    //                     });
                }
            }
        }

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
    }
}
