using System;

using UnityEngine;

namespace SkillSystem
{
  class EditModeParticleSystemUpdaterFactory : IParticleSystemUpdaterFactory
  {
    Func<bool> IParticleSystemUpdaterFactory.MakeParticleSystemUpdater(ParticleSystem ps)
    {
      ps.Play();

      Func<bool> checkSimTime;

      if (ps.main.loop)
        checkSimTime = () => false;
      else
      {
        var maxSimTime = ps.main.startDelay.constantMax + ps.main.duration + ps.main.startLifetime.constantMax;
        var timer = 0.0f;
        checkSimTime = () =>
        {
          timer += Time.deltaTime;
          return timer >= maxSimTime;
        };
      }

      return () =>
      {
        ps.Simulate(Time.deltaTime, false, false);
        return checkSimTime();
      };
    }
  }
}
