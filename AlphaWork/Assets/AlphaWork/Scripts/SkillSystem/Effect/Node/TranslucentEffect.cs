using UnityEngine;

namespace SkillSystem
{
  [Effect]
  public class TranslucentEffect : SkillEffect
  {
    class Instance : SkillEffectInstance<TranslucentEffect>
    {
      LifetimeController.IInstance m_lifeTimeController;

      MaterialReplacement m_materialReplacement;

      protected override bool DoUpdate(SkillRuntimeContext context)
      {
        return m_lifeTimeController.Update(context);
      }

      protected override void DoDestroy()
      {
        m_materialReplacement.Dispose();
      }

      public Instance(TranslucentEffect effect, SkillRuntimeContext context)
        : base(effect)
      {
        var shader = Shader.Find("Transparent/Diffuse");

        m_materialReplacement = new MaterialReplacement(context.TheGameObject, srcMat =>
        {
          var mat = Object.Instantiate(srcMat);

          mat.shader = shader;

          var color = mat.color;
          color.a *= effect.Transparency;
          mat.color = color;

          return mat;
        },
        Object.DestroyImmediate);

        m_lifeTimeController = effect.LifetimeMode.Obj.Instantiate(context);
      }
    }

    protected override bool ShouldInstantiate(SkillRuntimeContext context)
    {
      return true;
    }

    protected override ISkillEffectInstance DoInstantiate(SkillRuntimeContext context)
    {
      return new Instance(this, context);
    }

    public TranslucentEffect()
    {
      LifetimeMode = new SerializableObject<LifetimeController.IController>(new LifetimeController.EndWhenAnimProgressReached());
      Transparency = 0.3f;
    }

    [SliderTweakable(0.0f, 1.0f)]
    public float Transparency
    {
      set;
      get;
    }

    [Polymorphic(typeof(LifetimeController.EndWhenAnimProgressReached),
     typeof(LifetimeController.EndAfterSpecifiedTime))]
    public SerializableObject<LifetimeController.IController> LifetimeMode
    {
      set;
      get;
    }
  }
}
