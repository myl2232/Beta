﻿//this source code was auto-generated by tolua#, do not modify it
using System;
using LuaInterface;

public class AlphaWork_UIAttack2EventArgsWrap
{
	public static void Register(LuaState L)
	{
		L.BeginClass(typeof(AlphaWork.UIAttack2EventArgs), typeof(GameFramework.Event.GameEventArgs));
		L.RegFunction("Clear", Clear);
		L.RegFunction("New", _CreateAlphaWork_UIAttack2EventArgs);
		L.RegFunction("__tostring", ToLua.op_ToString);
		L.RegVar("EventId", get_EventId, null);
		L.RegVar("Id", get_Id, null);
		L.EndClass();
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int _CreateAlphaWork_UIAttack2EventArgs(IntPtr L)
	{
		try
		{
			int count = LuaDLL.lua_gettop(L);

			if (count == 0)
			{
				AlphaWork.UIAttack2EventArgs obj = new AlphaWork.UIAttack2EventArgs();
				ToLua.PushObject(L, obj);
				return 1;
			}
			else
			{
				return LuaDLL.luaL_throw(L, "invalid arguments to ctor method: AlphaWork.UIAttack2EventArgs.New");
			}
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int Clear(IntPtr L)
	{
		try
		{
			ToLua.CheckArgsCount(L, 1);
			AlphaWork.UIAttack2EventArgs obj = (AlphaWork.UIAttack2EventArgs)ToLua.CheckObject<AlphaWork.UIAttack2EventArgs>(L, 1);
			obj.Clear();
			return 0;
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get_EventId(IntPtr L)
	{
		try
		{
			LuaDLL.lua_pushinteger(L, AlphaWork.UIAttack2EventArgs.EventId);
			return 1;
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get_Id(IntPtr L)
	{
		object o = null;

		try
		{
			o = ToLua.ToObject(L, 1);
			AlphaWork.UIAttack2EventArgs obj = (AlphaWork.UIAttack2EventArgs)o;
			int ret = obj.Id;
			LuaDLL.lua_pushinteger(L, ret);
			return 1;
		}
		catch(Exception e)
		{
			return LuaDLL.toluaL_exception(L, e, o, "attempt to index Id on a nil value");
		}
	}
}

