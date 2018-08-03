using GameFramework.Event;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;


namespace AlphaWork
{
    class ClickListItemEventArgs : GameEventArgs
    {
        public static readonly int EventId = typeof(ClickListItemEventArgs).GetHashCode();
        public override int Id
        {
            get
            {
                return EventId;
            }
        }
        private Transform m_Form;
        public Transform ParentForm
        {
            get { return m_Form; }
        }
        private GameObject m_item;
        public GameObject ClickItem
        {
            get { return m_item; }
        }
        public ClickListItemEventArgs(Transform pForm, GameObject item)
        {
            m_Form = pForm;
            m_item = item;
        }
        public override void Clear()
        {

        }
    }
}
