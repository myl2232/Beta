using System;
using System.Collections.Generic;

namespace SkillSystem
{
  public class OnExitScope : IDisposable
  {
    Stack<Action> m_acts = new Stack<Action>();

    public void Dispose()
    {
      while (m_acts.Count > 0)
      {
        m_acts.Peek()();
        m_acts.Pop();
      }
    }

    public void Schedule(Action act)
    {
      m_acts.Push(act);
    }
  }
}
