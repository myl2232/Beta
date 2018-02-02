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

#if (UNITY_IPHONE || UNITY_ANDROID)
            if(IsPointerOverGameObject())
#else
            if (EventSystem.current.IsPointerOverGameObject())
#endif
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
        public bool IsPointerOverGameObject() { 
       //if (Input.touchCount > 0) { 
                        
       //    int id = Input.GetTouch(0).fingerId; 
       //    return UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject(id);//安卓机上不行 
       //} 
       //else { 
           //return UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject(); 
           PointerEventData eventData = new PointerEventData(UnityEngine.EventSystems.EventSystem.current); 
           eventData.pressPosition = Input.mousePosition; 
           eventData.position = Input.mousePosition; 
 
           List<RaycastResult> list = new List<RaycastResult>(); 
           UnityEngine.EventSystems.EventSystem.current.RaycastAll(eventData, list); 
           //Debug.Log(list.Count); 
           return list.Count > 0; 
      // } 
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
