using System;
using System.Collections.Generic;
using GameFramework;
using UnityEngine;
using UnityEngine.UI;

namespace AlphaWork
{
    class LoginUIForm : NGUIForm
    {
        public void OnClickPressLogin(bool login)
        {
            GameEntry.RefreshLoginFlag(login);
        }

#if UNITY_2017_3_OR_NEWER
        protected override void OnInit(object userData)
#else
        protected override internal void OnInit(object userData)
#endif
        {
            base.OnInit(userData);

            m_root = gameObject.GetComponent<UIRoot>();
        }

#if UNITY_2017_3_OR_NEWER
        protected override void OnClose(object userData)
#else
        protected internal override void OnClose(object userData)
#endif
        {
            //base.OnClose(userData);
            
        }

#if UNITY_2017_3_OR_NEWER
        protected override void OnOpen(object userData)
#else
        protected override internal void OnOpen(object userData)
#endif
        {
            base.OnOpen(userData);

            //for(int i = 0; i < m_root.transform.FindChild("SpriteBtn"); ++i)
            {
                Transform transBtn = m_root.transform.Find("Camera/SpriteBtn");
                UIButton bt = transBtn.gameObject.GetComponent<UIButton>();
                EventDelegate clickDelegate = new EventDelegate(this, "OnClickPressLogin");
                clickDelegate.parameters[0] = new EventDelegate.Parameter(true);
                bt.onClick.Add(clickDelegate);
            }
            
        }
    }
}
