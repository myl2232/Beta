using System;
using System.Collections.Generic;

using UnityEngine;

namespace SkillSystem
{
  abstract class SkillTreeNodeBinder : GUIWrapper.TreeView.IUserItem
  {
    static Action<object, List<object>> s_onSelectNode;

    protected object m_node;

    protected string GetName()
    {
      return new ObjectName(m_node).Name;
    }

    string GUIWrapper.TreeView.IUserItem.Name
    {
      set
      {
        new ObjectName(m_node).Name = Rename(value);
      }

      get
      {
        return GetName();
      }
    }

    void ModifyChildList(string methodName, object child)
    {
      ReflectionHelper.InvokeMethod(m_node, methodName, child);
    }

    void AddChild(object child)
    {
      ModifyChildList("AddChild", child);
    }

    void RemoveChild(object child)
    {
      ModifyChildList("RemoveChild", child);
    }

    void GUIWrapper.TreeView.IUserItem.OnSelected(List<GUIWrapper.TreeView.IUserItem> ancestors)
    {
      var ancestorNodes = new List<object>();
      ancestors.ForEach(item => ancestorNodes.Add((item as SkillTreeNodeBinder).m_node));
      s_onSelectNode(m_node, ancestorNodes);
    }

    void GUIWrapper.TreeView.IUserItem.OnContextClicked(GUIWrapper.TreeView.IContextMenuBuilder builder)
    {
      OnContextClickedImpl(builder);
    }

    void GUIWrapper.TreeView.IUserItem.OnAddedToParent(GUIWrapper.TreeView.IUserItem parent)
    {
      (parent as SkillTreeNodeBinder).AddChild(m_node);
    }

    void GUIWrapper.TreeView.IUserItem.OnRemovedFromParent(GUIWrapper.TreeView.IUserItem parent)
    {
      (parent as SkillTreeNodeBinder).RemoveChild(m_node);
    }

    protected virtual string Rename(string objName)
    {
      return objName;
    }

    protected abstract void OnContextClickedImpl(GUIWrapper.TreeView.IContextMenuBuilder builder);

    public static void Init(Action<object, List<object>> onSelectNode)
    {
      s_onSelectNode = onSelectNode;
    }

    public SkillTreeNodeBinder(object node)
    {
      m_node = node;
    }
  }

  class SkillListNodeBinder : SkillTreeNodeBinder
  {
    protected override void OnContextClickedImpl(GUIWrapper.TreeView.IContextMenuBuilder builder)
    {
      builder.BuildRename();

      builder.BuildAdd("Skill", () => new SkillNodeBinder(new Skill()));
    }

    public SkillListNodeBinder(object node)
      : base(node)
    {

    }
  }

  class SkillNodeBinder : SkillTreeNodeBinder
  {
    static Action s_playSkill;
    static Func<Skill, string, string> s_renameSkill;
    static List<Type> s_triggerTypes;

    protected override void OnContextClickedImpl(GUIWrapper.TreeView.IContextMenuBuilder builder)
    {
      builder.BuildCustomCommand("Play", s_playSkill);
      builder.BuildCustomCommand("Copy Name", () => GUIUtility.systemCopyBuffer = GetName());
      builder.BuildRename();
      builder.BuildRemove();

      foreach (var type in s_triggerTypes)
        builder.BuildAdd(type.Name, () => new TriggerNodeBinder(Activator.CreateInstance(type)));
    }

    protected override string Rename(string newName)
    {
      return s_renameSkill(m_node as Skill, newName);
    }

    public static void Init(Action playSkill, Func<Skill, string, string> renameSkill)
    {
      s_playSkill = playSkill;
      s_renameSkill = renameSkill;
      s_triggerTypes = ReflectionHelper.FindClassesWithAttribute(typeof(TriggerAttribute));
    }

    public SkillNodeBinder(object node)
      : base(node)
    {

    }
  }

  class TriggerNodeBinder : SkillTreeNodeBinder
  {
    protected override void OnContextClickedImpl(GUIWrapper.TreeView.IContextMenuBuilder builder)
    {
      builder.BuildRename();
      builder.BuildRemove();

      SkillEffectTypeList.ForEach(type =>
      builder.BuildAdd(type.Name, () => new EffectNodeBinder(Activator.CreateInstance(type))));
    }

    public TriggerNodeBinder(object node)
      : base(node)
    {

    }
  }

  class EffectNodeBinder : SkillTreeNodeBinder
  {
    protected override void OnContextClickedImpl(GUIWrapper.TreeView.IContextMenuBuilder builder)
    {
      builder.BuildRename();
      builder.BuildRemove();
    }

    public EffectNodeBinder(object node)
      : base(node)
    {

    }
  }
}
