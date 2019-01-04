using System;

using UnityEngine;
using UnityEditor;

namespace SkillSystem
{
  class KeyFrameEffectBrowser
  {
    GUIWrapper.ListView m_listView;

    public class ListViewBuilder
    {
      Skill m_skill;
      Action<ISkillEffect> m_onSelectEffect;
      Action m_onDeselectEffect;
      Action m_playSkill;

      public void Build<TriggerType>(Action<TriggerType, float> keyTimeSetter,
        Func<TriggerType, float> keyTimeGetter,
        string keyTimeColumnName,
        Func<float> keyTimeMaxValue,
        Action<Rect> adjustAnimProgressBarLayout,
        Action<Skill> additionalGUI)
        where TriggerType : SkillEffectTrigger, new()
      {
        KeyFrameNodeBinder<TriggerType>.Init(m_skill, m_onSelectEffect, keyTimeSetter, keyTimeGetter);

        ListView = new GUIWrapper.ListView(columnBuilder =>
        columnBuilder(keyTimeColumnName, 5.0f, (userItem, rect) =>
        {
          var binder = userItem as KeyFrameNodeBinder<TriggerType>;

          var currentProgress = EditorGUI.Slider(rect, GUIContent.none, binder.KeyTime, 0.0f, keyTimeMaxValue());
          if (binder.IsSelected)
            binder.KeyTime = currentProgress;

          adjustAnimProgressBarLayout(rect);
        }),

        KeyFrameNodeBinder<TriggerType>.PopulateListView,

        "Add Effect...", "Remove Effect",

        KeyFrameNodeBinder<TriggerType>.ShowEffectMenu,

        () =>
        {
          m_onDeselectEffect();
          KeyFrameNodeBinder<TriggerType>.OnDeselectEffect();
        },

        () =>
        {
          GUIWrapper.Button("Play Skill", m_playSkill);
          additionalGUI(m_skill);
        });
      }

      public GUIWrapper.ListView ListView
      {
        private set;
        get;
      }

      public ListViewBuilder(Skill skill, Action<ISkillEffect> onSelectEffect,
        Action onDeselectEffect, Action playSkill)
      {
        m_skill = skill;
        m_onSelectEffect = onSelectEffect;
        m_onDeselectEffect = onDeselectEffect;
        m_playSkill = playSkill;
      }
    }

    public KeyFrameEffectBrowser(Skill skill, Action<ISkillEffect> onSelectEffect, Action onDeselectEffect,
      Action playSkill, Action<ListViewBuilder> initEffectBrowser)
    {
      var builder = new ListViewBuilder(skill, onSelectEffect, onDeselectEffect, playSkill);
      initEffectBrowser(builder);
      m_listView = builder.ListView;
    }

    public void OnGUI()
    {
      m_listView.OnGUI();
    }
  }
}
