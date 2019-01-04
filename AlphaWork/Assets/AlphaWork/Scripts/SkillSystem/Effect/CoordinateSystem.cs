using UnityEngine;

namespace SkillSystem.CoordinateSystem
{
  public interface ISpace
  {
    // Vector3 defined in world space

    Vector3 XAxis(SkillRuntimeContext context);
    Vector3 YAxis(SkillRuntimeContext context);
    Vector3 ZAxis(SkillRuntimeContext context);
  }

  public class World : ISpace
  {
    Vector3 ISpace.XAxis(SkillRuntimeContext context)
    {
      return Vector3.right;
    }

    Vector3 ISpace.YAxis(SkillRuntimeContext context)
    {
      return Vector3.up;
    }

    Vector3 ISpace.ZAxis(SkillRuntimeContext context)
    {
      return Vector3.forward;
    }
  }

  public class CurrentCamera : ISpace
  {
    Vector3 ISpace.XAxis(SkillRuntimeContext context)
    {
      return Camera.current.transform.right;
    }

    Vector3 ISpace.YAxis(SkillRuntimeContext context)
    {
      return Camera.current.transform.up;
    }

    Vector3 ISpace.ZAxis(SkillRuntimeContext context)
    {
      return Camera.current.transform.forward;
    }
  }

  public class CurrentGameObject : ISpace
  {
    Vector3 ISpace.XAxis(SkillRuntimeContext context)
    {
      return context.TheGameObject.transform.right;
    }

    Vector3 ISpace.YAxis(SkillRuntimeContext context)
    {
      return context.TheGameObject.transform.up;
    }

    Vector3 ISpace.ZAxis(SkillRuntimeContext context)
    {
      return context.TheGameObject.transform.forward;
    }
  }
}
