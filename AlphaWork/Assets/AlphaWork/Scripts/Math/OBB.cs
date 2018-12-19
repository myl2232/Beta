using UnityEngine;

namespace AlphaWork
{
  public class OBB
  {
    public Quaternion Orientation
    {
      set;
      get;
    }

    public Vector3 HalfExtent
    {
      set;
      get;
    }

    public Vector3 Center
    {
      set;
      get;
    }

    public bool Intersect(Ray ray, out float distance)
    {
      var worldToLocal = Matrix4x4.Rotate(Quaternion.Inverse(Orientation)) * Matrix4x4.Translate(-Center);

      var localRay = new Ray(worldToLocal.MultiplyPoint3x4(ray.origin), worldToLocal.MultiplyVector(ray.direction));

      var aabb = new Bounds(Vector3.zero, 2.0f * HalfExtent);

      return aabb.IntersectRay(localRay, out distance);
    }

    public bool Intersect(Ray ray, out Vector3 intersection)
    {
      float dist;
      var r = Intersect(ray, out dist);
      intersection = ray.GetPoint(dist);
      return r;
    }
  }
}
