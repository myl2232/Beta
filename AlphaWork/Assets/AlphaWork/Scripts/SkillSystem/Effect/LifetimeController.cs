using UnityEngine;

namespace SkillSystem
{
  namespace LifetimeController
  {
    public interface IController
    {
      IInstance Instantiate(SkillRuntimeContext context);
    }

    public interface IInstance
    {
      bool Update(SkillRuntimeContext context);
    }

    public class Free : IController
    {
      class Instance : IInstance
      {
        bool IInstance.Update(SkillRuntimeContext context)
        {
          return false;
        }
      }

      IInstance IController.Instantiate(SkillRuntimeContext context)
      {
        return new Instance();
      }
    }

    public class EndWhenAnimProgressReached : IController
    {
      class Instance : IInstance
      {
        EndWhenAnimProgressReached m_def;
        AnimProgressHitTester m_animProgressHitTester = new AnimProgressHitTester(0.0f);

        bool IInstance.Update(SkillRuntimeContext context)
        {
          m_animProgressHitTester.Update(context.AnimProgress);
          return m_animProgressHitTester.Contains(m_def.EndProgress);
        }

        public Instance(EndWhenAnimProgressReached def, SkillRuntimeContext context)
        {
          m_def = def;
        }
      }

      IInstance IController.Instantiate(SkillRuntimeContext context)
      {
        return new Instance(this, context);
      }

      public EndWhenAnimProgressReached()
      {
        EndProgress = 1.0f;
      }

      [SliderTweakable(0.0f, 1.0f), AnimationProgress]
      public float EndProgress
      {
        set;
        get;
      }
    }

    public class EndAfterSpecifiedTime : IController
    {
      class Instance : IInstance
      {
        EndAfterSpecifiedTime m_def;
        float m_timer;

        bool IInstance.Update(SkillRuntimeContext context)
        {
          m_timer += Time.deltaTime;
          return m_timer >= m_def.Lifetime;
        }

        public Instance(EndAfterSpecifiedTime def)
        {
          m_def = def;
        }
      }

      IInstance IController.Instantiate(SkillRuntimeContext context)
      {
        return new Instance(this);
      }

      float m_lifetime;

      public float Lifetime
      {
        set
        {
          m_lifetime = Mathf.Max(0.0f, value);
        }

        get
        {
          return m_lifetime;
        }
      }
    }
  }
}
