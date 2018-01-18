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
        [SerializeField]
        private bool m_useNavGrid = true;
        public bool UseNavGrid
        {
            get { return m_useNavGrid; }
            set { m_useNavGrid = value; }
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

