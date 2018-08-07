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
        public void OnAlphaClick()
        {
            GameEntry.Event.Fire(this, new UIAlphaEventArgs());
        }

        public void OnBetaClick()
        {
            GameEntry.Event.Fire(this, new UIBetaEventArgs());
        }
       
        public void OnThetaClick()
        {
            GameEntry.Event.Fire(this, new UIThetaEventArgs());
        }

        public void OnAttack1Click()
        {
            GameEntry.Event.Fire(this, new UIAttack1EventArgs());
        }

        public void OnAttack2Click()
        {
            GameEntry.Event.Fire(this, new UIAttack2EventArgs());
        }
        public void OnBackClick()
        {
            GameEntry.Event.Fire(this, new GameToLoginEventArgs());
        }
    }
}
