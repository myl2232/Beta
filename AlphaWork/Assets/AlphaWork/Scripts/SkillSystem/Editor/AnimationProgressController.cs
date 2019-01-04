using System;

using UnityEngine;
using UnityEditor;

namespace SkillSystem
{
  class AnimationProgressController
  {
    Func<float> m_progressGetter;
    Action<float> m_progressSetter;

    public AnimationProgressController(Func<float> progressGetter, Action<float> progressSetter)
    {
      m_progressGetter = progressGetter;
      m_progressSetter = progressSetter;

      ResetSliderLayout();
    }

    public void OnGUI()
    {
      var rect = GUILayoutUtility.GetRect(SliderXMin, 16);
      EditorGUI.LabelField(rect, new GUIContent(" AnimationProgress"));

      rect.x += SliderXMin;
      rect.x = Mathf.Max(rect.x, GUIUtility.ScreenToGUIPoint(new Vector2(SliderScreenCoordX, 0)).x);
      rect.width = SliderWidth;

      var currVal = m_progressGetter();
      var newVal = EditorGUI.Slider(rect, GUIContent.none, currVal, 0.0f, 1.0f);
      if (newVal != currVal)
        m_progressSetter(newVal);
    }

    public void ResetSliderLayout()
    {
      SliderScreenCoordX = 0.0f;
      SliderWidth = 700.0f;
    }

    public float SliderXMin
    {
      get
      {
        return 150.0f;
      }
    }

    public float SliderScreenCoordX
    {
      set;
      get;
    }

    public float SliderWidth
    {
      set;
      get;
    }
  }
}
