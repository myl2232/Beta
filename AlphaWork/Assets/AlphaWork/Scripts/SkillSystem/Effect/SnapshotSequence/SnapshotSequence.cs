using System;
using System.Collections.Generic;

using UnityEngine;

namespace SkillSystem
{
  public class SnapshotSequence : IDisposable
  {
    Parameters m_params;
    List<Snapshot> m_snapshots = new List<Snapshot>();
    Action<Material, float> m_updateAlpha;
    Func<bool> m_doUpdate;

    class Snapshot : IDisposable
    {
      float m_age;
      Mesh m_mesh = new Mesh();
      Matrix4x4 m_worldMat;
      AbstractAssetManager.IInstanceHandle<Material> m_material;

      public Snapshot(Parameters p, SkinnedMeshRenderer meshRenderer)
      {
        meshRenderer.BakeMesh(m_mesh);

        m_worldMat = meshRenderer.transform.localToWorldMatrix;

        m_material = GlobalObj<AbstractAssetManager>.Instance.Instantiate(p.SnapshotMaterial);
      }

      public void Dispose()
      {
        m_material.Dispose();
        UnityEngine.Object.DestroyImmediate(m_mesh);
      }

      public bool Update(SnapshotSequence owner)
      {
        if (m_age >= owner.m_params.SnapshotLifetime)
        {
          Dispose();
          return true;
        }

        Graphics.DrawMesh(m_mesh, m_worldMat, m_material.Instance, owner.m_params.TheGameObject.layer);

        owner.m_updateAlpha(m_material.Instance, m_age);

        m_age += Time.deltaTime;
        return false;
      }
    }

    void UpdateSnapshotInstances()
    {
      m_snapshots.RemoveAll(s => s.Update(this));
    }

    public void Stop()
    {
      m_doUpdate = () =>
      {
        UpdateSnapshotInstances();
        return 0 == m_snapshots.Count;
      };
    }

    public bool Update()
    {
      return m_doUpdate();
    }

    public interface ISpacingMode
    {
      bool ShouldTakeSnapshot();
    }

    public class Parameters
    {
      public GameObject TheGameObject
      {
        set;
        get;
      }

      public Material SnapshotMaterial
      {
        set;
        get;
      }

      public float SnapshotLifetime
      {
        set;
        get;
      }

      public ISpacingMode SpacingMode
      {
        set;
        get;
      }
    }

    public SnapshotSequence(Parameters p)
    {
      m_params = p;

      if (p.SnapshotMaterial.HasProperty("_Color"))
      {
        var initialColor = p.SnapshotMaterial.GetColor("_Color");

        m_updateAlpha = (material, age) =>
        {
          var c = initialColor;
          c.a *= (1.0f - age / p.SnapshotLifetime);
          material.SetColor("_Color", c);
        };
      }
      else
        m_updateAlpha = (material, age) => { };

      m_doUpdate = () =>
      {
        if (m_params.SpacingMode.ShouldTakeSnapshot())
        {
          foreach (var smr in m_params.TheGameObject.GetComponentsInChildren<SkinnedMeshRenderer>())
            m_snapshots.Add(new Snapshot(m_params, smr));
        }

        UpdateSnapshotInstances();
        return false;
      };
    }

    public void Dispose()
    {
      foreach (var s in m_snapshots)
        s.Dispose();
    }
  }
}
