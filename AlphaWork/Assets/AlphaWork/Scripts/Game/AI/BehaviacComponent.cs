using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
//using behaviac;

namespace AlphaWork 
{
    public class BehaviacComponent: MonoBehaviour
    {
        public void Start()
        {
            InitBehavic();
        }

        public void OnDestroy()
        {
            CleanupBehaviac();
        }

        public bool InitBehavic()
        {
            //Console.WriteLine("InitBehavic");

            behaviac.Workspace.Instance.FilePath = "Assets/AlphaWork/Scripts/Game/Behaviour";
            behaviac.Workspace.Instance.FileFormat = behaviac.Workspace.EFileFormat.EFF_xml;

            return true;
        }


        public void UpdateLoop()
        {

        }

        public void CleanupBehaviac()
        {
            //Console.WriteLine("CleanupBehaviac");

            behaviac.Workspace.Instance.Cleanup();
        }
    }
}
