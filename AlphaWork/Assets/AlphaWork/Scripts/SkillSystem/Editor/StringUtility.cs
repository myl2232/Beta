using System;
using System.IO;

namespace SkillSystem
{
  static class StringUtility
  {
    public static bool CaseInsensitiveEquals(string first, string second)
    {
      return string.Equals(first, second, StringComparison.CurrentCultureIgnoreCase);
    }

    public static bool CaseInsensitiveMatchExtension(string path, params string[] exts)
    {
      foreach (var ext in exts)
        if (CaseInsensitiveEquals(Path.GetExtension(path), "." + ext))
          return true;

      return false;
    }

    public static string FormatFilePath(string filePath)
    {
      return filePath.Replace("\\", "/").
        Replace("//", "/").
        Replace("///", "/").
        Replace("\\\\", "/");
    }
  }
}
