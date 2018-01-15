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
    }
}
