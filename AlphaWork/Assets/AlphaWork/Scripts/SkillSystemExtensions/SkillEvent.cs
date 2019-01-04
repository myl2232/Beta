using System;
using System.Collections.Generic;

using UnityEngine;

using SkillSystem;

namespace Galaxy
{
  [Effect]
  public class SkillEvent : SkillEffect
  {
    protected override bool ShouldInstantiate(SkillRuntimeContext context)
    {
      Action<string> handlers;

      if (s_listeners.TryGetValue(context.TheGameObject, out handlers))
        handlers(EventToken);

      return false;
    }

    protected override ISkillEffectInstance DoInstantiate(SkillRuntimeContext context)
    {
      return null;
    }

    public SkillEvent()
    {
      EventToken = "";
    }

    public string EventToken
    {
      set;
      get;
    }

    static Dictionary<GameObject, Action<string>> s_listeners = new Dictionary<GameObject, Action<string>>();

    public static void RegisterListener(GameObject gameObj, Action<string> handler)
    {
      Action<string> handlers;
      if (!s_listeners.TryGetValue(gameObj, out handlers))
      {
        s_listeners.Add(gameObj, handler);
        return;
      }
      handlers += handler;
      s_listeners[gameObj] = handlers;
    }

    public static void UnregisterListener(GameObject gameObj, Action<string> handler)
    {
      var handlers = s_listeners[gameObj];
      handlers -= handler;
      if (null == handlers)
        s_listeners.Remove(gameObj);
      else
        s_listeners[gameObj] = handlers;
    }
  }
}
