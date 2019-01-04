using System.Collections.Generic;

using UnityEngine;

namespace SkillSystem
{
  public static class HierarchyPath
  {
    public static GameObject RootOf(GameObject obj)
    {
      var trans = obj.transform;

      while (null != trans.parent)
        trans = trans.parent;

      return trans.gameObject;
    }

    public static string Build(GameObject root, GameObject descendant)
    {
      var trans = descendant.transform;

      var nodeNames = new Stack<string>();

      while (root != trans.gameObject)
      {
        nodeNames.Push(trans.name);
        trans = trans.parent;
      }

      var result = "";

      if (nodeNames.Count > 0)
      {
        for (; ; )
        {
          result += nodeNames.Pop();
          if (nodeNames.Count == 0)
            break;

          result += "/";
        }
      }

      return result;
    }

    public static GameObject Locate(GameObject rootObj, string path)
    {
      var trans = rootObj.transform;

      foreach (var node in path.Split('/'))
      {
        trans = trans.Find(node);
        if (null == trans)
          return rootObj;
      }

      return trans.gameObject;
    }
  }
}
