using System;
using System.Reflection;

using UnityEngine;
using UnityEditor;

namespace SkillSystem
{
  class SkillEditor : EditorWindowWithGuard
  {
    SkillFileList m_fileList;
    RelatedSkillsBinder m_relatedSkillsBinder;
    AnimationPlayer m_animationPlayer;
    SkillAnimationBinder m_skillAnimationBinder;
    IViewManagerAccess m_viewManagerAccess;
    AnimationProgressController m_animProgressController;
    ScopedSetGlobalObj<ILocalPlayerDifferentiator> m_scopedSetLocalPlayerDifferentiator;
    Skill m_currentSkill;

    interface IViewManagerAccess : IDisposable
    {
      void ShowViewSwitch();
      void ShowView(float width);
      void Reload();
    }

    class NullViewManagerAccess : IViewManagerAccess
    {
      void IDisposable.Dispose()
      {

      }

      void IViewManagerAccess.ShowViewSwitch()
      {

      }

      void IViewManagerAccess.ShowView(float width)
      {

      }

      void IViewManagerAccess.Reload()
      {

      }
    }

    class NormalViewManagerAccess : IViewManagerAccess
    {
      SkillDataViewManager m_viewManager;

      void IDisposable.Dispose()
      {
        m_viewManager.Dispose();
      }

      void IViewManagerAccess.ShowViewSwitch()
      {
        m_viewManager.ShowViewSwitch();
      }

      void IViewManagerAccess.ShowView(float width)
      {
        m_viewManager.ShowView(width);
      }

      void IViewManagerAccess.Reload()
      {
        m_viewManager.Reload();
      }

      public NormalViewManagerAccess(SkillEditor editor)
      {
        m_viewManager = new SkillDataViewManager(new EditingEnvironment(editor));
      }
    }

    class EditingEnvironment : SkillDataViewManager.IEditingEnvironment
    {
      SkillEditor m_skillEidtor;

      SkillList SkillDataViewManager.IEditingEnvironment.TheSkillList
      {
        get
        {
          return m_skillEidtor.m_fileList.ActiveSkillList;
        }
      }

      string SkillDataViewManager.IEditingEnvironment.RenameSkill(Skill skill, string newName)
      {
        return m_skillEidtor.RenameSkill(skill, newName);
      }

      void SkillDataViewManager.IEditingEnvironment.SetCurrentSkill(Skill skill)
      {
        m_skillEidtor.OnSelectSkill(skill);
      }

      void SkillDataViewManager.IEditingEnvironment.PlayCurrentSkill()
      {
        m_skillEidtor.PlaySkill();
      }

      float SkillDataViewManager.IEditingEnvironment.AnimProgressBarXMin
      {
        get
        {
          return m_skillEidtor.m_animProgressController.SliderXMin;
        }
      }

      void SkillDataViewManager.IEditingEnvironment.ResetAnimProgressBarLayout()
      {
        m_skillEidtor.m_animProgressController.ResetSliderLayout();
      }

      void SkillDataViewManager.IEditingEnvironment.AdjustAnimProgressBarLayout(float screenCoordX, float width)
      {
        m_skillEidtor.m_animProgressController.SliderScreenCoordX = screenCoordX;
        m_skillEidtor.m_animProgressController.SliderWidth = width;
      }

      public EditingEnvironment(SkillEditor skillEditor)
      {
        m_skillEidtor = skillEditor;
      }
    }

    protected override void OnEnableImpl()
    {
      if (!EditorApplication.isPlaying)
        GlobalObj<IParticleSystemUpdaterFactory>.Instance = new EditModeParticleSystemUpdaterFactory();

      m_viewManagerAccess = new NullViewManagerAccess();

      Action<IViewManagerAccess> assignAccess = access =>
      {
        m_viewManagerAccess.Dispose();
        m_viewManagerAccess = access;
      };

      m_fileList = new SkillFileList(() =>
      {
        if (m_viewManagerAccess is NullViewManagerAccess)
          assignAccess(new NormalViewManagerAccess(this));
        else
          m_viewManagerAccess.Reload();
      },
        () => assignAccess(new NullViewManagerAccess()));

      m_relatedSkillsBinder = new RelatedSkillsBinder();

      m_animationPlayer = MakeAnimationPlayer();

      m_skillAnimationBinder = new SkillAnimationBinder();

      m_scopedSetLocalPlayerDifferentiator =
        new ScopedSetGlobalObj<ILocalPlayerDifferentiator>(new EditModeLocalPlayerDifferentiator());

      m_animProgressController = new AnimationProgressController(
        () => m_animationPlayer.AnimProgress,
        OnUserModifyAnimProgress);

      ReflectionHelper.OnPropertyValueChanged += OnUserModifyPropertyValue;
    }

    protected override void OnDestroyImpl()
    {
      m_viewManagerAccess.Dispose();

      m_fileList.Dispose();

      m_animationPlayer.TheGameObject = null;
      m_animationPlayer.Dispose();

      m_scopedSetLocalPlayerDifferentiator.Dispose();

      ReflectionHelper.OnPropertyValueChanged -= OnUserModifyPropertyValue;
    }

