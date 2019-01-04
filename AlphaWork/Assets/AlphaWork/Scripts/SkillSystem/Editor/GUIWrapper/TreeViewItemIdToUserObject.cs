using System;
using System.Collections.Generic;

namespace SkillSystem
{
  class TreeViewItemIdToUserObject<T>
  {
    static int s_idGen;

    List<int> m_ids = new List<int>();
    List<T> m_objs = new List<T>();

    int IndexOf(int id)
    {
      return m_ids.IndexOf(id);
    }

    public T this[int id]
    {
      get
      {
        return m_objs[IndexOf(id)];
      }
    }

    public int Id(int i)
    {
      return m_ids[i];
    }

    public int Add(T obj)
    {
      int id;
      checked
      {
        id = s_idGen++;
      }

      m_ids.Add(id);
      m_objs.Add(obj);

      return id;
    }

    public int Remove(int id)
    {
      var i = IndexOf(id);

      m_ids.RemoveAt(i);
      m_objs.RemoveAt(i);

      return i;
    }

    public int Count
    {
      get
      {
        return m_ids.Count;
      }
    }

    public void ForEach(Action<int, T> fn)
    {
      for (var i = 0; i != m_ids.Count; ++i)
        fn(m_ids[i], m_objs[i]);
    }

    public IEnumerator<T> GetEnumerator()
    {
      foreach (var o in m_objs)
        yield return o;
    }

    public void Move(int id, int newPos)
    {
      var oldPos = IndexOf(id);
      CollectionUtility.Move(ref m_ids, oldPos, newPos);
      CollectionUtility.Move(ref m_objs, oldPos, newPos);
    }
  }
}
