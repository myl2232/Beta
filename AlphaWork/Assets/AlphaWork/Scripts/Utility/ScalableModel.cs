using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace AlphaWork
{
    public class ScalableModel: MonoBehaviour
    {
        [HideInInspector]
        [SerializeField]
        private float scaleX = 1.0f;
        public float ScaleX
        {
            get { return scaleX; }
            set { scaleX = value; }
        }
        [HideInInspector]
        [SerializeField]
        private float scaleY = 1.0f;
        public float ScaleY
        {
            get { return scaleY; }
            set { scaleY = value; }
        }
        [HideInInspector]
        [SerializeField]
        private float scaleZ = 1.0f;
        public float ScaleZ
        {
            get { return scaleZ; }
            set { scaleZ = value; }
        }
        [HideInInspector]
        [SerializeField]
        private string bone;
        public string Bone
        {
            get { return bone; }
            set { bone = value; }
        }
        [HideInInspector]
        [SerializeField]
        private GameObject m_instance;
        public GameObject Instance
        {
            get { return m_instance; }
            set { m_instance = value; }
        }
        

        private void LateUpdate()
        {
            //Scale();
        }

        public void Scale()
        {
            if (m_instance)
            {
                Transform trans = FindChild(m_instance.transform, bone);
                trans.SetLocalScaleX(scaleX);
                trans.SetLocalScaleY(scaleY);
                trans.SetLocalScaleZ(scaleZ);
            }
        }

        public Transform FindChild(Transform trans, string str)
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
    }
}
