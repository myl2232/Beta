using System;
using System.Collections.Generic;
using GameFramework;
using GameFramework.Event;
using UnityEngine;
using UnityEngine.UI;

namespace AlphaWork
{
    class LoginUIForm : UGuiForm
    {
        ProcedureLogin m_ProcedureLogin = null;
        string uiViewlist = "Canvas/UserViewList";
        string uiCurUser = "Canvas/InputField/Text";

        public void OnClickPressLogin(bool login)
        {
            Text txt = UIForm.transform.Find(uiCurUser).GetComponent<Text>();
            if(txt.text != "")
            {
                CreateUserImpl();
                m_ProcedureLogin.StartLogin();
            }
                
        }

        public void OnClickItem(object sender, GameEventArgs e)
        {
            ClickListItemEventArgs args = e as ClickListItemEventArgs;
            if (args != null && this.gameObject == args.ParentForm.gameObject)
            {
                RefreshUser(args.ClickItem.GetComponent<UGridItem>().GetItemText());
            }
        }

        public void FillUserView()
        {
            List<PlayerSetting> users = GameEntry.Config.GameSetting.Users;
            List<object> data = new List<object>();
            foreach(PlayerSetting setting in users)
            {
                data.Add(setting.user);
            }
            UGridListView view = UIForm.transform.Find(uiViewlist).GetComponent<UGridListView>();
            if(view)
            {
                view.SetData(data);
            }
        }

        public void RefreshUser(string name)
        {
            UIForm.transform.Find(uiCurUser).GetComponent<Text>().text = name;
        }

        protected void CreateUserImpl()
        {
#if UNITY_EDITOR || UNITY_STANDALONE_WIN
#else

#endif
        }

#if UNITY_2017_3_OR_NEWER
        protected override void OnInit(object userData)
#else
        protected override internal void OnInit(object userData)
#endif
        {
            base.OnInit(userData);

            GameEntry.Event.Subscribe(ClickListItemEventArgs.EventId, OnClickItem);
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

            m_ProcedureLogin = (ProcedureLogin)userData;
            if (m_ProcedureLogin == null)
            {
                Log.Warning("ProcedureLogin is invalid when open LoginUIForm.");
                return;
            }

        }
    }
}
