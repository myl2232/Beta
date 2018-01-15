using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace AlphaWork
{
    public class EffectEntity : EntityObject
    {
        private Rigidbody m_rg;
        private SphereCollider m_Collider;
        GameObject gb;
        private float speed;
        private float lifetime;
        private float dist;
        private float spawnTime = 0.0f;
        private Transform tr;

        protected bool bActive;
        public bool Active
        {
            get { return bActive; }
            set { bActive = value; }
        }
        public float Speed
        {
            get { return speed; }
            set { speed = value; }
        }
        public float LifeTime
        {
            get { return lifetime; }
            set { lifetime = value; }
        }
        public float Dist
        {
            get { return dist; }
            set { dist = value; }
        }

        void Start()
        {
            gb = Entity.Handle as GameObject;
            m_rg = gb.AddComponent<Rigidbody>();
            m_Collider = gb.AddComponent<SphereCollider>();
            m_Collider.isTrigger = true;
        }

        void Update()
        {
          if (!bActive)
          {
              tr.position += tr.forward * speed * Time.deltaTime;
              dist -= speed * Time.deltaTime;
          }

          if (dist <= 0 || lifetime <= 0)
              bActive = false;
        }

        void OnEnable()
        {
            bActive = true;
            tr = transform;
            spawnTime = Time.time;
        }

        private void OnTriggerEnter(Collider other)
        {
            bActive = false;
            GameEntry.Entity.HideEntity(this);
        }

        protected internal override void OnShow(object userdata)
        {
            base.OnShow(userdata);
            EffectData data = userdata as EffectData;
            speed = data.Speed;
            lifetime = data.LifeTime;
            transform.forward = data.Forward;
            Transform vTrans = ObjectUtility.FindChild(data.Parent, data.AttachName);
            transform.position = vTrans.position + transform.forward.normalized*4;
        }

    }
}
