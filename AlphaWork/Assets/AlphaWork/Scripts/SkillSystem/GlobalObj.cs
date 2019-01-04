using System;

namespace SkillSystem
{
  public sealed class GlobalObj<T>
  {
    public static T Instance
    {
      set;
      get;
    }
  }

  public class ScopedSetGlobalObj<T> : IDisposable
  {
    T m_oldObj;

    public ScopedSetGlobalObj(T newObj)
    {
      m_oldObj = GlobalObj<T>.Instance;
      GlobalObj<T>.Instance = newObj;
    }

    public void Dispose()
    {
      GlobalObj<T>.Instance = m_oldObj;
    }
  }
}