    protected override void OnGUIImpl()
    {
      using (GUIWrapper.VerticalGroup.MakeWithHeight(position.height / 8))
      {
        m_fileList.OnGUI();
      }

      m_viewManagerAccess.ShowViewSwitch();

      m_relatedSkillsBinder.OnGUI();

      m_animationPlayer.TheGameObject = EditorGUILayout.ObjectField("PreviewGameObject",
        m_animationPlayer.TheGameObject, typeof(GameObject), true) as GameObject;

      m_skillAnimationBinder.OnGUI();

      m_animationPlayer.Loop = EditorGUILayout.Toggle("LoopAnimation", m_animationPlayer.Loop);

      GUIWrapper.Label("AnimationTime", m_animationPlayer.AnimTime + "s");

      m_animProgressController.OnGUI();

      m_viewManagerAccess.ShowView(position.width);
    }

    protected override void UpdateImpl()
    {
      // HACK : force MonoBehaviour.Update (SkillComponent.Update) to be called every frame in editing mode
      if (!EditorApplication.isPlaying && null != m_animationPlayer.TheGameObject)
      {
        m_animationPlayer.TheGameObject.transform.Translate(Vector3.one);
        m_animationPlayer.TheGameObject.transform.Translate(-Vector3.one);
      }

      Repaint();
    }

    void OnSkillPaused()
    {
      m_animationPlayer.Pause();
    }

    void OnSkillResumed()
    {
      m_animationPlayer.Resume();
    }

    AnimationPlayer MakeAnimationPlayer()
    {
      return new AnimationPlayer(gameObject =>
      {
        GameObjectUtility.TryAddComponent<GameObjectAvailabilityProbe>(gameObject).OnNotAvailable += OnDestroyGameObject;
        var c = GameObjectUtility.TryAddComponent<SkillComponent>(gameObject);

        c.OnPaused += OnSkillPaused;
        c.OnResumed += OnSkillResumed;
      },

      gameObject =>
      {
        gameObject.GetComponent<GameObjectAvailabilityProbe>().OnNotAvailable -= OnDestroyGameObject;

        var c = gameObject.GetComponent<SkillComponent>();

        c.OnPaused -= OnSkillPaused;
        c.OnResumed -= OnSkillResumed;

        GameObjectUtility.RemoveComponent<SkillComponent>(gameObject);
        GameObjectUtility.RemoveComponent<GameObjectAvailabilityProbe>(gameObject);
      }
      );
    }

    void OnDestroyGameObject()
    {
      m_animationPlayer.Dispose();
      m_animationPlayer = MakeAnimationPlayer();
    }

    string RenameSkill(Skill skill, string name)
    {
      if (null != m_fileList.ActiveSkillList.FindChild(skl => skl != skill && skl.Name == name))
        return name + "_" + Guid.NewGuid().ToString("N");

      return name;
    }

    void PlaySkill()
    {
      if (null == m_animationPlayer.TheGameObject)
      {
        GUIWrapper.MessageBox("Please choose a GameObject !");
        return;
      }

      UpdateSkillAnimationBinding();

      m_animationPlayer.AutoPlay(animProgressQuery => m_relatedSkillsBinder.RelatedPreviewSkills.ForEach(m_currentSkill, m_fileList,
      skill => m_animationPlayer.TheGameObject.GetComponent<SkillComponent>().Play(skill, animProgressQuery)),

      () => m_animationPlayer.TheGameObject.GetComponent<SkillComponent>().StopAll());
    }

    void OnUserModifyAnimProgress(float progress)
    {
      UpdateSkillAnimationBinding();
      m_animationPlayer.AnimProgress = progress;
    }

    void OnUserModifyPropertyValue(object node, PropertyInfo propInfo, object newVal)
    {
      if (ReflectionHelper.HasAttribute<AnimationProgressAttribute>(propInfo))
        OnUserModifyAnimProgress((float)newVal);
    }

    void OnSelectSkill(Skill skill)
    {
      m_currentSkill = skill;
    }

    void UpdateSkillAnimationBinding()
    {
      var animClip = m_skillAnimationBinder.GetAnimationClip(m_currentSkill.Name);
      if (m_animationPlayer.TheAnimationClip != animClip)
        m_animationPlayer.TheAnimationClip = animClip;
    }

    [MenuItem("Window/" + GUIWrapper.SkillEditorName)]
    static void OnClickMenuItem()
    {
      GetWindow<SkillEditor>(GUIWrapper.SkillEditorName);
    }

    [MenuItem("GameObject/Copy Hierarchy Path", priority = -1)]
    static void CopyHierarchyPath()
    {
      GUIUtility.systemCopyBuffer = HierarchyPath.Build(HierarchyPath.RootOf(Selection.activeGameObject), Selection.activeGameObject);
    }

    [MenuItem("GameObject/Copy Hierarchy Path", true)]
    static bool EnableCopyHierarchyPath()
    {
      return Selection.gameObjects.Length == 1;
    }
  }
}
