// Unity built-in shader source. Copyright (c) 2016 Unity Technologies. MIT license (see license.txt)

using System;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEditor;


public class DistortionShaderGUI : ShaderGUI
{
    enum Blend
    {
        AlphaAdd,
        Alpha
    }

    bool bFirstTime = true;
    GUIContent guiContent = new GUIContent("Blend mode", "Blend mode");
    public override void OnGUI(MaterialEditor materialEditor, MaterialProperty[] props)
    {
        base.OnGUI(materialEditor, props);

        Material targetMat = materialEditor.target as Material;
        Blend mode = (Blend)targetMat.GetFloat("_Mode");
        if(bFirstTime)
        {
            MaterialChanged(targetMat, mode);
            bFirstTime = false;
        }
        EditorGUI.BeginChangeCheck();
        mode = (Blend)EditorGUILayout.EnumPopup(guiContent, mode);
        if (EditorGUI.EndChangeCheck())
        {
            //SetKeyword("_BLENDMODE_ALPHA", mode == BlendMode.Alpha);
            //SetKeyword("_BLENDMODE_ALPHAADD", mode == BlendMode.AlphaAdd);
            MaterialChanged(targetMat, mode);
        }
    }

    static void MaterialChanged(Material targetMat, Blend mode)
    {
        if (mode == Blend.Alpha)
        {
            targetMat.SetFloat("_SrcBlend", (float)BlendMode.SrcAlpha);
            targetMat.SetFloat("_DstBlend", (float)BlendMode.OneMinusSrcAlpha);
            targetMat.SetFloat("_Mode", (float)Blend.Alpha);
        }
        else if (mode == Blend.AlphaAdd)
        {
            targetMat.SetFloat("_SrcBlend", (float)BlendMode.SrcAlpha);
            targetMat.SetFloat("_DstBlend", (float)BlendMode.One);
            targetMat.SetFloat("_Mode", (float)Blend.AlphaAdd);
        }
    }
}
