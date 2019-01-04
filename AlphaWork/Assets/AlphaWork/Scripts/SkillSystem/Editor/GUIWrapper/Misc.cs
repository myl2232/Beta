using System;

using UnityEngine;
using UnityEditor;

namespace SkillSystem
{
  static partial class GUIWrapper
  {
    public const string SkillEditorName = "Skill Editor";

    public class HorizontalGroup : IDisposable
    {
      public HorizontalGroup()
      {
        EditorGUILayout.BeginHorizontal();
      }

      void IDisposable.Dispose()
      {
        EditorGUILayout.EndHorizontal();
      }
    }

    public class VerticalGroup : IDisposable
    {
      VerticalGroup(params GUILayoutOption[] options)
      {
        EditorGUILayout.BeginVertical(options);
      }

      public VerticalGroup()
      {
        EditorGUILayout.BeginVertical();
      }

      public static VerticalGroup MakeWithHeight(float height)
      {
        return new VerticalGroup(GUILayout.Height(height));
      }

      public static VerticalGroup MakeWithWidth(float width)
      {
        return new VerticalGroup(GUILayout.Width(width));
      }

      void IDisposable.Dispose()
      {
        EditorGUILayout.EndVertical();
      }
    }

    public static void Button(string caption, Action onClick)
    {
      if (GUILayout.Button(caption))
        onClick();
    }

    public static void Label(string caption)
    {
      EditorGUILayout.LabelField(new GUIContent(caption));
    }

    public static void Label(string caption, string content)
    {
      EditorGUILayout.LabelField(new GUIContent(caption), new GUIContent(content));
    }

    public static void Label(string caption, float width)
    {
      EditorGUILayout.LabelField(new GUIContent(caption), GUILayout.Width(width));
    }

    public static void MessageBox(string msg)
    {
      EditorUtility.DisplayDialog(SkillEditorName, msg, "OK");
    }

    public static bool MessageBoxYesNo(string msg)
    {
      return EditorUtility.DisplayDialog(SkillEditorName, msg, "Yes", "No");
    }

    public static void PopupContextMenu(Action<Action<string, GenericMenu.MenuFunction>> populator)
    {
      GenericMenu menu = new GenericMenu();

      populator((caption, act) => menu.AddItem(new GUIContent(caption), false, act));

      menu.ShowAsContext();
    }

    public class TwoStateButton
    {
      string m_caption;
      Action m_onClick;

      public TwoStateButton(string caption, Action onClick)
      {
        m_caption = caption;
        m_onClick = onClick;
      }

      public void OnGUI()
      {
        var backup = GUI.enabled;
        GUI.enabled = Enabled;
        Button(m_caption, m_onClick);
        GUI.enabled = backup;
      }

      public bool Enabled
      {
        set;
        get;
      }
    }

    public class Toggle
    {
      string m_caption;
      bool m_state;
      Action m_onCheck;
      Action m_onUncheck;

      public Toggle(string caption, bool initChecked, Action onCheck, Action onUncheck)
      {
        m_caption = caption;
        m_state = initChecked;
        m_onCheck = onCheck;
        m_onUncheck = onUncheck;
      }

      public void OnGUI()
      {
        var newState = EditorGUILayout.Toggle(m_caption, m_state);
        if (m_state != newState)
        {
          m_state = newState;

          if (newState)
            m_onCheck();
          else
            m_onUncheck();
        }
      }
    }
  }
}
