using System;

namespace SkillSystem
{
  class SkillBrowser
  {
    GUIWrapper.ListView m_listView;

    class NodeBinder : GUIWrapper.ListView.IUserItem
    {
      static Action<Skill> s_onSelectSkill;
      static Func<Skill, string, string> s_renameSkill;
      static SkillList s_skillList;

      Skill m_skill;

      string GUIWrapper.ListView.IUserItem.Name
      {
        set
        {
          new ObjectName(m_skill).Name = s_renameSkill(m_skill, value);
        }

        get
        {
          return new ObjectName(m_skill).Name;
        }
      }

      void GUIWrapper.ListView.IUserItem.OnSelected()
      {
        s_onSelectSkill(m_skill);
      }

      void GUIWrapper.ListView.IUserItem.OnRemove()
      {
        s_skillList.RemoveChild(m_skill);
      }

      public static void Init(Action<Skill> onSelectSkill,
        Func<Skill, string, string> renameSkill,
        SkillList skillList)
      {
        s_onSelectSkill = onSelectSkill;
        s_renameSkill = renameSkill;
        s_skillList = skillList;
      }

      public NodeBinder(Skill skill)
      {
        m_skill = skill;
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
          return true;
        }
      }

      void GUIWrapper.ListView.IUserItem.Move(int newPos)
      {
        s_skillList.Move(m_skill, newPos);
      }
    }

    public SkillBrowser(Action<Skill> setCurrentSkill,
      Func<Skill, string, string> renameSkill,
      SkillList skillList,
      Action onDeselectSkill)
    {
      NodeBinder.Init(setCurrentSkill, renameSkill, skillList);

      m_listView = new GUIWrapper.ListView(appender =>
      skillList.ForEachChild(skill => appender(new NodeBinder(skill))),

      "Add Skill", "Remove Skill",

      appender =>
        {
          var skill = new Skill();
          skillList.AddChild(skill);
          appender(new NodeBinder(skill));
        },

      onDeselectSkill,

      () => { });
    }

    public void OnGUI()
    {
      m_listView.OnGUI();
    }
  }
}
