using System;

using UnityEngine;

namespace SkillSystem
{
  class AnimationPlayer : IDisposable
  {
    GameObject m_gameObject;
    AnimationClip m_animClip;
    AnimationFunctionality.IAccess m_animationFunctionality;

    Action<GameObject> m_onSelectGameObject;
    Action<GameObject> m_onDeselectGameObject;

    Action<AnimationProgressQuery.IQuery> m_onAutoPlayingStart;
    Action m_onAutoPlayingEnd;

    State m_playerState;

    WorldTransformBackup m_worldTransformBackup;

    class WorldTransformBackup
    {
      Vector3 m_pos;
      Quaternion m_rot;

      public WorldTransformBackup(GameObject gameObject)
      {
        var trans = gameObject.transform;
        m_pos = trans.position;
        m_rot = trans.rotation;
      }

      public void Restore(GameObject gameObject)
      {
        var trans = gameObject.transform;
        trans.position = m_pos;
        trans.rotation = m_rot;
      }
    }

    void AssignGameObject(GameObject newObj)
    {
      if (m_gameObject == newObj)
        return;

      if (null != newObj && !newObj.activeInHierarchy)
        return;

      var oldObj = m_gameObject;

      try
      {
        if (null != newObj)
        {
          m_onSelectGameObject(newObj);

          EnterReadyState(newObj);
          return;
        }
        else
          EnterNotReadyState();
      }
      finally
      {
        if (null != oldObj)
          m_onDeselectGameObject(oldObj);
      }

      m_gameObject = newObj;
    }

    void AssignAnimationClip(AnimationClip animClip)
    {
      if (m_animClip == animClip)
        return;

      m_animClip = animClip;

      if (null != m_gameObject)
        EnterReadyState();
      else
        EnterNotReadyState();
    }

    void ResetAnimationFunctionality(Func<AnimationFunctionality.IAccess> factory)
    {
      if (null != m_animationFunctionality)
        m_animationFunctionality.Dispose();

      m_animationFunctionality = factory();
    }

    void ResetAnimationFunctionality()
    {
      ResetAnimationFunctionality(() => null);
    }

    abstract class State : IDisposable
    {
      protected AnimationPlayer m_player;

      public State(AnimationPlayer player)
      {
        m_player = player;
      }

      public abstract float AnimProgress
      {
        set;
        get;
      }

      public abstract void AutoPlay();

      public abstract void Pause();
      public abstract void Resume();

      protected virtual void Destroy()
      {

      }

      public void Dispose()
      {
        Destroy();
      }
    }

    class NotReady : State
    {
      public NotReady(AnimationPlayer player)
        : base(player)
      {
        player.ResetAnimationFunctionality();
      }

      public override float AnimProgress
      {
        set
        {

        }

        get
        {
          return 0.0f;
        }
      }

      public override void AutoPlay()
      {

      }

      public override void Pause()
      {

      }

      public override void Resume()
      {

      }
    }

    class Ready : State
    {
      public Ready(AnimationPlayer player)
        : base(player)
      {
        m_player.BackupWorldTransform();

        m_player.ResetAnimationFunctionality(() => AnimationFunctionality.MakeAccess(m_player.m_gameObject, m_player.m_animClip));

        m_player.RestoreWorldTransform();
      }

      public override float AnimProgress
      {
        set
        {
          m_player.EnterUserControlledState(value);
        }

        get
        {
          return 0.0f;
        }
      }

      public override void AutoPlay()
      {
        m_player.EnterAutoPlayingState();
      }

      public override void Pause()
      {

      }

      public override void Resume()
      {

      }
    }

    class AutoPlaying : State
    {
      float m_progress;
      bool m_needUpdate = true;
      bool m_stopped;
      bool m_paused;

      void UpdateAnimProgress()
      {
        if (!m_needUpdate)
          return;

        var progress = m_player.m_animationFunctionality.Progress;

        if (progress < m_progress)
        {
          if (m_player.TheAnimationClip.isLooping)
          {
            if (!m_player.Loop)
            {
              m_player.m_animationFunctionality.StopAutoPlaying();
              m_progress = 0.0f;
              m_needUpdate = false;
              m_stopped = true;
              return;
            }
          }
          else
          {
            if (m_player.Loop)
            {
              m_player.m_animationFunctionality.StopAutoPlaying();
              m_player.RestoreWorldTransform();
              m_player.m_animationFunctionality.StartAutoPlaying();
              m_progress = 0.0f;
              return;
            }
            else
            {
              m_progress = 0.0f;
              m_needUpdate = false;
              return;
            }
          }
        }

        m_progress = progress;
      }

      class AnimProgressQuery : AnimationProgressQuery.IQuery
      {
        AutoPlaying m_state;

