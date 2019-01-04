using System;
using UnityEngine;

namespace SkillSystem
{
  public interface IParticleSystemUpdaterFactory
  {
    Func<bool> MakeParticleSystemUpdater(ParticleSystem ps);
  }
}
