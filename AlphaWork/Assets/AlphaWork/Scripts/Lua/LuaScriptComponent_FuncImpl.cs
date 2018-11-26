using GameFramework;
using LuaInterface;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityGameFramework.Runtime;

namespace AlphaWork
{
    public partial class LuaScriptComponent : GameFrameworkComponent
    {
        public void RegistGameObject2Lua(GameObject gb, string tableName)
        {
            LuaTable tb = m_LuaState.Require<LuaTable>(tableName);
            //Log.Info("--------RegistGameObject2Lua---------------:");
            tb.GetLuaFunction("RegistObj").Invoke<GameObject,object>(gb);
            //tb.GetLuaFunction("OnLoadPanel").Invoke<object>();
        }
    }
}
