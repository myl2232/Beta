using System;

namespace SkillSystem
{
  class SkillDataViewManager : IDisposable
  {
    IEditingEnvironment m_editingEnv;
    GUIWrapper.PropertyPane m_viewSwitch;
    IViewFactory m_currentFactory;
    IView m_currentView;

    void MakeView()
    {
      m_currentView = m_currentFactory.MakeView(m_editingEnv);
    }

    public interface IView : IDisposable
    {
      void OnGUI(float mainWndWidth);
    }

    public interface IEditingEnvironment
    {
      SkillList TheSkillList
      {
        get;
      }

      string RenameSkill(Skill skill, string newName);
      void SetCurrentSkill(Skill skill);
      void PlayCurrentSkill();

      float AnimProgressBarXMin
      {
        get;
      }
      void ResetAnimProgressBarLayout();
      void AdjustAnimProgressBarLayout(float x, float width);
    }

    public interface IViewFactory
    {
      IView MakeView(IEditingEnvironment env);
    }

    class AnimationProgressBasedKeyFrameView : IViewFactory
    {
      IView IViewFactory.MakeView(IEditingEnvironment env)
      {
        return SkillDataKeyFrameViewFactory.MakeAnimProgressBasedKeyFrameView(env);
      }
    }

    class TimeBasedKeyFrameView : IViewFactory
    {
      IView IViewFactory.MakeView(IEditingEnvironment env)
      {
        return SkillDataKeyFrameViewFactory.MakeTimeBasedKeyFrameView(env);
      }
    }

    class TreeView : IViewFactory
    {
      IView IViewFactory.MakeView(IEditingEnvironment env)
      {
        return new SkillDataTreeView(env);
      }
    }

    [Polymorphic(typeof(AnimationProgressBasedKeyFrameView),
      typeof(TimeBasedKeyFrameView),
      typeof(TreeView))]
    public IViewFactory SkillDataView
    {
      set
      {
        m_currentFactory = value;
        Reload();
      }

      get
      {
        return m_currentFactory;
      }
    }

    public SkillDataViewManager(IEditingEnvironment editingEnv)
    {
      m_editingEnv = editingEnv;

      m_currentFactory = new AnimationProgressBasedKeyFrameView();

      m_viewSwitch = new GUIWrapper.PropertyPane(this);

      MakeView();
    }

    public void ShowViewSwitch()
    {
      m_viewSwitch.OnGUI();
    }

    public void ShowView(float width)
    {
      m_currentView.OnGUI(width);
    }

    public void Reload()
    {
      GlobalObj<UpdateEventForwarder>.Instance.QueueTask(() =>
      {
        m_currentView.Dispose();
        MakeView();
      });
    }

    public void Dispose()
    {
      m_currentView.Dispose();
    }
  }
}
