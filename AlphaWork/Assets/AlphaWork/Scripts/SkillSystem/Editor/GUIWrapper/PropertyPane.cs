using System;
using System.Reflection;
using System.Collections.Generic;

using UnityEngine;
using UnityEditor;

namespace SkillSystem
{
  static partial class GUIWrapper
  {
    public class PropertyPane
    {
      interface IField
      {
        void OnGUI(object owningObj);
      }

      class NullField : IField
      {
        void IField.OnGUI(object owningObj)
        {

        }
      }

      class SimpleField<T> : IField
      {
        Func<GUIContent, T, T> m_guiFunc;
        Func<T, T, bool> m_valueNotEqual;
        PropertyInfo m_propInfo;

        void IField.OnGUI(object owningObj)
        {
          var oldVal = ReflectionHelper.GetPropertyValue<T>(owningObj, m_propInfo);
          var newVal = m_guiFunc(new GUIContent(m_propInfo.Name), oldVal);

          if (m_valueNotEqual(oldVal, newVal))
            ReflectionHelper.SetPropertyValue(owningObj, m_propInfo, newVal);
        }

        public SimpleField(Func<GUIContent, T, T> guiFunc, Func<T, T, bool> valNotEqual, PropertyInfo propInfo)
        {
          m_guiFunc = guiFunc;
          m_valueNotEqual = valNotEqual;
          m_propInfo = propInfo;
        }
      }

      IField MakeSimpleField<T>(Func<GUIContent, T, T> guiFunc, PropertyInfo propInfo)
        where T : IEquatable<T>
      {
        return new SimpleField<T>(guiFunc, (l, r) => !l.Equals(r), propInfo);
      }

      IField MakeSimpleFieldVector3(Func<GUIContent, Vector3, Vector3> guiFunc, PropertyInfo propInfo)
      {
        return new SimpleField<Vector3>(guiFunc, (l, r) => l != r, propInfo);
      }

      class AssetField : IField
      {
        UnityEngine.Object m_intermediateObject;
        Type m_intermediateObjectType;
        bool m_allowSceneObjs;
        PropertyInfo m_propInfo;

        void IField.OnGUI(object owningObj)
        {
          var obj = EditorGUILayout.ObjectField(m_propInfo.Name, m_intermediateObject,
            m_intermediateObjectType, m_allowSceneObjs);

          if (obj != m_intermediateObject)
          {
            m_intermediateObject = obj;

            ReflectionHelper.SetPropertyValue(owningObj, m_propInfo,
              null != obj ? GlobalObj<EditModeAssetManager>.Instance.GetAssetLocator(obj) : new AssetLocator());
          }
        }

        public AssetField(Type objType,
          bool allowSceneObjs,
          object owningObj,
          PropertyInfo propInfo)
        {
          m_intermediateObject = GlobalObj<EditModeAssetManager>.Instance.LoadAsset(
            ReflectionHelper.GetPropertyValue<AssetLocator>(owningObj, propInfo), objType);

          m_intermediateObjectType = objType;

          m_allowSceneObjs = allowSceneObjs;

          m_propInfo = propInfo;
        }
      }

      class FolderField : IField
      {
        string m_folder;
        PropertyInfo m_propInfo;

        string GetPropertyValue(object owningObj)
        {
          return ReflectionHelper.GetPropertyValue<string>(owningObj, m_propInfo);
        }

        void IField.OnGUI(object owningObj)
        {
          using (new HorizontalGroup())
          {
            Label(m_propInfo.Name, m_folder);

            Button("Select Folder...",
              () =>
              {
                var folder = EditorUtility.OpenFolderPanel("Select Folder", Application.dataPath, "");
                if (folder.Length > 0)
                  m_folder = folder;
              });

            if (GetPropertyValue(owningObj) != m_folder)
              ReflectionHelper.SetPropertyValue(owningObj, m_propInfo, m_folder);
          }
        }

        public FolderField(object owningObj, PropertyInfo propInfo)
        {
          m_propInfo = propInfo;
          m_folder = GetPropertyValue(owningObj);
        }
      }

      abstract class PolymorphicField : IField
      {
        protected PropertyInfo m_propInfo;
        Type[] m_typeList;
        string[] m_displayedOptions;
        int m_selectedIndex;

        PropertyPane m_objPropertyPane;

        void UpdatePropertyPane(object obj)
        {
          m_objPropertyPane = new PropertyPane(obj);
        }

        protected abstract object GetCurrentObject(object owningObj);

        protected abstract void SetCurrentObject(object owningObj, object realObj);

        void IField.OnGUI(object owningObj)
        {
          var index = EditorGUILayout.Popup(m_propInfo.Name, m_selectedIndex, m_displayedOptions);
          if (m_selectedIndex != index)
          {
            m_selectedIndex = index;

            var obj = Activator.CreateInstance(m_typeList[index]);
            SetCurrentObject(owningObj, obj);

            GlobalObj<UpdateEventForwarder>.Instance.QueueTask(() => UpdatePropertyPane(obj));
          }

          m_objPropertyPane.ShowPropertiesIndent();
        }

        public PolymorphicField(Type[] typeList,
          object owningObj,
          PropertyInfo propInfo)
        {
          m_propInfo = propInfo;
          m_typeList = typeList;

          m_displayedOptions = new string[typeList.Length];
          for (var i = 0; i != m_displayedOptions.Length; ++i)
            m_displayedOptions[i] = typeList[i].Name;

          var obj = GetCurrentObject(owningObj);
          m_selectedIndex = Array.IndexOf(typeList, obj.GetType());

          UpdatePropertyPane(obj);
        }
      }

