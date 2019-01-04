using System.Collections.Generic;
using System;

namespace SkillSystem
{
  public class SkillDataCache
  {
    Dictionary<string, Dictionary<string, Skill>> m_loadedSkillData =
      new Dictionary<string, Dictionary<string, Skill>>();

    Skill m_nullSkill = new Skill
    {
      Name = "nullSkill"
    };

    public void Load(string skillFilePath, Action<Skill> onSkill)
    {
      var fileData = new Dictionary<string, Skill>();
      m_loadedSkillData.Add(skillFilePath, fileData);

      try
      {
        SkillFileReader.Open(skillFilePath).ForEachChild(child =>
        {
          fileData.Add(child.Name, child);
          onSkill(child);
        });
      }
      catch (Exception)
      {

      }
    }

    public Skill GetSkill(string skillFilePath, string skillName)
    {
      Dictionary<string, Skill> fileData;

      if (!m_loadedSkillData.TryGetValue(skillFilePath, out fileData))
      {
        Load(skillFilePath, s => { });
        return GetSkill(skillFilePath, skillName);
      }

      Skill skill;
      if (fileData.TryGetValue(skillName, out skill))
        return skill;

      return m_nullSkill;
    }

    public void Clear()
    {
      m_loadedSkillData.Clear();
    }
  }
}
