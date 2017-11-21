using UnityEditor;
using UnityEngine;

namespace AlphaWork.Editor
{
    [CustomEditor(typeof(DeviceModelConfig))]
    public class AlphaDeviceModelConfigInspector : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            if (GUILayout.Button("Open Device Model Config Editor"))
            {
                AlphaDeviceModelConfigEditorWindow.OpenWindow((/*Texture2D*/DeviceModelConfig)target);
            }
        }
    }
}
