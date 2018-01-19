using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace AlphaWork
{
    public class EffectData : EntityData
    {
          /// <summary>
          /// 伤害值。
          /// </summary>
          public float HitHP
          {
              get;
              set;
          }
          public float Speed
          {
              get;
              set;
          }
          public float LifeTime
          {
              get;
              set;
          }
          public string AssetName
          {
              get;
              set;
          }
          public string AttachName
          {
              get;
              set;
          }
          public string AttachOffset
          {
              get;
              set;
          }
          public string AttachRot
          {
              get;
              set;
          }
          public Vector3 Forward
          {
              get;
              set;
          }
          public Transform Parent
          {
              get;
              set;
          }

        public EffectData(Vector3 forward,Transform parent, int entityId = -1, int typeId = -1)
            : base(entityId, typeId)
        {
            Forward = forward;
            Parent = parent;
        }

        // Use this for initialization
        void Start()
        {
            
        }
        
        // Update is called once per frame
        void Update()
        {

        }

        public void Flush()
        {
            Transform attachTrans = AssetUtility.FindChild(Parent, AttachName);
            Vector3 rot = new Vector3();
            GetParseString(AttachRot, ref rot);
            Vector3 offset = new Vector3();
            GetParseString(AttachRot, ref offset);
            Position = attachTrans.position + offset;
            Rotation = Quaternion.Euler(rot.x, rot.y, rot.z);
        }

        private void GetParseString(string str,ref Vector3 vResult)
        {            
            str = str.Remove(0,1);
            str = str.Remove(str.Length - 1,1);
            string[] result = str.Split(',');
            vResult.x = Convert.ToSingle(result[0]);
            vResult.y = Convert.ToSingle(result[1]);
            vResult.z = Convert.ToSingle(result[2]);            
        }
    }
}
