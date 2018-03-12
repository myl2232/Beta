using GameFramework;
using UnityEngine;
using UnityGameFramework.Runtime;

namespace AlphaWork
{
    public class ConfigComponent : GameFrameworkComponent
    {
        [SerializeField]
        private DeviceModelConfig m_DeviceModelConfig = null;

        [SerializeField]
        private TextAsset m_BuildInfoTextAsset = null;

        [SerializeField]
        private TextAsset m_DefaultDictionaryTextAsset = null;

        [SerializeField]
        private UpdateResourceForm m_UpdateResourceFormTemplate = null;

        [SerializeField]
        private int m_MainSceneId = 4;
        public int MainScene
        {
            get { return m_MainSceneId; }
            set { m_MainSceneId = value; }
        }

        private BuildInfo m_BuildInfo = null;

        public DeviceModelConfig DeviceModelConfig
        {
            get
            {
                return m_DeviceModelConfig;
            }
        }

        public BuildInfo BuildInfo
        {
            get
            {
                return m_BuildInfo;
            }
        }

        public UpdateResourceForm UpdateResourceFormTemplate
        {
            get
            {
                return m_UpdateResourceFormTemplate;
            }
        }

        public void InitBuildInfo()
        {
            if (m_BuildInfoTextAsset == null || string.IsNullOrEmpty(m_BuildInfoTextAsset.text))
            {
                Log.Info("Build info can not be found or empty.");
                return;
            }

            m_BuildInfo = Utility.Json.ToObject<BuildInfo>(m_BuildInfoTextAsset.text);
            //m_BuildInfo.CheckVersionUrl = "http://localhost:81/WWW/version.txt";
            //m_BuildInfo.GameVersion = "0.1.0";
            //m_BuildInfo.InternalVersion = 0;
            if (m_BuildInfo == null)
            {
                Log.Warning("Parse build info failure.");
                return;
            }

            GameEntry.Base.GameVersion = GameEntry.Config.BuildInfo.GameVersion;
            GameEntry.Base.InternalApplicationVersion = GameEntry.Config.BuildInfo.InternalVersion;
        }

        public void InitDefaultDictionary()
        {
            if (m_DefaultDictionaryTextAsset == null || string.IsNullOrEmpty(m_DefaultDictionaryTextAsset.text))
            {
                Log.Info("Default dictionary can not be found or empty.");
                return;
            }

            if (!GameEntry.Localization.ParseDictionary(m_DefaultDictionaryTextAsset.text))
            {
                Log.Warning("Parse default dictionary failure.");
                return;
            }
        }
    }
}
