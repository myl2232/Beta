using UnityEngine;

namespace SkillSystem
{
  namespace TransformUpdater
  {
    public interface IUpdater
    {
      IInstance Instantiate(SkillRuntimeContext context);
    }

    public interface IInstance
    {
      void UpdateTransform(GameObject obj, SkillRuntimeContext context);
    }

    public class LocalTransform
    {
      public LocalTransform()
      {
        Scale = Vector3.one;
      }

      public Vector3 Position
      {
        set;
        get;
      }

      public Vector3 Rotation
      {
        set;
        get;
      }

      public Vector3 Scale
      {
        set;
        get;
      }

      public void Update(GameObject child, GameObject parent)
      {
        child.transform.position = parent.transform.localToWorldMatrix.MultiplyPoint3x4(Position);
        child.transform.rotation = parent.transform.rotation * Quaternion.Euler(Rotation);
        child.transform.localScale = Scale;
      }
    }

    public class BindToBone : LocalTransform, IUpdater
    {
      class Instance : IInstance
      {
        BindToBone m_def;
        GameObject m_bone;

        void IInstance.UpdateTransform(GameObject obj, SkillRuntimeContext context)
        {
          m_def.Update(obj, m_bone);
        }

        public Instance(BindToBone def, SkillRuntimeContext context)
        {
          m_def = def;
          m_bone = HierarchyPath.Locate(context.TheGameObject, def.Bone);
        }
      }

      IInstance IUpdater.Instantiate(SkillRuntimeContext context)
      {
        return new Instance(this, context);
      }

      public BindToBone()
      {
        Bone = "";
      }

      public string Bone
      {
        set;
        get;
      }
    }

    public class Inplace : LocalTransform, IUpdater
    {
      class Instance : IInstance
      {
        Inplace m_def;

        void IInstance.UpdateTransform(GameObject obj, SkillRuntimeContext context)
        {
          if (null == m_def)
            return;

          m_def.Update(obj, context.TheGameObject);

          m_def = null;
        }

        public Instance(Inplace def)
        {
          m_def = def;
        }
      }

      IInstance IUpdater.Instantiate(SkillRuntimeContext context)
      {
        return new Instance(this);
      }
    }
  }
}
