using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEditor;
using System.Reflection;

namespace AlphaWork.Editor
{
    [CustomEditor(typeof(ScalableModel))]
    public class ScalableModelInspector : UnityEditor.Editor
    {        
        private SerializedProperty m_ScaleX;
        private SerializedProperty m_ScaleY;
        private SerializedProperty m_ScaleZ;
        private SerializedProperty m_Bone;
        private SerializedProperty m_Instance;

        void OnEnable()
        {
            m_ScaleX = serializedObject.FindProperty("scaleX");
            m_ScaleY = serializedObject.FindProperty("scaleY");
            m_ScaleZ = serializedObject.FindProperty("scaleZ");
            m_Bone = serializedObject.FindProperty("bone");
            m_Instance = serializedObject.FindProperty("m_instance");            

            serializedObject.ApplyModifiedProperties();
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            serializedObject.Update();

            ScalableModel m_Model = (ScalableModel)target;
            
            float ScaleX = EditorGUILayout.Slider("ScaleX", m_ScaleX.floatValue, 0f, 100f);
            if (ScaleX != m_ScaleX.floatValue)
            {
                if (EditorApplication.isPlaying)
                {
                    m_Model.ScaleX = ScaleX;
                }
                else
                {
                    m_ScaleX.floatValue = ScaleX;
                }
            }

            float ScaleY = EditorGUILayout.Slider("ScaleY", m_ScaleY.floatValue, 0f, 100f);
            if (ScaleY != m_ScaleY.floatValue)
            {
                if (EditorApplication.isPlaying)
                {
                    m_Model.ScaleY = ScaleY;
                }
                else
                {
                    m_ScaleY.floatValue = ScaleY;
                }
            }

            float ScaleZ = EditorGUILayout.Slider("ScaleZ", m_ScaleZ.floatValue, 0f, 100f);
            if (ScaleZ != m_ScaleZ.floatValue)
            {
                if (EditorApplication.isPlaying)
                {
                    m_Model.ScaleZ = ScaleZ;
                }
                else
                {
                    m_ScaleZ.floatValue = ScaleZ;
                }
            }

            EditorGUILayout.BeginHorizontal();
            {
                EditorGUILayout.LabelField("Bone", GUILayout.Width(160f));
                m_Model.Bone = EditorGUILayout.TextField(m_Model.Bone);
                if (GUILayout.Button("Apply", GUILayout.Width(80f)))
                {
                    ApplyScale(m_Model);
                }
            }
            EditorGUILayout.EndHorizontal();

            object[] sceneObjs = GameObject.FindObjectsOfType(typeof(GameObject));
            foreach (object o in sceneObjs)
            {
                GameObject gameObject = (GameObject)o;
                ScalableModel model = gameObject.GetComponentInChildren<ScalableModel>();
                if (model != null)
                {
                    m_Model.Instance = gameObject;
                    break;
                }
            }

            serializedObject.ApplyModifiedProperties();
            Repaint();
        }

        private void ApplyScale(ScalableModel model)
        {
            model.Scale();
        }
    }

}
