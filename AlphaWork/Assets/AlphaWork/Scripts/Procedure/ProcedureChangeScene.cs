using GameFramework;
using GameFramework.DataTable;
using GameFramework.Event;
using UnityGameFramework.Runtime;
using ProcedureOwner = GameFramework.Fsm.IFsm<GameFramework.Procedure.IProcedureManager>;

namespace AlphaWork
{
    public partial class ProcedureChangeScene : ProcedureBase
    {
        public override bool UseNativeDialog
        {
            get
            {
                return false;
            }
        }

        private int m_nextSceneId = -1;
        private bool m_IsChangeSceneComplete = false;
        private int m_BackgroundMusicId = 0;
        private NavigationHelper m_navHelper = null;//myl

        protected override void OnEnter(ProcedureOwner procedureOwner)
        {
            base.OnEnter(procedureOwner);

            m_IsChangeSceneComplete = false;

            GameEntry.Event.Subscribe(LoadSceneSuccessEventArgs.EventId, OnLoadSceneSuccess);
            GameEntry.Event.Subscribe(LoadSceneFailureEventArgs.EventId, OnLoadSceneFailure);
            GameEntry.Event.Subscribe(LoadSceneUpdateEventArgs.EventId, OnLoadSceneUpdate);
            GameEntry.Event.Subscribe(LoadSceneDependencyAssetEventArgs.EventId, OnLoadSceneDependencyAsset);

            // 停止所有声音
            GameEntry.Sound.StopAllLoadingSounds();
            GameEntry.Sound.StopAllLoadedSounds();

            // 隐藏所有实体
            GameEntry.Entity.HideAllLoadingEntities();
            GameEntry.Entity.HideAllLoadedEntities();

            // 卸载所有场景
            string[] loadedSceneAssetNames = GameEntry.Scene.GetLoadedSceneAssetNames();
            for (int i = 0; i < loadedSceneAssetNames.Length; i++)
            {
                GameEntry.Scene.UnloadScene(loadedSceneAssetNames[i]);
            }

            // 还原游戏速度
            GameEntry.Base.ResetNormalGameSpeed();

            // 卸载navigation
            if(GameEntry.Config.GameSetting.UseNavGrid)
            {
                GameEntry.NavGrid.Close();
            }
            else
            {
                if (m_navHelper != null)
                    m_navHelper.CloseNavigation();
            }

            m_nextSceneId = procedureOwner.GetData<VarInt>(Constant.ProcedureData.NextSceneId).Value;
            
            IDataTable<DRScene> dtScene = GameEntry.DataTable.GetDataTable<DRScene>();
            DRScene drScene = dtScene.GetDataRow(m_nextSceneId);
            if (drScene == null)
            {
                Log.Warning("Can not load scene '{0}' from data table.", m_nextSceneId.ToString());
                return;
            }

            GameEntry.Config.GameSetting.CurrentSceneId = m_nextSceneId;
            string sceneName = AssetUtility.GetSceneAsset(drScene.AssetName);
            if (!GameEntry.Scene.SceneIsLoaded(sceneName))
                GameEntry.Scene.LoadScene(AssetUtility.GetSceneAsset(drScene.AssetName), this);

            m_BackgroundMusicId = drScene.BackgroundMusicId;
        }

        protected override void OnLeave(ProcedureOwner procedureOwner, bool isShutdown)
        {
            GameEntry.Event.Unsubscribe(LoadSceneSuccessEventArgs.EventId, OnLoadSceneSuccess);
            GameEntry.Event.Unsubscribe(LoadSceneFailureEventArgs.EventId, OnLoadSceneFailure);
            GameEntry.Event.Unsubscribe(LoadSceneUpdateEventArgs.EventId, OnLoadSceneUpdate);
            GameEntry.Event.Unsubscribe(LoadSceneDependencyAssetEventArgs.EventId, OnLoadSceneDependencyAsset);

            // 卸载navigation
            if (GameEntry.Config.GameSetting.UseNavGrid)
            {                
                //GameEntry.NavGrid.Close();
            }
            else
            {
                if (m_navHelper != null)
                    m_navHelper.CloseNavigation();
            }
            
            base.OnLeave(procedureOwner, isShutdown);
        }

        protected override void OnUpdate(ProcedureOwner procedureOwner, float elapseSeconds, float realElapseSeconds)
        {
            base.OnUpdate(procedureOwner, elapseSeconds, realElapseSeconds);

            if (!m_IsChangeSceneComplete)
            {
                return;
            }

            if ((int)SceneId.Menu == m_nextSceneId)
            {
                ChangeState<ProcedureMenu>(procedureOwner);
            }
            else if((int)SceneId.YeWai == m_nextSceneId)
            {
                ChangeState<ProcedureMain>(procedureOwner);
            }
        }

        private void OnLoadSceneSuccess(object sender, GameEventArgs e)
        {
            LoadSceneSuccessEventArgs ne = (LoadSceneSuccessEventArgs)e;
            if (ne.UserData != this)
            {
                return;
            }

            Log.Info("Load scene '{0}' OK.", ne.SceneAssetName);

            m_IsChangeSceneComplete = true;

            // navigation
            if (GameEntry.Config.GameSetting.UseNavGrid)
                GameEntry.NavGrid.ReadData();//Nav_Grid
            else
                m_navHelper = new NavigationHelper(ne.SceneAssetName);//Recast_Nav
            
        }

        private void OnLoadSceneFailure(object sender, GameEventArgs e)
        {
            LoadSceneFailureEventArgs ne = (LoadSceneFailureEventArgs)e;
            if (ne.UserData != this)
            {
                return;
            }

            Log.Error("Load scene '{0}' failure, error message '{1}'.", ne.SceneAssetName, ne.ErrorMessage);
        }

        private void OnLoadSceneUpdate(object sender, GameEventArgs e)
        {
            LoadSceneUpdateEventArgs ne = (LoadSceneUpdateEventArgs)e;
            if (ne.UserData != this)
            {
                return;
            }

            Log.Info("Load scene '{0}' update, progress '{1}'.", ne.SceneAssetName, ne.Progress.ToString("P2"));
        }

        private void OnLoadSceneDependencyAsset(object sender, GameEventArgs e)
        {
            LoadSceneDependencyAssetEventArgs ne = (LoadSceneDependencyAssetEventArgs)e;
            if (ne.UserData != this)
            {
                return;
            }

            Log.Info("Load scene '{0}' dependency asset '{1}', count '{2}/{3}'.", ne.SceneAssetName, ne.DependencyAssetName, ne.LoadedCount.ToString(), ne.TotalCount.ToString());
        }
    }
}
