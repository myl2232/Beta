using System;

using UnityEngine;

namespace SkillSystem
{
  public static class AnimationProgressQuery
  {
    class LegacyImpl : IQuery
    {
      GameObject m_gameObj;
      string m_animClipName;

      float IQuery.GetProgress()
      {
        var animState = m_gameObj.GetComponent<Animation>()[m_animClipName];
        if (null == animState)
          return 0.0f;

        return NormalizedAnimTimeToProgress(animState.normalizedTime);
      }

      public LegacyImpl(GameObject obj, string animClipName)
      {
        m_gameObj = obj;
        m_animClipName = animClipName;
      }
    }

    class MecanimImpl : IQuery
    {
      Func<float> m_doQuery;

      float IQuery.GetProgress()
      {
        return m_doQuery();
      }

      static bool IsLoop(AnimatorClipInfo[] infos)
      {
        foreach (var info in infos)
          if (info.clip.isLooping)
            return true;

        return false;
      }

      void ConvertToNonLoopQuery()
      {
        var lastProgress = 0.0f;

        var originalQuery = m_doQuery;

        m_doQuery = () =>
        {
          var currentProgress = originalQuery();

          if (currentProgress < lastProgress)
          {
            m_doQuery = () => 0.0f;
            currentProgress = 0.0f;
          }
          else
            lastProgress = currentProgress;

          return currentProgress;
        };
      }

      // must be called before the new animator state is triggered
      public MecanimImpl(GameObject gameObj, int layerId)
      {
        var initState = gameObj.GetComponent<Animator>().GetCurrentAnimatorStateInfo(layerId).fullPathHash;

        m_doQuery = () =>
        {
          if (gameObj.GetComponent<Animator>().GetCurrentAnimatorStateInfo(layerId).fullPathHash == initState)
            return 0.0f;

          m_doQuery = () => GetAnimProgress(gameObj.GetComponent<Animator>(), layerId);

          if (!IsLoop(gameObj.GetComponent<Animator>().GetCurrentAnimatorClipInfo(layerId)))
            ConvertToNonLoopQuery();

          return m_doQuery();
        };
      }
    }

    public static float NormalizedAnimTimeToProgress(float time)
    {
      return Mathf.Repeat(time, 1.0f);
    }

    public static float GetAnimProgress(Animator animator)
    {
      return GetAnimProgress(animator, 0);
    }

    public static float GetAnimProgress(Animator animator, int layerId)
    {
      return NormalizedAnimTimeToProgress(animator.GetCurrentAnimatorStateInfo(layerId).normalizedTime);
    }

    public interface IQuery
    {
      float GetProgress();
    }

    public static IQuery MakeLegacyImpl(GameObject obj, string animClipName)
    {
      return new LegacyImpl(obj, animClipName);
    }

    public static IQuery MakeMecanimImpl(GameObject obj, int layerId)
    {
      return new MecanimImpl(obj, layerId);
    }
  }
}
