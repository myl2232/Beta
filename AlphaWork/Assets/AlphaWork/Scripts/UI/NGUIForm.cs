using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GameFramework;
using UnityEngine;
using UnityEngine.UI;
using UnityGameFramework.Runtime;

namespace AlphaWork
{
    public abstract class NGUIForm : UIFormLogic
    {
        public UIRoot m_root;

        protected override internal void OnInit(object userData)
        {
            base.OnInit(userData);
        }

        protected override internal void OnOpen(object userData)
        {
            base.OnOpen(userData);
           // GameEntry.UI.OpenUIForm(, userData);
        }

        protected override internal void OnClose(object userData)
        {
            base.OnClose(userData);
            GameEntry.UI.CloseUIForm(UIForm);
        }

        protected override internal void OnPause()
        {
            base.OnPause();
        }

        protected override internal void OnResume()
        {
            base.OnResume();
        }

        protected override internal void OnCover()
        {
            base.OnCover();
        }

        protected override internal void OnReveal()
        {
            base.OnReveal();
        }

        protected override internal void OnRefocus(object userData)
        {
            base.OnRefocus(userData);
        }

        protected override internal void OnUpdate(float elapseSeconds, float realElapseSeconds)
        {
            base.OnUpdate(elapseSeconds, realElapseSeconds);
        }
    }
}
