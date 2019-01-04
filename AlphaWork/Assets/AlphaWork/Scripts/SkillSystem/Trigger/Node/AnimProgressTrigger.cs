namespace SkillSystem
{
  [Trigger]
  public class AnimProgressTrigger : SkillEffectTrigger
  {
    class Instance : SkillEffectTriggerInstance<AnimProgressTrigger>
    {
      AnimProgressHitTester m_animProgressHitTester;

      public Instance(AnimProgressTrigger trigger, SkillRuntimeContext context)
        : base(trigger)
      {
        m_animProgressHitTester = new AnimProgressHitTester(context.AnimProgress);
      }

      protected override bool ShouldTrigger(SkillRuntimeContext context)
      {
        m_animProgressHitTester.Update(context.AnimProgress);
        return m_animProgressHitTester.Contains(TheTrigger.Progress);
      }
    }

    [SliderTweakable(0.0f, 1.0f), AnimationProgress]
    public float Progress
    {
      set;
      get;
    }

    protected override ISkillEffectTriggerInstance DoInstantiate(SkillRuntimeContext context)
    {
      return new Instance(this, context);
    }
  }
}
