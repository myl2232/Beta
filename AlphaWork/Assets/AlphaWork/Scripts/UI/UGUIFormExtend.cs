using GameFramework;
using System;
using System.Collections.Generic;


namespace AlphaWork
{
    public class UGUIFormExtend : UGuiForm
    {
        ProcedureBase m_Procedure = null;

        string uiViewlist = "Canvas/UserViewList";
        string uiCurUser = "Canvas/InputField";

        public void ProcedureImpl()
        {
            m_Procedure.Go();
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
