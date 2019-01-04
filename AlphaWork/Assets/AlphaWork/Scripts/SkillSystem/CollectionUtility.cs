using System.Collections.Generic;

namespace SkillSystem
{
  public static class CollectionUtility
  {
    public static void Move<T>(ref List<T> l, int oldPos, int newPos)
    {
      var val = l[oldPos];
      l.RemoveAt(oldPos);
      l.Insert(newPos > oldPos ? newPos - 1 : newPos, val);
    }
  }
}