        public AnimProgressQuery(AutoPlaying state)
        {
          m_state = state;
        }

        float AnimationProgressQuery.IQuery.GetProgress()
        {
          return m_state.m_progress;
        }
      }

      public AutoPlaying(AnimationPlayer player)
        : base(player)
      {
        player.BackupWorldTransform();
        player.m_animationFunctionality.StartAutoPlaying();

        player.m_onAutoPlayingStart(new AnimProgressQuery(this));

        GlobalObj<UpdateEventForwarder>.Instance.OnUpdate += UpdateAnimProgress;
      }

      public override float AnimProgress
      {
        set
        {
          if (!m_paused)
            m_player.EnterUserControlledState(value);
        }

        get
        {
          return m_progress;
        }
      }

      public override void AutoPlay()
      {
        if (!m_paused)
          m_player.EnterAutoPlayingState();
      }

      public override void Pause()
      {
        m_player.m_animationFunctionality.PauseAutoPlaying();

        m_paused = true;
      }

      public override void Resume()
      {
        m_paused = false;

        m_player.m_animationFunctionality.ResumeAutoPlaying();
      }

      protected override void Destroy()
      {
        m_progress = 0.0f;

        GlobalObj<UpdateEventForwarder>.Instance.OnUpdate -= UpdateAnimProgress;

        m_player.m_onAutoPlayingEnd();

        if (!m_stopped)
          m_player.m_animationFunctionality.StopAutoPlaying();

        m_player.RestoreWorldTransform();
      }
    }

    class UserControlled : State
    {
      float m_animProgress;

      void UpdatePlaybackTime()
      {
        m_player.m_animationFunctionality.Progress = m_animProgress;
      }

      public UserControlled(AnimationPlayer player, float animProgress)
        : base(player)
      {
        m_animProgress = animProgress;
        m_player.m_animationFunctionality.StartUserControlledPlaying();
        UpdatePlaybackTime();
      }

      public override float AnimProgress
      {
        set
        {
          m_animProgress = value;
          UpdatePlaybackTime();
        }

        get
        {
          return m_animProgress;
        }
      }

      public override void AutoPlay()
      {
        m_player.EnterAutoPlayingState();
      }

      public override void Pause()
      {

      }

      public override void Resume()
      {

      }

      protected override void Destroy()
      {
        m_player.m_animationFunctionality.StopUserControlledPlaying();
      }
    }

    void EnterState(Func<State> stateFactory)
    {
      m_playerState.Dispose();
      m_playerState = stateFactory();
    }

    void EnterNotReadyState()
    {
      EnterState(() => new NotReady(this));
    }

    void EnterReadyState()
    {
      EnterState(() => new Ready(this));
    }

    void EnterReadyState(GameObject newObj)
    {
      EnterState(() =>
      {
        m_gameObject = newObj;
        return new Ready(this);
      });
    }

    void EnterAutoPlayingState()
    {
      EnterState(() => new AutoPlaying(this));
    }

    void EnterUserControlledState(float animProgress)
    {
      EnterState(() => new UserControlled(this, animProgress));
    }

    void BackupWorldTransform()
    {
      m_worldTransformBackup = new WorldTransformBackup(m_gameObject);
    }

    void RestoreWorldTransform()
    {
      m_worldTransformBackup.Restore(m_gameObject);
    }

    public AnimationPlayer(Action<GameObject> onSelectGameObject,
      Action<GameObject> onDeselectGameObject)
    {
      m_onSelectGameObject = onSelectGameObject;
      m_onDeselectGameObject = onDeselectGameObject;
      m_playerState = new NotReady(this);
    }

    public GameObject TheGameObject
    {
      set
      {
        AssignGameObject(value);
      }

      get
      {
        return m_gameObject;
      }
    }

    public AnimationClip TheAnimationClip
    {
      set
      {
        AssignAnimationClip(value);
      }

      get
      {
        return m_animClip;
      }
    }

    public float AnimProgress
    {
      set
      {
        m_playerState.AnimProgress = value;
      }

      get
      {
        return m_playerState.AnimProgress;
      }
    }

    public float AnimTime
    {
      get
      {
        return null == m_animClip ? 0.0f : AnimProgress * m_animClip.length;
      }
    }

    public void AutoPlay(Action<AnimationProgressQuery.IQuery> onStart, Action onEnd)
    {
      m_onAutoPlayingStart = onStart;
      m_onAutoPlayingEnd = onEnd;

      m_playerState.AutoPlay();
    }

    public bool Loop
    {
      set;
      get;
    }

    public void Dispose()
    {
      m_playerState.Dispose();
      ResetAnimationFunctionality();
    }

    public void Pause()
    {
      m_playerState.Pause();
    }

    public void Resume()
    {
      m_playerState.Resume();
    }
  }
}
