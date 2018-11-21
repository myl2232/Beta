using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;

namespace AlphaWork
{
    public class UGUIEventListenner : MonoBehaviour,
IMoveHandler,
IPointerDownHandler, IPointerUpHandler,
IPointerEnterHandler, IPointerExitHandler,
ISelectHandler, IDeselectHandler, IPointerClickHandler,
ISubmitHandler, ICancelHandler {

	protected UGUIMsgHandler TouchedMsgHandler;   //用于记录当前UGUIListenner对应的对象所touch的MsgHandler对象  每一个大界面下的小的控件只能touch一个MsgHandler对象
	//用来记录当前的selectable组件
	protected Selectable mCurSelectableComponent;

	public UGUIMsgHandler MsgHandler {
		get {
			if (TouchedMsgHandler != null)
				return TouchedMsgHandler;
			else
				return null;
		}
		set {
			TouchedMsgHandler = value;
		}
	}

	public Selectable CurSelectable {
		get {
			if (mCurSelectableComponent != null)
				return mCurSelectableComponent;
			else
				return null;
		}
		set {
			mCurSelectableComponent = value;
		}
	}

	public delegate void UIEventHandler(GameObject obj);
	public delegate void UIDragEventHandlerDetail(GameObject obj, Vector2 deltaPos, Vector2 curToucPosition);
	public UIEventHandler onClick;
	public UIEventHandler onDown;
	public UIEventHandler onUp;
	public UIDragEventHandlerDetail onDownDetail;
	public UIDragEventHandlerDetail onUpDetail;
	public UIDragEventHandlerDetail onDrag;
	public UIEventHandler onExit;
	public UIEventHandler onDrop;
	public UIEventHandler onSelect;
	public UIEventHandler onDeSelect;
	public UIEventHandler onMove;
	public UIDragEventHandlerDetail onBeginDrag;
	public UIDragEventHandlerDetail onEndDrag;
	public UIEventHandler onEnter;
	public UIEventHandler onSubmit;
	public UIEventHandler onScroll;
	public UIEventHandler onCancel;
	public UIEventHandler onUpdateSelected;
	public UIEventHandler onInitializePotentialDrag;
	//统一检查是否需要屏蔽点击事件
	protected bool CheckNeedHideEvent()
	{
		if (this.mCurSelectableComponent == null || !this.mCurSelectableComponent.interactable
		    || !this.mCurSelectableComponent.enabled) {
			return true;
		}
		return false;
	}
	public virtual void OnPointerClick(PointerEventData eventData)
	{
		if (CheckNeedHideEvent ()) {
			return;
		}

		if (this.onClick != null) {
			this.onClick (gameObject);
		}
	}

	public virtual void OnPointerDown(PointerEventData eventData)
	{
		if (CheckNeedHideEvent ()) {
			return;
		}

		if (this.onDown != null) { 
			this.onDown (gameObject);
		}
		if (this.onDownDetail != null) {
			this.onDownDetail (gameObject, eventData.delta, eventData.position);
		}
	}

	public virtual void OnPointerUp(PointerEventData eventData)
	{
		if (CheckNeedHideEvent ()) {
			return;
		}

		if (this.onUp != null) {
			this.onUp (gameObject);
		}
		if (this.onUpDetail != null) {
			this.onUpDetail (gameObject, eventData.delta, eventData.position);
		}
	}

	public virtual void OnPointerExit(PointerEventData eventData)
	{
		if (CheckNeedHideEvent())
		{
			return;
		}

		if (this.onExit != null) {
			this.onExit(gameObject);
		}
	}

	public virtual void OnSelect(BaseEventData eventData)
	{
		if (CheckNeedHideEvent ()) {
			return;
		}

		if (this.onSelect != null) {
			this.onSelect (gameObject);
		}
	}

	public virtual void OnMove(AxisEventData eventData)
	{
		if (CheckNeedHideEvent ()) {
			return;
		}

		if (this.onMove != null) {
			this.onMove (gameObject);
		}
	}

	public virtual void OnPointerEnter(PointerEventData eventData)
	{
		if (CheckNeedHideEvent ()) {
			return;
		}
		if (this.onEnter != null) {
			this.onEnter (gameObject);
		}
	}

	public void OnSubmit(BaseEventData eventData)
	{
		if (CheckNeedHideEvent ()) {
			return;
		}

		if (this.onSubmit != null) {
			this.onSubmit (gameObject);
		}
	}

	public void OnCancel(BaseEventData eventData)
	{
		if (CheckNeedHideEvent ()) {
			return;
		}

		if (this.onCancel != null) {
			this.onCancel (gameObject);
		}
	}

	public void OnDeselect(BaseEventData eventData)
	{
		if (CheckNeedHideEvent ()) {
			return;
		}
		if (this.onDeSelect != null) {
			this.onDeSelect (gameObject);
		}
	}


	/*public static UGUIEventListenner GetEventListenner(GameObject obj) {
        UGUIEventListenner listenner = obj.GetComponent<UGUIEventListenner>();
        if (listenner == null) {
            listenner = obj.AddComponent<UGUIEventListenner>();
        }
        return listenner;
    }*/
}
}