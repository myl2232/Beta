using System;

using UnityEngine;
using UnityEditor;

namespace SkillSystem
{
  class KeyFrameNodeBinder<TriggerType> : GUIWrapper.ListView.IUserItem
    where TriggerType : SkillEffectTrigger, new()
  {
    static Skill s_skill;
    static Action<ISkillEffect> s_onSelectEffect;
    static ISkillEffect s_selectedEffect;

    static Action<TriggerType, float> s_keyTimeSetter;
    static Func<TriggerType, float> s_keyTimeGetter;

    TriggerType m_trigger;
    ISkillEffect m_effect;

    void UpdateTriggerName()
    {
      new ObjectName(m_trigger).Name = typeof(TriggerType).Name + "_" + new ObjectName(m_effect).Name;
    }

    string GUIWrapper.ListView.IUserItem.Name
    {
      set
      {
        new ObjectName(m_effect).Name = value;

        if (m_trigger.ChildCount == 1)
          UpdateTriggerName();
      }

      get
      {
        return new ObjectName(m_effect).Name;
      }
    }

    void GUIWrapper.ListView.IUserItem.OnSelected()
    {
      s_onSelectEffect(m_effect);
      s_selectedEffect = m_effect;
    }

    void GUIWrapper.ListView.IUserItem.OnRemove()
    {
      if (m_trigger.ChildCount == 1)
        s_skill.RemoveChild(m_trigger);
      else
        m_trigger.RemoveChild(m_effect);
    }

    bool GUIWrapper.ListView.IUserItem.CanRename
    {
      get
      {
        return true;
      }
    }

    bool GUIWrapper.ListView.IUserItem.CanStartDrag
    {
      get
      {
        return false;
      }
    }

    void GUIWrapper.ListView.IUserItem.Move(int newPos)
    {

    }

    static void PopulateListView(TriggerType trigger, Action<GUIWrapper.ListView.IUserItem> appender)
    {
      if (trigger.ChildCount == 0)
        return;

      trigger.ForEachChild(effect => appender(new KeyFrameNodeBinder<TriggerType>(trigger, effect)));
    }

    KeyFrameNodeBinder(TriggerType trigger, ISkillEffect effect)
    {
      m_trigger = trigger;
      m_effect = effect;
    }

    public static void Init(Skill skill, Action<ISkillEffect> onSelectEffect,
      Action<TriggerType, float> keyTimeSetter, Func<TriggerType, float> keyTimeGetter)
    {
      s_skill = skill;
      s_onSelectEffect = onSelectEffect;
      s_keyTimeSetter = keyTimeSetter;
      s_keyTimeGetter = keyTimeGetter;
    }

    public static void PopulateListView(Action<GUIWrapper.ListView.IUserItem> appender)
    {
      s_skill.ForEachChild(trigger =>
      {
        var t = trigger as TriggerType;
        if (null != t)
          PopulateListView(t, appender);
      });
      }

    public KeyFrameNodeBinder(ISkillEffect effect)
    {
      m_trigger = new TriggerType();
      m_effect = effect;

      s_skill.AddChild(m_trigger);
      m_trigger.AddChild(m_effect);
    }

    public float KeyTime
    {
      set
      {
        if (s_keyTimeGetter(m_trigger) != value)
        {
          if (m_trigger.ChildCount > 1)
          {
            m_trigger.RemoveChild(m_effect);
            m_trigger = new TriggerType();
            UpdateTriggerName();
            m_trigger.AddChild(m_effect);
            s_skill.AddChild(m_trigger);
          }

          s_keyTimeSetter(m_trigger, value);
        }
      }

      get
      {
        return s_keyTimeGetter(m_trigger);
      }
    }

    public bool IsSelected
    {
      get
      {
        return s_selectedEffect == m_effect;
      }
    }

    public static void OnDeselectEffect()
    {
      s_selectedEffect = null;
    }

    public static void ShowEffectMenu(Action<GUIWrapper.ListView.IUserItem> appender)
    {
      GUIWrapper.PopupContextMenu(addMenuItem =>
        SkillEffectTypeList.ForEach(type => addMenuItem(type.Name,
          () => appender(new KeyFrameNodeBinder<TriggerType>(Activator.CreateInstance(type) as ISkillEffect)))));
    }
  }
}
