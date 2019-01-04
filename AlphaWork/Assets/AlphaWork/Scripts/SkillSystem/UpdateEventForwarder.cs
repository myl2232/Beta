using System;
using System.Collections.Generic;

namespace SkillSystem
{
  public class UpdateEventForwarder
  {
    class OnUpdateBackingField
    {
      class Entry
      {
        public Action TheDelegate
        {
          set;
          get;
        }
      }

      List<Entry> m_delegates = new List<Entry>();
      List<Entry> m_invokeList = new List<Entry>();

      public void Add(Action d)
      {
        m_delegates.Add(new Entry { TheDelegate = d });
      }

      public void Remove(Action d)
      {
        var i = m_delegates.FindIndex(e => e.TheDelegate == d);
        m_delegates[i].TheDelegate = null;
        m_delegates.RemoveAt(i);
      }

      public void Invoke()
      {
        m_invokeList.AddRange(m_delegates);

        foreach (var e in m_invokeList)
        {
          if (null != e.TheDelegate)
            e.TheDelegate();
        }

        m_invokeList.Clear();
      }
    }

    Action m_tasks;

    OnUpdateBackingField m_onUpdate = new OnUpdateBackingField();
    public event Action OnUpdate
    {
      add
      {
        m_onUpdate.Add(value);
      }

      remove
      {
        m_onUpdate.Remove(value);
      }
    }

    public void QueueTask(Action task)
    {
      m_tasks += task;
    }

    public void Update()
    {
      m_onUpdate.Invoke();

      if (null != m_tasks)
      {
        var tmp = m_tasks;
        m_tasks = null;
        tmp();
      }
    }
  }
}
