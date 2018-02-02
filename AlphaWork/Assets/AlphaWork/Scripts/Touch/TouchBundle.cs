using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.EventSystems;

namespace AlphaWork
{
    [RequireComponent(typeof(FingerHoverDetector))]
    [RequireComponent(typeof(FingerDownDetector))]
    [RequireComponent(typeof(ScreenRaycaster))]
    public class TouchBundle : MonoBehaviour
    {
        public GameObject fingerHoverObject;

        private void Update()
        {
            List<GestureRecognizer> recognizers = FingerGestures.RegisteredGestureRecognizers;
            if (EventSystem.current.IsPointerOverGameObject())
            {
                for (int i = 0; i < recognizers.Count; ++i)
                {
                    recognizers[i].UseSendMessage = false;
                }
            }
            else
            {
                for (int i = 0; i < recognizers.Count; ++i)
                {
                    recognizers[i].UseSendMessage = true;
                }
            }
        }
        void OnFingerHover(FingerHoverEvent e)
        {
            if (e.Selection == fingerHoverObject)
            {
            }
        }

        void OnFingerDown()
        {

        }
    }
}
