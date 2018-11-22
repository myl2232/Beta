using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System;

using LuaInterface;

namespace AlphaWork
{ 
    public class UGUIMsgHandler : MonoBehaviour {

    private static LuaFunction tryget = null;
	private LuaState m_luaState = null;
	private LuaTable m_luaclass = null;
	private LuaTable m_luatable = null;

	private Dictionary<GameObject, Dictionary<string, string>> m_luaOprFuncDic = new Dictionary<GameObject, Dictionary<string, string>>();

	public void SetMsgTable(LuaTable table, LuaTable luaclass)
    {
        if (tryget == null)
        {
            LuaTable lPlus = GameEntry.LuaScriptEngine.LuaState.Require<LuaTable>("Lplus");
            tryget = lPlus.GetLuaFunction("tryget");
        }
        m_luaclass = luaclass;
		m_luatable = table;
	}

	public void Touch(GameObject obj) {

		ScrollRect[] rects = obj.GetComponentsInChildren<ScrollRect>(true);
		for (int i = 0; i < rects.Length; i++) {
			AddUGUIOtherEventListenner (rects [i]);
		}
		Selectable[] selectable = obj.GetComponentsInChildren<Selectable>(true);

		foreach (Selectable st in selectable) {
			AddEventHandler (st);
		}
		//UITweener[] playTweens = obj.GetComponentsInChildren<UITweener>();
		//foreach (UITweener tween in playTweens) {
		//	AddOtherEventHandler (tween.gameObject);
		//}
		//ToggleGroup[] tggroups = gameObject.GetComponentsInChildren<ToggleGroup>(true);
		//foreach (ToggleGroup tgp in tggroups) {
		//    AddEventHandler(tgp.gameObject);
		//}
	}

	public void AddLuaEventHandler(GameObject obj, string eventName, string func)
	{
		if (obj == null) {
			return; 
		}
		if (!m_luaOprFuncDic.ContainsKey (obj)) {
			m_luaOprFuncDic [obj] = new Dictionary<string, string> ();
		}
		m_luaOprFuncDic [obj] [eventName] = func;
		Touch (obj);
	}

	private void AddEventHandler(Selectable st) {
		UGUIEventListenner listenner = st.gameObject.GetComponent<UGUIEventListenner> ();

		if (listenner == null) { //防止多次Touch
			if ((st is Scrollbar) || (st is InputField) || (st is Slider)) {
				listenner = st.gameObject.AddComponent<UGUIDragEventListenner> ();
			} else {
				//此处正常button是可以响应拖拽事件但有ScrollRect作为父组件的情况下会存在冲突
				bool useDrag = false;
				if (st is Button) {
					ScrollRect[] rect = st.gameObject.GetComponentsInParent<ScrollRect> (true);
					useDrag = (rect == null || rect.Length == 0);
				}
				/*else if (st is ToggleExt)
				{
					useDrag = ((ToggleExt)st).useDrag;
				}*/

				if (useDrag) {
					listenner = st.gameObject.AddComponent<UGUIDragEventListenner> ();
				} else {
					listenner = st.gameObject.AddComponent<UGUIEventListenner> ();
					//UGUIEventListenner.GetEventListenner(obj);
				}

			}

			listenner.MsgHandler = this;
		} else {
			if (this == listenner.MsgHandler) { //如果当前的和原来的一样 就不用再touch一次
				listenner.CurSelectable = st;
				return;
			} else {             //如果想touch一个新的对象 先清除掉原来的
				UGUIMsgHandler prevHandler = listenner.MsgHandler;
				if (prevHandler)
					prevHandler.RemoveEventHandler (listenner.gameObject);   
				listenner.MsgHandler = this;
			}
		}
		//在listenner上面记录Selectable组件
		listenner.CurSelectable = st;
		AddEventHandlerEx (listenner);
	}

	void AddEventHandlerEx(UGUIEventListenner listenner) {
		listenner.onClick += this.onClickhandle;
		listenner.onDown += this.onDownhandle;
		listenner.onUp += this.onUphandle;
		listenner.onDownDetail += this.onDownDetailhandle;
		listenner.onUpDetail += this.onUpDetailhandle;
		listenner.onEnter += this.onEnterhandle;
		listenner.onExit += this.onExithandle;
		listenner.onDrop += this.onDrophandle;
		listenner.onBeginDrag += this.onBeginDraghandle;
		listenner.onDrag += this.onDraghandle;
		listenner.onEndDrag += this.onEndDraghandle;
		listenner.onSelect += this.onSelecthandle;
		listenner.onDeSelect += this.onDeSelecthandle;
		listenner.onScroll += this.onScrollhandle;
		listenner.onCancel += this.onCancelhandle;
		listenner.onSubmit += this.onSubmithandle;
		listenner.onMove += this.onMovehandle;
		listenner.onUpdateSelected += this.onUpdateSelectedhandle;
		listenner.onInitializePotentialDrag += this.onInitializePotentialDragHandle;
		AddOtherEventHandler (listenner.gameObject);
	}

