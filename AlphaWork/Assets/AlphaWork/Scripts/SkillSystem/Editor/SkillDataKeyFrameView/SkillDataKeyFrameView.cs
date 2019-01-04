using System;

using UnityEngine;

namespace SkillSystem
{
  class SkillDataKeyFrameView : SkillDataViewManager.IView
  {
    SkillDataViewManager.IEditingEnvironment m_editingEnv;

    SkillBrowser m_skillBrowser;

    Action<KeyFrameEffectBrowser.ListViewBuilder> m_initEffectBrowser;
    KeyFrameEffectBrowser m_effectBrowser;

    GUIWrapper.PropertyPane m_skillPropertyPane;
    GUIWrapper.PropertyPane m_effectPropertyPane;

    void SkillDataViewManager.IView.OnGUI(float width)
    {
      using (new GUIWrapper.HorizontalGroup())
      {
        using (GUIWrapper.VerticalGroup.MakeWithWidth(Mathf.Max(m_editingEnv.AnimProgressBarXMin, width / 7)))
        {
          m_skillBrowser.OnGUI();
        }

        using (new GUIWrapper.VerticalGroup())
        {
          m_effectBrowser.TryOnGUI();
        }

        using (GUIWrapper.VerticalGroup.MakeWithWidth(width / 3))
        {
          GUIWrapper.Label("");
          m_skillPropertyPane.TryOnGUI();

          GUIWrapper.Label("");
          m_effectPropertyPane.TryOnGUI();
        }
      }
    }

    void IDisposable.Dispose()
    {
      m_editingEnv.ResetAnimProgressBarLayout();
    }

    void OnSelectSkill(Skill skill)
    {
      GlobalObj<UpdateEventForwarder>.Instance.QueueTask(() =>
      {
        m_skillPropertyPane = new GUIWrapper.PropertyPane(skill);

        m_effectBrowser = new KeyFrameEffectBrowser(skill, OnSelectEffect, OnDeselectEffect,
          m_editingEnv.PlayCurrentSkill,
          m_initEffectBrowser);

        m_effectPropertyPane = null;
      });
    }

    void OnDeselectSkill()
    {
      m_effectBrowser = null;
      m_skillPropertyPane = null;
      m_effectPropertyPane = null;
    }

    void OnSelectEffect(ISkillEffect effect)
    {
      GlobalObj<UpdateEventForwarder>.Instance.QueueTask(() => m_effectPropertyPane = new GUIWrapper.PropertyPane(effect));
    }

    void OnDeselectEffect()
    {
      m_effectPropertyPane = null;
    }

    public SkillDataKeyFrameView(SkillDataViewManager.IEditingEnvironment editingEnv,
      Action<KeyFrameEffectBrowser.ListViewBuilder> initEffectBrowser)
    {
      m_editingEnv = editingEnv;

      m_skillBrowser = new SkillBrowser(skill =>
      {
        editingEnv.SetCurrentSkill(skill);
        OnSelectSkill(skill);
      },

      editingEnv.RenameSkill,

      editingEnv.TheSkillList,

      OnDeselectSkill);

      m_initEffectBrowser = initEffectBrowser;
    }
  }
}
