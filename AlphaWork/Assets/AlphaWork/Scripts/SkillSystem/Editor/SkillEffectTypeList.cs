using System;
using System.Collections.Generic;

namespace SkillSystem
{
  sealed class SkillEffectTypeList
  {
    static List<Type> s_effectTypes;

    static SkillEffectTypeList()
    {
      s_effectTypes = ReflectionHelper.FindClassesWithAttribute(typeof(EffectAttribute));
    }

    public static void ForEach(Action<Type> fn)
    {
      s_effectTypes.ForEach(fn);
    }
  }
}
