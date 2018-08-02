using UnityEngine;
using UnityGameFramework;

namespace AlphaWork
{
    /// <summary>
    /// 游戏入口。
    /// </summary>
    public partial class GameEntry : MonoBehaviour
    {
        private float markTime;
        private float lastMarkTime;
        
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
        }

    }
}
