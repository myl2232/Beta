using System;

using UnityEngine;

namespace SkillSystem
{
  [ExecuteInEditMode]
  public class GameObjectAvailabilityProbe : MonoBehaviour
  {
    void OnDisable()
    {
      if (null != OnNotAvailable)
        OnNotAvailable();
    }

    public event Action OnNotAvailable;
  }
}
