using System;

using UnityEngine;

namespace SkillSystem
{
  class PlayModeParticleSystemUpdaterFactory : IParticleSystemUpdaterFactory
  {
    Func<bool> IParticleSystemUpdaterFactory.MakeParticleSystemUpdater(ParticleSystem ps)
    {
      return () => ps.isStopped;
    }
  }
}
