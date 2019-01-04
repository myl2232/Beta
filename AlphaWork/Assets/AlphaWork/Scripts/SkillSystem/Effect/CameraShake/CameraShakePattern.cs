using UnityEngine;

namespace SkillSystem.CameraShakePattern
{
  public interface IPattern
  {
    float Update();
  }

  public class SHM : IPattern
  {
    float m_a;
    float m_f;

    float m_t;

    float IPattern.Update()
    {
      var val = m_a * Mathf.Sin(2.0f * Mathf.PI * m_f * m_t);
      m_t += Time.deltaTime;
      return val;
    }

    public SHM(float amp, float freq)
    {
      m_a = amp;
      m_f = freq;
    }
  }
}
