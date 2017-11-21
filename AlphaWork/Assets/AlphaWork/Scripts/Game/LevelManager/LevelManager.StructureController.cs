using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityGameFramework;

namespace AlphaWork
{
    partial class LevelManager
    {
        protected StructureBehaviourCallback m_StructureCallbacks;
        protected List<int> m_StructureIds = new List<int>();
        protected List<int> m_EnemyIds = new List<int>();

        public void RegisterStructure(UnityGameFramework.Runtime.Entity ent)
        {
            if ((ent.Logic as Structure).m_TypeId == 90002)
            {
                m_EnemyIds.Add(ent.Id);
            }

            string str = "structure";
            str += (m_StructureIds.Count + 1);
            (ent.Handle as GameObject).name = str;
            m_StructureIds.Add(ent.Id);

            Structure st = ent.Logic as Structure;
            st.Weight = UnityEngine.Random.Range(1, 5);
        }


        public void OnOccupy(object asset, string assetName, object userData, bool initialized)
        {
            UnityGameFramework.Runtime.Entity ent = userData as UnityGameFramework.Runtime.Entity;
            GameObject gb = ent.Handle as GameObject;
            if (gb)
            {
                gb.GetComponent<MeshRenderer>().material.mainTexture = asset as Texture;
            }
        }
        public void OnUnOccupy(object asset, string assetName, object userData, bool destroy)
        {
            UnityGameFramework.Runtime.Entity ent = userData as UnityGameFramework.Runtime.Entity;
            GameObject gb = ent.Handle as GameObject;
            if (gb)
            {
                gb.GetComponent<MeshRenderer>().material.mainTexture = (ent.Logic as Structure).OriginalTex;
            }
        }
        public void OccupyBehaviour(object asset, string assetName,object userdata,StructureBehaviourCallback callbacks)
        {
            UnityGameFramework.Runtime.Entity ent = userdata as UnityGameFramework.Runtime.Entity;
            Structure structLogic = ent.Logic as Structure;
            if (structLogic.Occupyed)
                callbacks.occupyCallback(asset, assetName,userdata, false);
            else
                callbacks.unOccupyCallback(asset, assetName,userdata, false);
        }

        public void RefreshStructures()
        {
            if (m_StructureIds.Count <= 0)
                return;

//             UnityGameFramework.Runtime.Entity structObj = GameEntry.Entity.GetEntity(m_StructureIds[0]);
//             if (structObj)
//             {
//                 GameObject assistOb = ObjectUtility.GetAssistObject() as GameObject;
//                 GameObject structOb = structObj.Handle as GameObject;
//                 structOb.transform.position = assistOb.transform.position + new Vector3(0, 5, 0);
//             }

        }

        public void ClearStructures()
        {
            m_StructureIds.Clear();
            m_EnemyIds.Clear();
        }

    }    
}
