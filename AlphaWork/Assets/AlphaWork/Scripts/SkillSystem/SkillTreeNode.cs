using System;
using System.Xml.Serialization;
using System.Collections.Generic;

namespace SkillSystem
{
  public class SkillTreeNode
  {
    public string Name
    {
      set;
      get;
    }

    public SkillTreeNode()
    {
      Name = GetType().Name;
    }

    public void Traverse(int depth, Action<int, object> visitor)
    {
      visitor(depth, this);
    }
  }

  public class SkillTreeNonLeafNode<ChildType> : SkillTreeNode
    where ChildType : class
  {
    [XmlArray("Children"), XmlArrayItem("Child")]
    public List<SerializableObject<ChildType>> m_children =
      new List<SerializableObject<ChildType>>();

    public void AddChild(ChildType child)
    {
      m_children.Add(new SerializableObject<ChildType>(child));
    }

    public void RemoveChild(ChildType child)
    {
      m_children.Remove(m_children.Find(slot => slot.Obj == child));
    }

    public void ForEachChild(Action<ChildType> fn)
    {
      m_children.ForEach(slot => fn(slot.Obj));
    }

    public ChildType FindChild(Predicate<ChildType> pred)
    {
      var result = m_children.Find(slot => pred(slot.Obj));
      if (null == result)
        return null;
      return result.Obj;
    }

    public int ChildCount
    {
      get
      {
        return m_children.Count;
      }
    }

    public new void Traverse(int depth, Action<int, object> visitor)
    {
      visitor(depth, this);
      m_children.ForEach(child => ReflectionHelper.InvokeMethod(child.Obj, "Traverse", depth + 1, visitor));
    }

    public void Move(ChildType child, int newPos)
    {
      CollectionUtility.Move(ref m_children, m_children.FindIndex(slot => slot.Obj == child), newPos);
    }
  }

  public class SkillTreeNonLeafNodeInstance<ChildType, ChildInstanceType>
    where ChildType : class, IInstantiatable<ChildInstanceType>
  {
    protected List<ChildInstanceType> m_children = new List<ChildInstanceType>();

    public void InstantiateChildren(SkillTreeNonLeafNode<ChildType> node, SkillRuntimeContext context)
    {
      node.ForEachChild(child => m_children.Add(child.Instantiate(context)));
    }
  }

  public interface IInstantiatable<InstanceType>
  {
    InstanceType Instantiate(SkillRuntimeContext context);
  }

  public interface ISkillEffectTriggerInstance : IDisposable
  {
    void Update(SkillRuntimeContext context);
    void Abandon(SkillRuntimeContext context);
    void Pause();
    void Resume();
  }

  public interface ISkillEffectTrigger : IInstantiatable<ISkillEffectTriggerInstance>
  {

  }

  public interface ISkillEffectInstance : IDisposable
  {
    // true : all done and will be removed
    bool Update(SkillRuntimeContext context);

    // false : the effect keeps active even though the owning skill instance is removed
    bool Abandon();

    void Pause();
    void Resume();
  }

  public interface ISkillEffect : IInstantiatable<ISkillEffectInstance>
  {
    void PreloadResources();
  }
}
