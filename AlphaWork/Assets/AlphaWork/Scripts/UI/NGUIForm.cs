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

#if UNITY_2017_3_OR_NEWER
        protected override void OnInit(object userData)
#else
        protected override internal void OnInit(object userData)
#endif
        {
            base.OnInit(userData);
        }

#if UNITY_2017_3_OR_NEWER
        protected override void OnOpen(object userData)
#else
        protected override internal void OnOpen(object userData)
#endif
        {
            base.OnOpen(userData);
           // GameEntry.UI.OpenUIForm(, userData);
        }

#if UNITY_2017_3_OR_NEWER
        protected override void OnClose(object userData)
#else
        protected override internal void OnClose(object userData)
#endif
        {
            base.OnClose(userData);
            GameEntry.UI.CloseUIForm(UIForm);
        }

#if UNITY_2017_3_OR_NEWER
        protected override void OnPause()
#else
        protected override internal void OnPause()
#endif
        {
            base.OnPause();
        }

#if UNITY_2017_3_OR_NEWER
        protected override void OnResume()
#else
        protected override internal void OnResume()
#endif
        {
            base.OnResume();
        }

#if UNITY_2017_3_OR_NEWER
        protected override void OnCover()
#else
        protected override internal void OnCover()
#endif
        {
            base.OnCover();
        }

#if UNITY_2017_3_OR_NEWER
        protected override void OnReveal()
#else
        protected override internal void OnReveal()
#endif
        {
            base.OnReveal();
        }

#if UNITY_2017_3_OR_NEWER
        protected override void OnRefocus(object userData)
#else
        protected override internal void OnRefocus(object userData)
#endif
        {
            base.OnRefocus(userData);
        }

#if UNITY_2017_3_OR_NEWER
        protected override void OnUpdate(float elapseSeconds, float realElapseSeconds)
#else
        protected override internal void OnUpdate(float elapseSeconds, float realElapseSeconds)
#endif
        {
            base.OnUpdate(elapseSeconds, realElapseSeconds);
        }
    }
}
