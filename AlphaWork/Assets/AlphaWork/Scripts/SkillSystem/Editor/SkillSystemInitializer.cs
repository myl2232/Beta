using UnityEditor;

namespace SkillSystem
{
  [InitializeOnLoad]
  class SkillSystemInitializer
  {
    static SkillSystemInitializer()
    {
      var am = new EditModeAssetManager();
      GlobalObj<EditModeAssetManager>.Instance = am;
      GlobalObj<AbstractAssetManager>.Instance = am;

      EditorApplication.update += SkillComponent.InEditorUpdate;
    }
  }
}
