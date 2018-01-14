using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AlphaWork
{
    public class FlashMuzzle : MonoBehaviour
    {

        protected bool bActive;
        public bool Active
        {
            get { return bActive; }
            set { bActive = value; }
        }
        private float speed;
        private float lifetime;
        private float dist;
        private float spawnTime = 0.0f;
        private Transform tr;
        private ParticleSystem pt;

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
        // Use this for initialization
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            if (!bActive)

                tr.position += tr.forward * speed * Time.deltaTime;
            dist -= speed * Time.deltaTime;

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
            pt = gameObject.AddComponent<ParticleSystem>();
        }

    }
}

