using UnityEngine;
using System.Collections;
using GameFramework;

namespace AlphaWork
{
    public class GameBuilder : MonoBehaviour
    {
        [SerializeField]
        private bool m_ArMode;
        public bool ARMode
        {
            get { return m_ArMode; }
            set { value = m_ArMode; }
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

