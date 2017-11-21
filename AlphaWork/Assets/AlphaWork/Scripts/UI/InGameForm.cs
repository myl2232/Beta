using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GameFramework;
using UnityEngine;
using UnityGameFramework.Runtime;

namespace AlphaWork
{
    class InGameForm : UGuiForm
    {
        public void OnReplaceButtonClick()
        {
            GameEntry.Event.Fire(this, new UIOccupyEventArgs());
        }

        public void OnAIButtonClick()
        {
            GameEntry.Event.Fire(this, new AIGoEventArgs());
        }
       
    }
}
