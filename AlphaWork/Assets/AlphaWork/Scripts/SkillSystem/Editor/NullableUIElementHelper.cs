namespace SkillSystem
{
  static class NullableUIElementHelper
  {
    public static void TryOnGUI(this GUIWrapper.PropertyPane p)
    {
      if (null == p)
        GUIWrapper.Label("");
      else
        p.OnGUI();
    }

    public static void TryOnGUI(this KeyFrameEffectBrowser b)
    {
      if (null == b)
        GUIWrapper.Label("");
      else
        b.OnGUI();
    }
  }
}
