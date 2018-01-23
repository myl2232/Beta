using GameFramework.Event;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityGameFramework.Runtime;

namespace AlphaWork 
{
    public class BehaviacComponent: GameFrameworkComponent
    {
        protected List<GameObject> Targets;

        protected override void Awake()
        {
            base.Awake();
            
            InitBehavic();
            Targets = new List<GameObject>();
        }
        
        public void Start()
        {            
            GameEntry.Event.Subscribe(LoadSceneSuccessEventArgs.EventId, OnLoadSceneSuccess);
        }

        public void OnDestroy()
        {
            CleanupBehaviac();
        }


        protected bool InitBehavic()
        {
            behaviac.Workspace.Instance.FilePath = "Assets/AlphaWork/Scripts/Game/Behaviour";
            behaviac.Workspace.Instance.FileFormat = behaviac.Workspace.EFileFormat.EFF_xml;

            return true;
        }

        public void CleanupBehaviac()
        {
            if(behaviac.Workspace.Instance != null)
                behaviac.Workspace.Instance.Cleanup();
        }

        private void OnLoadSceneSuccess(object sender, GameEventArgs e)
        {
            FullfillTargets();
        }

        protected void FullfillTargets()
        {
            GameObject[] hings = FindObjectsOfType(typeof(GameObject)) as GameObject[];
            for (int i = 0; i < hings.Length; ++i)
            {
                TargetPoint pt = hings[i].GetComponent<TargetPoint>();
                if (pt)
                {
                    Targets.Add(hings[i]);
                }
            }
        }

        public void GetNextTarget(Vector3 vPos, ref GameObject target)
        {
            if(Targets.Count > 0)
            {
                int index = UnityEngine.Random.Range(0, Targets.Count() - 1);
                target = Targets[index];
            }

        }

    }
}
