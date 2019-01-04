namespace SkillSystem
{
  class AnimProgressHitTester
  {
    float m_last;
    float m_current;

    public AnimProgressHitTester(float current)
    {
      m_current = current;
    }

    public void Update(float current)
    {
      m_last = m_current;
      m_current = current;
    }

    public bool Contains(float val)
    {
      if (m_last < m_current)
        return val >= m_last && val < m_current;
      
      else if (m_last > m_current)
        return val >= m_last || val < m_current;

      return false;
    }
  }
}
