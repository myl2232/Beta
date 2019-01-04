using System;

using UnityEngine;

namespace SkillSystem
{
  static class GameObjectUtility
  {
    public static void ForEach(GameObject root, Action<GameObject> fn)
    {
      fn(root);

      foreach (Transform t in root.transform)
        ForEach(t.gameObject, fn);
    }

    public static T TryAddComponent<T>(GameObject obj)
      where T : Component
    {
      var comp = obj.GetComponent<T>();
      if (null == comp)
        comp = obj.AddComponent<T>();
      return comp;
    }

    public static void RemoveComponent<T>(GameObject obj)
      where T : Component
    {
      UnityEngine.Object.DestroyImmediate(obj.GetComponent<T>());
    }
  }
}
