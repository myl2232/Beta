using GameFramework;
using GameFramework.Event;
using System;
using System.Collections.Generic;
using UnityEngine.UI;

namespace AlphaWork
{
    public class UGUIFormExtend : UGuiForm
    {
        ProcedureBase m_Procedure = null;
        UGUIEventListenner m_listener = null;

        string uiViewlist = "Canvas/UserViewList";
        string uiCurUser = "Canvas/InputField";

        public void ProcedureImpl()
        {
            m_Procedure.Go();
        }

        public void CreateUserImpl(string name)
        {
            UPlayer player = new UPlayer();
            player.user = name;
            player.gamesetting = GameEntry.Config.GameSetting.UID;

            List<UPlayer> players;
            GameEntry.DataBase.DataDevice.GetDataByKey<UPlayer>(name, out players);
            if (players == null || players.Count <= 0)
            {
                GameEntry.DataBase.DataDevice.AddData<UPlayer>(player);
            }
            else
            {
                players[0].gamesetting = player.gamesetting;
            }
            GameEntry.Config.GameSetting.CurrentUser = name;
        }

        //for login user list
        public void FillUserView()
        {
            IEnumerator<UPlayer> iter = GameEntry.DataBase.DataDevice.GetData<UPlayer>();
            //List<PlayerSetting> users = GameEntry.Config.GameSetting.Users;
            if (iter != null)
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

        public void OnClickItem(object sender, GameEventArgs e)
        {
            ClickListItemEventArgs args = e as ClickListItemEventArgs;
            if (args != null && this.gameObject == args.ParentForm.gameObject)
            {
                RefreshUser(args.ClickItem.GetComponent<UGridItem>().GetItemText());
            }
        }

        protected override void OnInit(object userData)
        {
            base.OnInit(userData);

            m_listener = gameObject.GetOrAddComponent<UGUIEventListenner>();

            GameEntry.Event.Subscribe(ClickListItemEventArgs.EventId, OnClickItem);
        }

#if UNITY_2017_3_OR_NEWER
        protected override void OnOpen(object userData)
#else
        protected internal override void OnOpen(object userData)
#endif
        {
            base.OnOpen(userData);

            m_Procedure = (ProcedureBase)userData;
            if (m_Procedure == null)
            {
                Log.Warning("Procedure is invalid when open this UIForm.");
                return;
            }
            
        }

#if UNITY_2017_3_OR_NEWER
        protected override void OnClose(object userData)
#else
        protected internal override void OnClose(object userData)
#endif
        {
            m_Procedure = null;

            base.OnClose(userData);
        }
    }
}