	void AddOtherEventHandler(GameObject go)
	{
		UGUIOtherEventListenner otherlistenner = go.GetComponent<UGUIOtherEventListenner> ();
		if (otherlistenner == null)
			otherlistenner = go.AddComponent<UGUIOtherEventListenner> ();
		otherlistenner.inputvalueChangeAction += onStrValueChangeHandle;
		otherlistenner.inputeditEndAction += onEditEndHandle;
		otherlistenner.togglevalueChangeAction += onBoolValueChangeHandle;
		otherlistenner.slidervalueChangeAction += onFloatValueChangeHandle;
		otherlistenner.scrollbarvalueChangeAction += onFloatValueChangeHandle;
		otherlistenner.scrollrectvalueChangeAction += onRectValueChangeHandle;
		otherlistenner.dropdownvalueChangeAction += onIntValueChangeHandle;
		otherlistenner.OnPlayTweenHandle += OnPlayTweenFinishHandle;
	}
	void AddUGUIOtherEventListenner(ScrollRect rect)
	{

		UGUIOtherEventListenner otherlistenner = rect.gameObject.GetComponent<UGUIOtherEventListenner> ();
		if (otherlistenner == null)
			otherlistenner = rect.gameObject.AddComponent<UGUIOtherEventListenner> ();
		rect.onValueChanged.AddListener (otherlistenner.scrollrectValueChangeHandler ());
		otherlistenner.scrollrectvalueChangeAction += onRectValueChangeHandle;
        //if(rect is ScrollRectEx)
        //{
        //    var rect1 = rect as ScrollRectEx;
        //    rect1.beginDrag += onBeginDraghandle;
        //    rect1.endDrag += onEndDraghandle;
        //}
	}

	public void UnTouch(GameObject obj) {
		Selectable[] selectable = obj.GetComponentsInChildren<Selectable> (true);

		foreach (Selectable st in selectable) {
			RemoveEventHandler (st.gameObject);
		}
	}

	void RemoveEventHandler(GameObject obj) {
		UGUIEventListenner listenner = obj.GetComponent<UGUIEventListenner> ();
		if (listenner == null || listenner.MsgHandler == null || listenner.MsgHandler != this)        //必须在touch过同一个 MsgHandler的情况下才能用这个MsgHandler进行untouch
			return;

        listenner.onClick -= this.onClickhandle;
        listenner.onDown -= this.onDownhandle;
		listenner.onUp -= this.onUphandle;
		listenner.onEnter -= this.onEnterhandle;
		listenner.onExit -= this.onExithandle;
		listenner.onDrop -= this.onDrophandle;
		listenner.onBeginDrag -= this.onBeginDraghandle;
		listenner.onDrag -= this.onDraghandle;
		listenner.onEndDrag -= this.onEndDraghandle;
		listenner.onSelect -= this.onSelecthandle;
		listenner.onDeSelect -= this.onDeSelecthandle;
		listenner.onScroll -= this.onScrollhandle;
		listenner.onCancel -= this.onCancelhandle;
		listenner.onSubmit -= this.onSubmithandle;
		listenner.onMove -= this.onMovehandle;
		listenner.onUpdateSelected -= this.onUpdateSelectedhandle;
		listenner.onInitializePotentialDrag -= this.onInitializePotentialDragHandle;

		UGUIOtherEventListenner otherlistenner = listenner.gameObject.GetComponent<UGUIOtherEventListenner>();
		if (otherlistenner != null) {
			otherlistenner.inputvalueChangeAction -= onStrValueChangeHandle;
			otherlistenner.inputeditEndAction -= onEditEndHandle;
			otherlistenner.togglevalueChangeAction -= onBoolValueChangeHandle;
			otherlistenner.slidervalueChangeAction -= onFloatValueChangeHandle;
			otherlistenner.scrollbarvalueChangeAction -= onFloatValueChangeHandle;
			otherlistenner.scrollrectvalueChangeAction -= onRectValueChangeHandle;
			otherlistenner.dropdownvalueChangeAction -= onIntValueChangeHandle;
			otherlistenner.OnPlayTweenHandle -= OnPlayTweenFinishHandle;
		}
		if (m_luaOprFuncDic.ContainsKey (obj)) {
			m_luaOprFuncDic.Remove (obj);
		}
		listenner.MsgHandler = null;  //清除掉MsgHandler
	}
	#region LuaCall


