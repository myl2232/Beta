using GameFramework.Event;
using UnityGameFramework.Runtime;
using ProcedureOwner = GameFramework.Fsm.IFsm<GameFramework.Procedure.IProcedureManager>;

namespace AlphaWork
{
    public class ProcedureMenu : ProcedureBase
    {
        private bool m_StartGame = false;
        private UGUIFormExtend m_MenuForm = null;

        public override bool UseNativeDialog
        {
            get
            {
                return false;
            }
        }

        public override void Go()
        {
            base.Go();
            m_StartGame = true;
        }

        public void StartGame()
        {
            m_StartGame = true;
        }

        /// <summary>
        /// 状态初始化时调用。
        /// </summary>
        /// <param name="procedureOwner">流程持有者。</param>
        protected override void OnInit(ProcedureOwner procedureOwner)
        {
            base.OnInit(procedureOwner);
            GameEntry.Event.Subscribe(OpenUIFormSuccessEventArgs.EventId, OnOpenUIFormSuccess);
        }

        protected override void OnEnter(ProcedureOwner procedureOwner)
        {
            base.OnEnter(procedureOwner);            

            m_StartGame = false;
            GameEntry.UI.OpenUIForm(UIFormId.MenuForm, this);
        }

        protected override void OnLeave(ProcedureOwner procedureOwner, bool isShutdown)
        {
            base.OnLeave(procedureOwner, isShutdown);


            if (m_MenuForm != null)
            {
                m_MenuForm.Close(isShutdown);
                m_MenuForm = null;
            }
            m_StartGame = false;
        }
        /// <summary>
        /// 状态销毁时调用。
        /// </summary>
        /// <param name="procedureOwner">流程持有者。</param>
        protected override void OnDestroy(ProcedureOwner procedureOwner)
        {
            base.OnDestroy(procedureOwner);

            GameEntry.Event.Unsubscribe(OpenUIFormSuccessEventArgs.EventId, OnOpenUIFormSuccess);
        }

        protected override void OnUpdate(ProcedureOwner procedureOwner, float elapseSeconds, float realElapseSeconds)
        {
            base.OnUpdate(procedureOwner, elapseSeconds, realElapseSeconds);

            if (m_StartGame)
            {
                ChangeState<ProcedureLogin>(procedureOwner);
            }
            
        }

        private void OnOpenUIFormSuccess(object sender, GameEventArgs e)
        {
            OpenUIFormSuccessEventArgs ne = (OpenUIFormSuccessEventArgs)e;
            if (ne.UserData != this)
            {
                return;
            }

            m_MenuForm = (UGUIFormExtend)ne.UIForm.Logic;
            GameEntry.LuaScriptEngine.RegistGameObject2Lua(m_MenuForm.CachedTransform.gameObject, "Game.Login.ui.UIMainMenu");
        }
    }
}
