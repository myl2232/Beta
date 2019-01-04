using System;

using UnityEditor;

namespace SkillSystem
{
  abstract class EditorWindowWithGuard : EditorWindow
  {
    IState m_state;

    interface IState
    {
      void OnGUI(EditorWindowWithGuard wnd);
      void Update(EditorWindowWithGuard wnd);
      void OnDestroy(EditorWindowWithGuard wnd);
    }

    class ScriptCompiling : IState
    {
      void IState.OnGUI(EditorWindowWithGuard wnd)
      {
        GUIWrapper.Label("Please wait while script is compiling...");
      }

      void IState.Update(EditorWindowWithGuard wnd)
      {
        if (!EditorApplication.isCompiling)
          wnd.m_state = new Normal(wnd);
      }

      void IState.OnDestroy(EditorWindowWithGuard wnd)
      {

      }
    }

    class Normal : IState
    {
      Action<PlayModeStateChange> m_onPlayModeChanged;

      public Normal(EditorWindowWithGuard owner)
      {
        m_onPlayModeChanged = change => owner.Close();

        EditorApplication.playModeStateChanged += m_onPlayModeChanged;
        owner.OnEnableImpl();
      }

      void IState.OnGUI(EditorWindowWithGuard wnd)
      {
        wnd.OnGUIImpl();
      }

      void IState.Update(EditorWindowWithGuard wnd)
      {
        wnd.UpdateImpl();
      }

      void IState.OnDestroy(EditorWindowWithGuard wnd)
      {
        wnd.OnDestroyImpl();
        EditorApplication.playModeStateChanged -= m_onPlayModeChanged;
      }
    }

    void OnEnable()
    {
      m_state = new ScriptCompiling();
    }

    void OnGUI()
    {
      m_state.OnGUI(this);
    }

    void Update()
    {
      m_state.Update(this);
    }

    void OnDestroy()
    {
      m_state.OnDestroy(this);
    }

    protected abstract void OnEnableImpl();
    protected abstract void OnDestroyImpl();
    protected abstract void OnGUIImpl();
    protected abstract void UpdateImpl();
  }
}
