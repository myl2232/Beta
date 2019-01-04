using System;
using System.Collections.Generic;

using UnityEngine;

namespace SkillSystem
{
  public class CameraShakeManager
  {
    List<PositionalContributor> m_positionalContributors = new List<PositionalContributor>();
    List<RotationalContributor> m_rotationalContributors = new List<RotationalContributor>();

    Vector3 m_camPosBackup;
    Vector3 m_camRotbackup;

    class PositionalContributor
    {
      Func<Vector3> m_axisGetter;
      CameraShakePattern.IPattern m_shakePattern;

      public PositionalContributor(Func<Vector3> axisGetter, CameraShakePattern.IPattern shakePattern)
      {
        m_axisGetter = axisGetter;
        m_shakePattern = shakePattern;
      }

      public Vector3 Update()
      {
        return m_axisGetter() * m_shakePattern.Update();
      }
    }

    class RotationalContributor
    {
      Func<Vector3> m_axisGetter;
      CameraShakePattern.IPattern m_shakePattern;

      public RotationalContributor(Func<Vector3> axisGetter, CameraShakePattern.IPattern shakePattern)
      {
        m_axisGetter = axisGetter;
        m_shakePattern = shakePattern;
      }

      public Quaternion Update()
      {
        return Quaternion.AngleAxis(m_shakePattern.Update(), m_axisGetter());
      }
    }

    class CustomEffectInstance : IEffectBuilder, IDisposable
    {
      CameraShakeManager m_mgr;
      List<PositionalContributor> m_positionalContributorRefs = new List<PositionalContributor>();
      List<RotationalContributor> m_rotationalContributorRefs = new List<RotationalContributor>();

      void IEffectBuilder.BuildPositionalShake(Func<Vector3> axisGetter, CameraShakePattern.IPattern shakePattern)
      {
        var c = new PositionalContributor(axisGetter, shakePattern);
        m_mgr.m_positionalContributors.Add(c);
        m_positionalContributorRefs.Add(c);
      }

      void IEffectBuilder.BuildRotationalShake(Func<Vector3> axisGetter, CameraShakePattern.IPattern shakePattern)
      {
        var c = new RotationalContributor(axisGetter, shakePattern);
        m_mgr.m_rotationalContributors.Add(c);
        m_rotationalContributorRefs.Add(c);
      }

      void IDisposable.Dispose()
      {
        m_positionalContributorRefs.ForEach(c => m_mgr.m_positionalContributors.Remove(c));
        m_rotationalContributorRefs.ForEach(c => m_mgr.m_rotationalContributors.Remove(c));
      }

      public CustomEffectInstance(CameraShakeManager mgr)
      {
        m_mgr = mgr;
      }
    }

    void OnPreRender(Camera cam)
    {
      m_camPosBackup = cam.transform.position;
      m_camRotbackup = cam.transform.eulerAngles;

      var posShake = Vector3.zero;
      m_positionalContributors.ForEach(c => posShake += c.Update());

      cam.transform.Translate(posShake, Space.World);

      var rotShake = Quaternion.identity;
      m_rotationalContributors.ForEach(c => rotShake *= c.Update());

      cam.transform.Rotate(rotShake.eulerAngles, Space.World);
    }

    void OnPostRender(Camera cam)
    {
      cam.transform.position = m_camPosBackup;
      cam.transform.eulerAngles = m_camRotbackup;
    }

    public CameraShakeManager()
    {
      Camera.onPreRender += OnPreRender;
      Camera.onPostRender += OnPostRender;
    }

    public interface IEffectBuilder
    {
      // the axis is defined in world space
      void BuildPositionalShake(Func<Vector3> axisGetter, CameraShakePattern.IPattern shakePattern);
      void BuildRotationalShake(Func<Vector3> axisGetter, CameraShakePattern.IPattern shakePattern);
    }

    public IDisposable MakeEffect(Action<IEffectBuilder> effectConstructor)
    {
      var inst = new CustomEffectInstance(this);
      effectConstructor(inst);
      return inst;
    }
  }
}