	void CallWithParams<T>(LuaTable luaClass,string name, T objInfo, params object[] args)
	{
		m_luaState = luaClass.GetLuaState ();
		int top = m_luaState.LuaGetTop ();

		try {
			if (BeginCall (luaClass,name, top)) {
				m_luaState.PushGeneric (m_luaclass);
				m_luaState.PushGeneric (objInfo);
				int argcount = args.Length;
				for (int i = 0; i < argcount; i++) {
					m_luaState.PushGeneric (args [i]);
				}
				m_luaState.Call (argcount + 2, top + 2, top);
			}
			m_luaState.LuaSetTop (top);
		} catch (Exception e) {
			m_luaState.LuaSetTop (top);
			throw e;
		}
	}

	bool BeginCall(LuaTable luaClass,string name, int top)
	{
		m_luaState.Push (luaClass);
		m_luaState.ToLuaPushTraceback ();
		m_luaState.Push (name);
		m_luaState.LuaGetTable (top + 1);
		return m_luaState.LuaType (top + 3) == LuaTypes.LUA_TFUNCTION;
	}

	bool FunctionExist(LuaTable table, string functionName)
	{
		try {
 
            var find = tryget.Invoke< LuaTable,string, object>(table, functionName);
            return find != null;
		} catch(Exception e) {
			return false;
		}
	}

	private void OprLuaCall(GameObject obj, string eventName, bool checkObjFunc, params object[] args)
	{
		if (m_luaclass == null)
			return;
		if (m_luaOprFuncDic.ContainsKey (obj) && m_luaOprFuncDic [obj].ContainsKey (eventName)) {
			string luafuncname = m_luaOprFuncDic [obj] [eventName]; 
			CallWithParams<string> (m_luatable,luafuncname, eventName, args);

        }
        else {
           
            bool hasfunction = FunctionExist (m_luaclass, eventName);
			bool hasObjfunction = FunctionExist (m_luaclass, eventName + "Obj");
			if (checkObjFunc && hasObjfunction) {
                CallWithParams<GameObject> (m_luatable, eventName + "Obj", obj, args);
            } else if (hasfunction) {
                CallWithParams<string> (m_luatable, eventName, obj.name, args);
            }
        }
	}
    #endregion

    #region EventHandle

    /// <summary>
    /// 用于长按事件
    /// </summary>
    private float HoldDeltaTime = 0.2f;
    public float HoldTime
    {
        get { return HoldDeltaTime; }
        set { HoldDeltaTime = value; }
    }
    // 触摸down时的时间
    private float m_timePointDown = 0;
    // 是否是PointDown状态
    private bool m_isPointDown = false;
    // PointDown的物体
    private GameObject m_pointDownObj = null;

    // 是否是Hold事件(Hold事件不响应click)
    private bool m_isHoldEvent = false;
    // 是否触发了Hold事件, 避免长按时触发多次Hold事件
    private bool m_isTriggerHoldEvent = false;

    void onClickhandle(GameObject obj)
	{
        /*//Debug.LogWarning("onClick ..............." +obj.name);
		OprLuaCall(obj, "onClick",0,null ,true);
		//新手引导用接口
		//OprLuaCall(obj, "onGuideClick", 0, null, true);
		/*CommonTips comTips = obj.GetComponent<CommonTips>();
		if (comTips && comTips.GetTipsID() > 0 )
		{
			LuaDLL.lua_getglobal(wLua.L.L, "ShowCommonTips");
			LuaDLL.lua_pushnumber(wLua.L.L, comTips.GetTipsID());
			wLua.L.Call(1);
		}*/

        if (!m_isHoldEvent)
        {
            //Debug.Log(String.Format("OnClick name:{0} frame:{1}", obj.name, Time.time));

            OprLuaCall(obj, "onClick", true);
        }
	}

	void onDownhandle(GameObject obj)
	{
        //Debug.Log(String.Format("onDownhandle name:{0} frame:{1}", obj.name, Time.frameCount));

        m_timePointDown = Time.time;

        m_pointDownObj = obj;
        m_isPointDown = true;
        m_isHoldEvent = false;
        m_isTriggerHoldEvent = false;

        OprLuaCall(obj, "onDown", true);
	}

	void onDownDetailhandle(GameObject obj, Vector2 delta, Vector2 touchPostion)
	{
		OprLuaCall (obj, "onDownDetail", true, delta.x, delta.y, touchPostion.x, touchPostion.y);
	}

	void onUphandle(GameObject obj)
	{
        m_isPointDown = false;

        //if (!m_isHoldEvent)
        //{
        //Debug.Log(String.Format("OnUp name:{0} frame:{1}", obj.name, Time.time));

        OprLuaCall(obj, "onUp", true);
        //}
    }

