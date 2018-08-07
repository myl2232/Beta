using System;
using System.Collections.Generic;
using GameFramework.Event;
using UnityGameFramework.Runtime;
using ProcedureOwner = GameFramework.Fsm.IFsm<GameFramework.Procedure.IProcedureManager>;

namespace AlphaWork
{
    public class ProcedureLogin : ProcedureBase
    {
        public override bool UseNativeDialog
        {
            get
            {
                return false;
            }
        }
        private bool m_Go = false;
        private bool m_Back = false;
        private LoginUIForm m_Form = null;
        private int m_nextSceneId = -1;

        public void Go()
        {
            m_Go = true;
        }

        public void OnBackToLogin(object sender, GameEventArgs e)
        {
            GameToLoginEventArgs arg = (GameToLoginEventArgs)e;
            m_nextSceneId = (int)SceneId.Menu;
            m_Back = true;
        }
        /// <summary>
        /// 状态初始化时调用。
        /// </summary>
        /// <param name="procedureOwner">流程持有者。</param>
        protected override void OnInit(ProcedureOwner procedureOwner)
        {
            base.OnInit(procedureOwner);
            GameEntry.Event.Subscribe(GameToLoginEventArgs.EventId, OnBackToLogin);
            GameEntry.Event.Subscribe(OpenUIFormSuccessEventArgs.EventId, OnOpenUIFormSuccess);
        }

        /// <summary>
        /// 进入状态时调用。
        /// </summary>
        /// <param name="procedureOwner">流程持有者。</param>
        protected override void OnEnter(ProcedureOwner procedureOwner)
        {
            base.OnEnter(procedureOwner);
            
            m_Go = false;
            m_Back = false;
            GameEntry.UI.OpenUIForm(UIFormId.LoginForm, this);
            m_nextSceneId = GameEntry.Config.MainScene;//procedureOwner.GetData<VarInt>(Constant.ProcedureData.NextSceneId).Value;
        }

        /// <summary>
        /// 状态轮询时调用。
        /// </summary>
        /// <param name="procedureOwner">流程持有者。</param>
        /// <param name="elapseSeconds">逻辑流逝时间，以秒为单位。</param>
        /// <param name="realElapseSeconds">真实流逝时间，以秒为单位。</param>
        protected override void OnUpdate(ProcedureOwner procedureOwner, float elapseSeconds, float realElapseSeconds)
        {
            base.OnUpdate(procedureOwner, elapseSeconds, realElapseSeconds);

            if (m_Go)
            {
                int sceneId = (int)SceneId.Undefined;
                if (GameEntry.Config.GameSetting.ArMode)
                    sceneId = (int)SceneId.Default;
                else
                    sceneId = m_nextSceneId;
 

                procedureOwner.SetData<VarInt>(Constant.ProcedureData.NextSceneId, sceneId);
                procedureOwner.SetData<VarInt>(Constant.ProcedureData.GameMode, (int)GameEntry.Config.GameSetting.gameMode);

                ChangeState<ProcedureChangeScene>(procedureOwner);
            }
            else if(m_Back)
            {
                ChangeState<ProcedureMenu>(procedureOwner);
                m_Back = false;
            }
        }

        /// <summary>
        /// 离开状态时调用。
        /// </summary>
        /// <param name="procedureOwner">流程持有者。</param>
        /// <param name="isShutdown">是否是关闭状态机时触发。</param>
        protected override void OnLeave(ProcedureOwner procedureOwner, bool isShutdown)
        {
            base.OnLeave(procedureOwner, isShutdown);

            if (m_Form != null)
            {
                m_Form.Close(isShutdown);
                m_Form = null;
            }

        }

        /// <summary>
        /// 状态销毁时调用。
        /// </summary>
        /// <param name="procedureOwner">流程持有者。</param>
        protected override void OnDestroy(ProcedureOwner procedureOwner)
        {
            base.OnDestroy(procedureOwner);
            GameEntry.Event.Unsubscribe(GameToLoginEventArgs.EventId, OnBackToLogin);
            GameEntry.Event.Unsubscribe(OpenUIFormSuccessEventArgs.EventId, OnOpenUIFormSuccess);
        }

        private void OnOpenUIFormSuccess(object sender, GameEventArgs e)
        {
            OpenUIFormSuccessEventArgs ne = (OpenUIFormSuccessEventArgs)e;
            if (ne.UserData != this)
            {
                return;
            }

            m_Form = (LoginUIForm)ne.UIForm.Logic;
            m_Form.FillUserView();
        }

    }
}
