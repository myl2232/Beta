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

                for (int i = 0; i < 5; ++i)
                {
                    RaycastHit hitInfo;
                    Vector3 startPt = new Vector3(UnityEngine.Random.Range(mainPos.x + 10, mainPos.x + 50), 100f,
                        UnityEngine.Random.Range(mainPos.z + 10, mainPos.x + 50));

                    Physics.Raycast(startPt,
                        Vector3.down, out hitInfo, 1000);

                    GameEntry.Entity.ShowEnemy(new NPCData(GameEntry.Entity.GenerateSerialId(), 50001, CampType.Enemy)
                    {
                        Position = hitInfo.point,
                    });
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
