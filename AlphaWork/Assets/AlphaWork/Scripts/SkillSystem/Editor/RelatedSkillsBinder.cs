using System;

namespace SkillSystem
{
  class RelatedSkillsBinder
  {
    GUIWrapper.PropertyPane m_modeSwitch;

    public interface IBindingMode
    {
      void ForEach(Skill skill, SkillFileList fileList, Action<Skill> fn);
    }

    class LookUpInAllOpenedFilesBySkillName : IBindingMode
    {
      void IBindingMode.ForEach(Skill skill, SkillFileList fileList, Action<Skill> fn)
      {
        fileList.ForEachSkill(skl =>
        {
          if (skl.Name == skill.Name)
            fn(skl);
        });
      }
    }

    class DontLookUp : IBindingMode
    {
      void IBindingMode.ForEach(Skill skill, SkillFileList fileList, Action<Skill> fn)
      {
        fn(skill);
      }
    }

    [Polymorphic(typeof(LookUpInAllOpenedFilesBySkillName), typeof(DontLookUp))]
    public IBindingMode RelatedPreviewSkills
    {
      set;
      get;
    }

    public RelatedSkillsBinder()
    {
      RelatedPreviewSkills = new LookUpInAllOpenedFilesBySkillName();

      m_modeSwitch = new GUIWrapper.PropertyPane(this);
    }

    public void OnGUI()
    {
      m_modeSwitch.OnGUI();
    }
  }
}
