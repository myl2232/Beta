
namespace SkillSystem
{
  public abstract class SkillEffectTrigger : SkillTreeNonLeafNode<ISkillEffect>, ISkillEffectTrigger
  {
    protected abstract ISkillEffectTriggerInstance DoInstantiate(SkillRuntimeContext context);

    public ISkillEffectTriggerInstance Instantiate(SkillRuntimeContext context)
    {
      return DoInstantiate(context);
    }
  }
}
