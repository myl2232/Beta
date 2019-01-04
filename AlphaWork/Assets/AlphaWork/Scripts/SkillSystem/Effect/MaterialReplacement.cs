using System;
using UnityEngine;

namespace SkillSystem
{
  class MaterialReplacement : IDisposable
  {
    OnExitScope m_resetter = new OnExitScope();

    public MaterialReplacement(GameObject obj, Func<Material, Material> materialMap, Action<Material> materialReclaimer)
    {
      foreach (var rnd in obj.GetComponentsInChildren<Renderer>(true))
      {
        var mats = new Material[rnd.sharedMaterials.Length];

        for (var i = 0; i != mats.Length; ++i)
        {
          var original = rnd.sharedMaterials[i];
          if (null != original)
            mats[i] = materialMap(original);
        }

        var originalMats = rnd.sharedMaterials;
        rnd.sharedMaterials = mats;

        m_resetter.Schedule(() =>
        {
          foreach (var mat in rnd.sharedMaterials)
            materialReclaimer(mat);

          rnd.sharedMaterials = originalMats;
        });
      }
    }

    public void Dispose()
    {
      m_resetter.Dispose();
    }
  }
}
