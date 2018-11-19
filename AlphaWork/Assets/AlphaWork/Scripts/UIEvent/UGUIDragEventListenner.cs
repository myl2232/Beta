using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace AlphaWork
{
    public class UGUIDragEventListenner : UGUIEventListenner,IBeginDragHandler,IDragHandler,
IEndDragHandler,IDropHandler,IScrollHandler,IUpdateSelectedHandler,IInitializePotentialDragHandler {

	// Use this for initialization
	void Start () {

	}

	public virtual void OnDrag(PointerEventData eventData)
	{
		if (CheckNeedHideEvent())
		{
			return;
		}

		if (this.onDrag != null)
		{
			this.onDrag(gameObject, eventData.delta,eventData.position);
		}
	}

	public void OnDrop(PointerEventData eventData)
	{
		if (CheckNeedHideEvent())
		{
			return;
		}

		if (this.onDrop != null)
		{
			this.onDrop(gameObject);
		}
	}

	public virtual void OnBeginDrag(PointerEventData eventData)
	{
		if (CheckNeedHideEvent())
		{
			return;
		}

		if (this.onBeginDrag != null)
		{
			this.onBeginDrag(gameObject, eventData.delta, eventData.position);
		}
	}

	public virtual void OnEndDrag(PointerEventData eventData)
	{
		if (CheckNeedHideEvent())
		{
			return;
		}
		if (this.onEndDrag != null)
		{
			this.onEndDrag(gameObject, eventData.delta, eventData.position);
		}
	}

	public virtual void OnScroll(PointerEventData eventData)
	{
		if (CheckNeedHideEvent())
		{
			return;
		}

		if (this.onScroll != null)
		{
			this.onScroll(gameObject);
		}
	}

	public void OnUpdateSelected(BaseEventData eventData)
	{
		if (CheckNeedHideEvent())
		{
			return;
		}

		if (this.onUpdateSelected != null)
		{
			this.onUpdateSelected(gameObject);
		}
	}

	public void OnInitializePotentialDrag(PointerEventData eventData)
	{
		if (CheckNeedHideEvent())
		{
			return;
		}

		if (this.onInitializePotentialDrag != null)
		{
			this.onInitializePotentialDrag(gameObject);
		}
	}

    public static UGUIDragEventListenner Get(GameObject go)
    {
        UGUIDragEventListenner listener = go.GetComponent<UGUIDragEventListenner>();
        if (listener == null)
            listener = go.AddComponent<UGUIDragEventListenner>();
        return listener;
    }

    public static UGUIDragEventListenner Get(Transform tran)
    {
        return Get(tran.gameObject);
    }

}
}