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
        public static bool UseNavGrid;

        public GameBuilder builder;//assign by editor

        private void Start()
        {
            InitBuiltinComponents();
            InitCustomComponents();  
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
            UseNavGrid = builder.UseNavGrid;
        }

    }
}
