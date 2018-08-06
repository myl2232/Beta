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
        string uiCurUser = "Canvas/InputField";

        public void OnClickPressLogin(bool login)
        {
            InputField txt = UIForm.transform.Find(uiCurUser).GetComponent<InputField>();
            if(txt.text != "")
            {
                CreateUserImpl(txt.text);
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
            IEnumerator<UPlayer> iter = GameEntry.DataBase.DataDevice.GetData<UPlayer>();
            //List<PlayerSetting> users = GameEntry.Config.GameSetting.Users;
            if(iter != null)
            {
                List<object> data = new List<object>();
                while (iter.MoveNext())
                {
                    data.Add(iter.Current.user);
                }
                UGridListView view = UIForm.transform.Find(uiViewlist).GetComponent<UGridListView>();
                if (view)
                {
                    view.SetData(data);
                }
            }

        }

        public void RefreshUser(string name)
        {
            InputField info = UIForm.transform.Find(uiCurUser).transform.GetComponent<InputField>();
            info.text = name;
        }

        protected void CreateUserImpl(string name)
        {
            UPlayer player = new UPlayer();
            player.user = name;
            player.gamesetting = GameEntry.Config.GameSetting.UID;

            IEnumerator<UPlayer> playerIter;            
            GameEntry.DataBase.DataDevice.GetDataByKey<UPlayer>(name,out playerIter);
            if(playerIter == null || playerIter.Current == null)
            {
                GameEntry.DataBase.DataDevice.AddData<UPlayer>(player);
            }
            else
            {
                playerIter.Current.gamesetting = player.gamesetting;
            }
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
