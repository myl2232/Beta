using UnityEngine;

namespace SkillSystem.CameraShakeDamping
{
  public interface IDamping
  {
    CameraShakePattern.IPattern Make(CameraShakePattern.IPattern pattern);
  }

  public class Undamped : IDamping
  {
    CameraShakePattern.IPattern IDamping.Make(CameraShakePattern.IPattern pattern)
    {
      return pattern;
    }
  }

  public class Linear : IDamping
  {
    class Instance : CameraShakePattern.IPattern
    {
      Linear m_def;
      CameraShakePattern.IPattern m_pattern;
      float m_timer;

      public Instance(Linear def, CameraShakePattern.IPattern pattern)
      {
        m_def = def;
        m_pattern = pattern;
      }

      float CameraShakePattern.IPattern.Update()
      {
        m_timer += Time.deltaTime;
        return Mathf.Clamp01(1.0f - m_timer * m_def.m_ratioPerSec) * m_pattern.Update();
      }
    }

    CameraShakePattern.IPattern IDamping.Make(CameraShakePattern.IPattern pattern)
    {
      return new Instance(this, pattern);
    }

    float m_ratioPerSec;
    public float RatioPerSec
    {
      set
      {
        m_ratioPerSec = Mathf.Clamp01(value);
      }

      get
      {
        return m_ratioPerSec;
      }
    }
  }
}
