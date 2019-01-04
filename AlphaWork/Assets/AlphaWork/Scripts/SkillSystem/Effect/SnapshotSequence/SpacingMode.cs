using UnityEngine;

namespace SkillSystem.SnapshotSequenceSpacingModeImpl
{
  class ByTimeInterval : SnapshotSequence.ISpacingMode
  {
    float m_interval;
    float m_timer;

    public ByTimeInterval(float interval)
    {
      m_interval = interval;
      m_timer = interval;
    }

    bool SnapshotSequence.ISpacingMode.ShouldTakeSnapshot()
    {
      if (m_timer >= m_interval)
      {
        m_timer -= m_interval;
        return true;
      }

      m_timer += Time.deltaTime;
      return false;
    }
  }
}
