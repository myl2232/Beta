using GameFramework.Event;
using System.Collections.Generic;
using UnityGameFramework.Runtime;
using ProcedureOwner = GameFramework.Fsm.IFsm<GameFramework.Procedure.IProcedureManager>;

namespace AlphaWork
{
    public class ProcedureMain : ProcedureBase
    {
        public override bool UseNativeDialog
        {
            get
            {
                return false;
            }
        }

        private bool bExit = false;
        private UGUIFormExtend m_Form = null;

        protected override void OnInit(ProcedureOwner procedureOwner)
        {
            base.OnInit(procedureOwner);

            GameEntry.Event.Subscribe(OpenUIFormSuccessEventArgs.EventId, OnOpenUIFormSuccess);
            GameEntry.Event.Subscribe(GameToLoginEventArgs.EventId, OnBackToLogin);
        }

        protected override void OnDestroy(ProcedureOwner procedureOwner)
        {
            base.OnDestroy(procedureOwner);

            GameEntry.Event.Unsubscribe(GameToLoginEventArgs.EventId, OnBackToLogin);
            GameEntry.Event.Unsubscribe(OpenUIFormSuccessEventArgs.EventId, OnOpenUIFormSuccess);
            GameEntry.Config.GameSetting.gameContrller.Shutdown();
        }

        protected override void OnEnter(ProcedureOwner procedureOwner)
        {
            base.OnEnter(procedureOwner);

            bExit = false;
            GameEntry.Config.GameSetting.gameContrller.Initialize();
            GameEntry.UI.OpenUIForm(UIFormId.MainForm, this);
        }

        protected override void OnLeave(ProcedureOwner procedureOwner, bool isShutdown)
        {
            base.OnLeave(procedureOwner, isShutdown);

            if (m_Form != null)
            {
                m_Form.Close(isShutdown);
                m_Form = null;
            }

            GameEntry.Config.GameSetting.gameContrller.Shutdown();

            bExit = false;
        }

        protected override void OnUpdate(ProcedureOwner procedureOwner, float elapseSeconds, float realElapseSeconds)
        {
            base.OnUpdate(procedureOwner, elapseSeconds, realElapseSeconds);

            if (!bExit)
                GameEntry.Config.GameSetting.gameContrller.Update(elapseSeconds, realElapseSeconds);
            else
            {
                procedureOwner.SetData<VarInt>(Constant.ProcedureData.NextSceneId, (int)SceneId.Menu);
                ChangeState<ProcedureChangeScene>(procedureOwner);
            }
        }

        private void OnOpenUIFormSuccess(object sender, GameEventArgs e)
        {
            OpenUIFormSuccessEventArgs ne = (OpenUIFormSuccessEventArgs)e;
            if (ne.UserData != this)
            {
                return;
            }

            m_Form = (UGUIFormExtend)ne.UIForm.Logic;
            GameEntry.LuaScriptEngine.RegistGameObject2Lua(m_Form.CachedTransform.gameObject, "Game.InGame.ui.UIInGame");
        }

        public void OnBackToLogin(object sender, GameEventArgs e)
        {
            GameToLoginEventArgs arg = (GameToLoginEventArgs)e;
            bExit = true;
        }
    }
}
