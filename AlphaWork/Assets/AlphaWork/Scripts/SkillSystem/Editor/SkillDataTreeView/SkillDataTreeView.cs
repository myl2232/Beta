using System;

namespace SkillSystem
{
  class SkillDataTreeView : SkillDataViewManager.IView
  {
    GUIWrapper.TreeView m_skillListTreeView;
    GUIWrapper.PropertyPane m_propertyPane;

    void SkillDataViewManager.IView.OnGUI(float width)
    {
      using (new GUIWrapper.HorizontalGroup())
      {
        using (new GUIWrapper.VerticalGroup())
        {
          m_skillListTreeView.OnGUI();
        }

        using (GUIWrapper.VerticalGroup.MakeWithWidth(width / 2))
        {
          GUIWrapper.Label("");
          m_propertyPane.TryOnGUI();
        }
      }
    }

    void IDisposable.Dispose()
    {

    }

    public SkillDataTreeView(SkillDataViewManager.IEditingEnvironment editingEnv)
    {
      SkillTreeNodeBinder.Init((node, ancestors) =>
      {
        GlobalObj<UpdateEventForwarder>.Instance.QueueTask(() => m_propertyPane = new GUIWrapper.PropertyPane(node));

        ancestors.Add(node);
        var skill = ancestors.Find(obj => obj.GetType() == typeof(Skill));
        if (null != skill)
          editingEnv.SetCurrentSkill(skill as Skill);
      });

      SkillNodeBinder.Init(editingEnv.PlayCurrentSkill, editingEnv.RenameSkill);

      var binderTypes = new[]
      {
        typeof(SkillListNodeBinder),
        typeof(SkillNodeBinder),
        typeof(TriggerNodeBinder),
        typeof(EffectNodeBinder)
      };

      m_skillListTreeView = new GUIWrapper.TreeView(builder =>
      {
        editingEnv.TheSkillList.Traverse(0, (depth, node) =>
        {
          builder(depth, (GUIWrapper.TreeView.IUserItem)Activator.CreateInstance(
            binderTypes[depth], node));
        });
      });
    }
  }
}