	void onUpDetailhandle(GameObject obj, Vector2 delta, Vector2 touchPostion)
	{
		OprLuaCall (obj, "onUpDetail", true, delta.x, delta.y, touchPostion.x, touchPostion.y);
	}

	void onEnterhandle(GameObject obj)
	{
		OprLuaCall(obj, "onEnter", true);
	}

	void onExithandle(GameObject obj)
	{
		OprLuaCall(obj, "onExit", true);
	}

	void onDrophandle(GameObject obj)
	{
		OprLuaCall(obj, "onDrop", true);
	}

	void onSelecthandle(GameObject obj)
	{
		OprLuaCall(obj, "onSelect", true);
	}

	void onDeSelecthandle(GameObject obj)
	{
		OprLuaCall(obj, "onDeSelect", true);
	}

	void onBeginDraghandle(GameObject obj, Vector2 delta, Vector2 touchPostion)
	{
		OprLuaCall(obj, "onBeginDrag", true, delta.x, delta.y, touchPostion.x, touchPostion.y);
	}

	void onDraghandle(GameObject obj, Vector2 delta, Vector2 touchPostion)
	{
		OprLuaCall(obj, "onDrag", true, delta.x, delta.y, touchPostion.x, touchPostion.y);
	}

	void onEndDraghandle(GameObject obj, Vector2 delta, Vector2 touchPostion)
	{
		OprLuaCall(obj, "onEndDrag", true, delta.x, delta.y, touchPostion.x, touchPostion.y);
	}

	void onMovehandle(GameObject obj)
	{
		OprLuaCall(obj, "onMove", true);
	}

	void onSubmithandle(GameObject obj)
	{
		OprLuaCall(obj, "onSubmit", true);
	}

	void onScrollhandle(GameObject obj)
	{
		OprLuaCall(obj, "onScroll", true);
	}

	void onCancelhandle(GameObject obj)
	{
		OprLuaCall(obj, "onCancel", true);
	}

	void onUpdateSelectedhandle(GameObject obj)
	{
		OprLuaCall(obj, "onUpdateSelected", true);
	}

	void onInitializePotentialDragHandle(GameObject obj)
	{
		OprLuaCall(obj, "onInitializePotentialDrag", true);
	}


	//处理各个控件上onValueChanged事件响应
	void onStrValueChangeHandle(GameObject obj, string para)
	{
		OprLuaCall(obj, "onStrValueChanged", true, para);
	}
	void onFloatValueChangeHandle(GameObject obj, float para)
	{
		OprLuaCall(obj, "onFloatValueChanged", true, para);
	}
	void onIntValueChangeHandle(GameObject obj, int para)
	{
		OprLuaCall(obj, "onIntValueChanged", true, para);
	}
	void onBoolValueChangeHandle(GameObject obj, bool para)
	{
		OprLuaCall(obj, "onBoolValueChanged", true, para);
	}
	void onRectValueChangeHandle(GameObject obj, Vector2 para)
	{
		OprLuaCall(obj, "onRectValueChanged", true, para.x, para.y);
	}
	void onEditEndHandle(GameObject obj, string para)
	{
		OprLuaCall(obj, "onEditEnd", true, para);
	}
    void OnPlayTweenFinishHandle(MonoBehaviour playTween)
    {
        OprLuaCall(playTween.gameObject, "onPlayTweenFinish", true);
    }
    #endregion

    void Update() {
        //	if (CallLuaBegin(gameObject, "tick") == false)
        //		return;
        //	CallLuaEnd(0);

        if (m_isPointDown && !m_isTriggerHoldEvent && m_pointDownObj != null)
        {
            // 长按事件
            if (Time.time - m_timePointDown > HoldDeltaTime)
            {
                //Debug.Log(String.Format("onHold name:{0} frame:{1}", m_pointDownObj.name, Time.frameCount));

                OprLuaCall(m_pointDownObj, "onHold", true);

                m_isPointDown = false;
                m_isTriggerHoldEvent = true;
                m_isHoldEvent = true;
            }
        }
	}

	/*public void AddCustomClickEventHandler(GameObject Obj, LUACALLBACKFUNC func )
	{
		UGUIEventListenner listenner = Obj.GetComponent<UGUIEventListenner>();
		if(listenner == null)
		{
			listenner = Obj.AddComponent<UGUIEventListenner>();
		}

		Selectable selectable = Obj.GetComponent<Selectable>();
		listenner.CurSelectable = selectable;
		listenner.onClick = (obj) => {
			if (func != null && wLua.L != null)
			{
				LuaDLL.lua_rawgeti(wLua.L.L, LuaIndexes.LUA_REGISTRYINDEX, func.funcRef);
				wLua.L.Call(0);
			}
		};
	}*/
}
}