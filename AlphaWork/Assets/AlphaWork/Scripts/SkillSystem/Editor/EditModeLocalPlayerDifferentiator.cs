using UnityEngine;

namespace SkillSystem
{
  class EditModeLocalPlayerDifferentiator : ILocalPlayerDifferentiator
  {
    bool ILocalPlayerDifferentiator.IsLocalPlayer(GameObject obj)
    {
      return true;
    }
  }
}
