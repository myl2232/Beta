using System.Xml.Serialization;

namespace SkillSystem
{
  public static class SkillFileReader
  {
    public static SkillList Open(string skillFilePath)
    {
      using (var s = GlobalObj<AbstractAssetManager>.Instance.OpenFile(skillFilePath))
      {
        return new XmlSerializer(typeof(SkillList)).Deserialize(s) as SkillList;
      }
    }
  }
}
