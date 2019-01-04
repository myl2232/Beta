using UnityEngine;
using UnityEditor;

namespace SkillSystem
{
  static class SkillDataKeyFrameViewFactory
  {
    static float MaxTime(Skill skill)
    {
      var maxTime = 0.0f;

      skill.ForEachChild(trigger =>
      {
        var timeTrigger = trigger as TimeTrigger;
        if (null != timeTrigger)
          maxTime = Mathf.Max(maxTime, timeTrigger.Time);
      });

      return maxTime;
    }

    public static SkillDataViewManager.IView MakeAnimProgressBasedKeyFrameView(SkillDataViewManager.IEditingEnvironment editingEnv)
    {
      return new SkillDataKeyFrameView(editingEnv,
        builder =>
        {
          builder.Build<AnimProgressTrigger>((trigger, keyTime) => ReflectionHelper.SetPropertyValue(trigger, typeof(AnimProgressTrigger).GetProperty("Progress"), keyTime),
            trigger => trigger.Progress,
            "Animation Progress",
            () => 1.0f,
            rect => editingEnv.AdjustAnimProgressBarLayout(GUIUtility.GUIToScreenPoint(new Vector2(rect.x, 0)).x, rect.width),
            skill => { });
        });
    }

    public static SkillDataViewManager.IView MakeTimeBasedKeyFrameView(SkillDataViewManager.IEditingEnvironment editingEnv)
    {
      var maxTime = 10.0f;

      return new SkillDataKeyFrameView(editingEnv,
        builder =>
        {
          builder.Build<TimeTrigger>((trigger, keyTime) => trigger.Time = keyTime,
            trigger => trigger.Time,
            "Time",
            () => maxTime,
            rect => { },
            skill => maxTime = Mathf.Max(EditorGUILayout.FloatField("Max Time", maxTime), MaxTime(skill)));
        });
    }
  }
}