      class SerializablePolymorphicField : PolymorphicField
      {
        void GetSerializableObj(object owningObj, out object serObj, out PropertyInfo propInfo)
        {
          serObj = ReflectionHelper.GetPropertyValue(owningObj, m_propInfo);
          propInfo = serObj.GetType().GetProperty("Obj");
        }

        protected override object GetCurrentObject(object owningObj)
        {
          object serializableObj;
          PropertyInfo propInfo;
          GetSerializableObj(owningObj, out serializableObj, out propInfo);

          return ReflectionHelper.GetPropertyValue(serializableObj, propInfo);
        }

        protected override void SetCurrentObject(object owningObj, object realObj)
        {
          object serializableObj;
          PropertyInfo propInfo;
          GetSerializableObj(owningObj, out serializableObj, out propInfo);

          ReflectionHelper.SetPropertyValue(serializableObj, propInfo, realObj);
        }

        public SerializablePolymorphicField(Type[] typeList,
          object owningObj,
          PropertyInfo propInfo)
          : base(typeList, owningObj, propInfo)
        {

        }

        public static bool IsSerializableObject(PropertyInfo propInfo)
        {
          return propInfo.PropertyType.IsGenericType &&
            propInfo.PropertyType.GetGenericTypeDefinition() == typeof(SerializableObject<>);
        }
      }

      class NormalPolymorphicField : PolymorphicField
      {
        protected override object GetCurrentObject(object owningObj)
        {
          return ReflectionHelper.GetPropertyValue(owningObj, m_propInfo);
        }

        protected override void SetCurrentObject(object owningObj, object realObj)
        {
          ReflectionHelper.SetPropertyValue(owningObj, m_propInfo, realObj);
        }

        public NormalPolymorphicField(Type[] typeList,
          object owningObj,
          PropertyInfo propInfo)
          : base(typeList, owningObj, propInfo)
        {

        }
      }

      class SliderTweakableField : IField
      {
        float m_min;
        float m_max;
        PropertyInfo m_propInfo;

        void IField.OnGUI(object owningObj)
        {
          var oldVal = ReflectionHelper.GetPropertyValue<float>(owningObj, m_propInfo);
          var newVal = EditorGUILayout.Slider(new GUIContent(m_propInfo.Name), oldVal, m_min, m_max);

          if (oldVal != newVal)
            ReflectionHelper.SetPropertyValue(owningObj, m_propInfo, newVal);
        }

        public SliderTweakableField(float min, float max, PropertyInfo propInfo)
        {
          m_min = min;
          m_max = max;
          m_propInfo = propInfo;
        }
      }

      object m_obj;
      List<IField> m_fields = new List<IField>();

      void ShowPropertiesIndent()
      {
        m_fields.ForEach(field =>
        {
          using (new HorizontalGroup())
          {
            Label("", 16);
            field.OnGUI(m_obj);
          }
        });
      }

      IField MakeField(PropertyInfo info)
      {
        var type = info.PropertyType;

        {
          var attr = ReflectionHelper.GetAttribute<PolymorphicAttribute>(info);
          if (null != attr)
          {
            if (SerializablePolymorphicField.IsSerializableObject(info))
              return new SerializablePolymorphicField(attr.TypeList, m_obj, info);
            else
              return new NormalPolymorphicField(attr.TypeList, m_obj, info);
          }
        }

        {
          var attr = ReflectionHelper.GetAttribute<AssetAttribute>(info);
          if (null != attr)
            return new AssetField(attr.AssetType, false, m_obj, info);
        }

        if (typeof(bool) == type)
          return MakeSimpleField<bool>((content, val) => EditorGUILayout.Toggle(content, val), info);

        if (typeof(float) == type)
        {
          var attr = ReflectionHelper.GetAttribute<SliderTweakableAttribute>(info);
          if (null != attr)
            return new SliderTweakableField(attr.Min, attr.Max, info);

          return MakeSimpleField<float>((content, val) => EditorGUILayout.FloatField(content, val), info);
        }

        if (typeof(int) == type)
          return MakeSimpleField<int>((content, val) => EditorGUILayout.IntField(content, val), info);

        if (typeof(Vector3) == type)
          return MakeSimpleFieldVector3((content, val) => EditorGUILayout.Vector3Field(content, val), info);

        if (typeof(string) == type)
        {
          if (ObjectName.IsNameProperty(info))
            return new NullField();

          var attr = ReflectionHelper.GetAttribute<FolderAttribute>(info);
          if (null != attr)
            return new FolderField(m_obj, info);

          return MakeSimpleField<string>((content, val) => EditorGUILayout.TextField(content, val), info);
        }

        return new NullField();
      }

      public PropertyPane(object obj)
      {
        m_obj = obj;

        foreach (var info in obj.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public))
          if (info.CanWrite)
            m_fields.Add(MakeField(info));
      }

      public void OnGUI()
      {
        if (ObjectName.HasName(m_obj))
          Label(new ObjectName(m_obj).Name + " (" + m_obj.GetType().Name + ")");

        m_fields.ForEach(field => field.OnGUI(m_obj));
      }
    }
  }
}
