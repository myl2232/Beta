using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AlphaWork
{
    public class ObjectUtility : MonoBehaviour
    {

        // Use this for initialization
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        public static Transform GetRootParent(Transform trans)
        {
            if (!trans)
                return null;

            Transform parentTrans = trans.parent;
            if (!parentTrans)
                return trans;

            return GetRootParent(parentTrans);
        }

        public static Transform FindChild(Transform trans, string str)
        {
            if (!trans)
                return null;
            Transform result = trans.Find(str);
            if (!result)
            {
                int count = trans.childCount;
                for (int i = 0; i < count; ++i)
                {
                    if (result)
                        break;

                    Transform tr = trans.GetChild(i);
                    if (tr.gameObject.name == str)
                    {
                        result = tr;
                    }
                    else
                        result = FindChild(tr, str);
                }
            }
            else
                return result;

            return result;
        }

        public static UnityEngine.Object GetAssistObject()
        {
            string str = "EthanAgent";
            return ObjectUtility.GetFellow(str);
        }
        public static UnityEngine.Object GetMainPlayer()
        {
            string str = "Ethan";
            return ObjectUtility.GetFellow(str);
        }
        public static UnityEngine.Object GetTargetAgent()
        {
            string str = "TargetAgent";
            return ObjectUtility.GetFellow(str);
        }
        public static Object GetFellow(string str)
        {
            GameObject[] hings = FindObjectsOfType(typeof(GameObject)) as GameObject[];
            for (int i = 0; i < hings.Length; ++i)
            {
                if (hings[i].name == str)
                {
                    return hings[i];
                }
            }
            return null;
        }

        public static Object GetTerrain(string str)
        {
            Terrain[] terrains = FindObjectsOfType(typeof(Terrain)) as Terrain[];
            for (int i = 0; i < terrains.Length; ++i)
            {
                if (terrains[i].name == str)
                {
                    return terrains[i];
                }
            }
            return null;
        }
        
    }

}

