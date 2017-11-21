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

        protected override internal void OnInit(object userData)
        {
            base.OnInit(userData);

            m_root = gameObject.GetComponent<UIRoot>();
        }

        protected internal override void OnClose(object userData)
        {
            //base.OnClose(userData);
            
        }

        protected override internal void OnOpen(object userData)
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
