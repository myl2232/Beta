using System;
using System.Collections.Generic;
using GameFramework;
using UnityEngine;
using UnityEngine.UI;

namespace AlphaWork
{
    class MainUIForm : NGUIForm
    {
        public void OnClickPressBack(bool back)
        {
            GameEntry.RefreshBackToLoginFlag(back);
        }
        public void OnClickSit()
        {            
            GameEntry.Event.Fire(this, new UIAlphaEventArgs());
        }
        public void OnClickHome()        
        {
            GameEntry.Event.Fire(this, new UIHomeEventArgs());
        }
        public void OnClickMain()
        {
            GameEntry.Event.Fire(this, new UIMainEventArgs());
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
                Transform transBtn = m_root.transform.Find("Camera/BackToLoginBtn");
                UIButton bt = transBtn.gameObject.GetComponent<UIButton>();
                EventDelegate clickDelegate = new EventDelegate(this, "OnClickPressBack");
                clickDelegate.parameters[0] = new EventDelegate.Parameter(true);
                bt.onClick.Add(clickDelegate);
            }

            {
                Transform SitBtn = m_root.transform.Find("Camera/SitBtn");
                UIButton bt = SitBtn.gameObject.GetComponent<UIButton>();
                EventDelegate clickDelegate = new EventDelegate(this, "OnClickSit");
                //clickDelegate.parameters[0] = new EventDelegate.Parameter(true);
                bt.onClick.Add(clickDelegate);
            }

            {
                Transform homeBtn = m_root.transform.Find("Camera/HomeEditBtn");
                UIButton bt = homeBtn.gameObject.GetComponent<UIButton>();
                EventDelegate clickDelegate = new EventDelegate(this, "OnClickHome");
                //clickDelegate.parameters[0] = new EventDelegate.Parameter(true);
                bt.onClick.Add(clickDelegate);
            }

            {
                Transform homeBtn = m_root.transform.Find("Camera/MainBtn");
                UIButton bt = homeBtn.gameObject.GetComponent<UIButton>();
                EventDelegate clickDelegate = new EventDelegate(this, "OnClickMain");
                //clickDelegate.parameters[0] = new EventDelegate.Parameter(true);
                bt.onClick.Add(clickDelegate);
            }
        }
    }
}
