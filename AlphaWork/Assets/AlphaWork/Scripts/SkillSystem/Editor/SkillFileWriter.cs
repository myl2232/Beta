using System.Xml.Serialization;
using System.IO;
using System.Text;

namespace SkillSystem
{
  public static partial class SkillFileWriter
  {
    public const string SkillFileExt = "xml";

    public static void Save(string skillFilePath, SkillList skillList)
    {
      using (var fs = GlobalObj<EditModeAssetManager>.Instance.CreateFile(skillFilePath))
      {
        using (var writer = new StreamWriter(fs, Encoding.GetEncoding("utf-8")))
        {
          new XmlSerializer(typeof(SkillList)).Serialize(writer, skillList);
        }
      }
    }
  }
}
