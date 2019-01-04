using System;
using System.Reflection;
using System.Collections.Generic;

namespace SkillSystem
{
  public static class ReflectionHelper
  {
    public static List<Type> FindClassesWithAttribute(Type attrType)
    {
      var types = new List<Type>();

      foreach (var assem in AppDomain.CurrentDomain.GetAssemblies())
      {
        foreach (var type in assem.GetTypes())
        {
          if (type.IsDefined(attrType, true))
            types.Add(type);
        }
      }

      types.Sort((x, y) => x.Name.CompareTo(y.Name));

      return types;
    }

    public static object InvokeMethod(object obj, string method, params object[] args)
    {
      return obj.GetType().GetMethod(method).Invoke(obj, args);
    }

    public static bool HasAttribute<T>(MemberInfo info)
    {
      return info.GetCustomAttributes(typeof(T), true).Length > 0;
    }

    public static T GetAttribute<T>(MemberInfo info)
      where T : class
    {
      var attrs = info.GetCustomAttributes(typeof(T), true);
      return attrs.Length > 0 ? (T)attrs[0] : null;
    }

    public static T GetPropertyValue<T>(object obj, PropertyInfo info)
    {
      return (T)GetPropertyValue(obj, info);
    }

    public static object GetPropertyValue(object obj, PropertyInfo info)
    {
      return info.GetValue(obj, null);
    }

    public static void SetPropertyValue<T>(object obj, PropertyInfo info, T val)
    {
      info.SetValue(obj, val, null);
      if (null != OnPropertyValueChanged)
        OnPropertyValueChanged(obj, info, val);
    }

    public static event Action<object, PropertyInfo, object> OnPropertyValueChanged;
  }

  public class ObjectName
  {
    const string c_propertyName = "Name";
    object m_obj;

    PropertyInfo GetNameProperty()
    {
      return m_obj.GetType().GetProperty(c_propertyName);
    }

    public static bool IsNameProperty(PropertyInfo info)
    {
      return info.Name == c_propertyName;
    }

    public static bool HasName(object obj)
    {
      return obj.GetType().GetProperty(c_propertyName) != null;
    }

    public ObjectName(object obj)
    {
      m_obj = obj;
    }

    public string Name
    {
      set
      {
        ReflectionHelper.SetPropertyValue(m_obj, GetNameProperty(), value);
      }

      get
      {
        return ReflectionHelper.GetPropertyValue<string>(m_obj, GetNameProperty());
      }
    }
  }
}
