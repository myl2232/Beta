using UnityEngine;
using UnityGameFramework;

namespace AlphaWork
{
    /// <summary>
    /// 游戏入口。
    /// </summary>
    public partial class GameEntry : MonoBehaviour
    {
        /*public SurviveGame gm;*/
        private float markTime;
        private float lastMarkTime;
        public static bool ArMode;
        public GameBuilder builder;
        
        private void Start()
        {
//             GameObject frameworkPrefab = (GameObject)Resources.Load("PrefabDefined/GameFramework");
//             Instantiate(frameworkPrefab);

            InitBuiltinComponents();
            InitCustomComponents();

            //builder = ObjectUtility.GetFellow("GameBuilder") as GameBuilder;
            /*Agent = GetAssistObject();*/
            //OnClick = OnClickImpl;
            //if(gm == null)
            //    gm = new SurvivalGame();
        }
        private void Awake()
        {
            markTime = Time.time;
            lastMarkTime = markTime;

            //gm.Initialize();
        }

        private void Update()
        {
            markTime = Time.time;
            //gm.Update(Time.timeSinceLevelLoad, Time.fixedDeltaTime/*markTime - lastMarkTime*/);
            lastMarkTime = markTime;
            ArMode = builder.ARMode;
        }

    }
}
