using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;
using System;

using LuaInterface;
namespace AlphaWork
{
    public class UGUIOtherEventListenner : MonoBehaviour
{
	public delegate void StrValueChangeAction(GameObject obj,string para);
	public delegate void FloatValueChangeAction(GameObject obj, float para);
	public delegate void IntValueChangeAction(GameObject obj, int para);
	public delegate void BoolValueChangeAction(GameObject obj, bool para);
	public delegate void RectValueChangeAction(GameObject obj, Vector2 para);
	//InputField
	public StrValueChangeAction inputvalueChangeAction;
	public StrValueChangeAction inputeditEndAction;

	//Toggle
	public BoolValueChangeAction togglevalueChangeAction;
	//ScrollBar
	public FloatValueChangeAction scrollbarvalueChangeAction;
	//slider
	public FloatValueChangeAction slidervalueChangeAction;
	//dropdown
	public IntValueChangeAction dropdownvalueChangeAction;
	//scrollrect
	public RectValueChangeAction scrollrectvalueChangeAction;

	public delegate void PlayerTweenDeliverHandler(MonoBehaviour playTween);
	public event PlayerTweenDeliverHandler OnPlayTweenHandle;

	public void Awake()
	{
		//inputEditEndAction += delegate { };
		InputField input = gameObject.GetComponent<InputField>();
		if (input != null)
		{
			input.onValueChanged.AddListener(inputValueChangeHandler);
			input.onEndEdit.AddListener(inputEditEndHanler);
		}

		Toggle toggle = gameObject.GetComponent<Toggle>();
		if (toggle != null)
		{
			toggle.onValueChanged.AddListener(toggleValueChangeHandler);
		}
		else
		{
			/*ToggleExt toggleext = gameObject.GetComponent<ToggleExt>();
			if(toggleext !=null)
			{
				toggleext.onValueChanged.AddListener(toggleValueChangeHandler);
			}*/
		}

		Scrollbar scrollbar = gameObject.GetComponent<Scrollbar>();
		if (scrollbar != null)
		{
			scrollbar.onValueChanged.AddListener(scrollbarValueChangeHandler);
		}

		Slider slider = gameObject.GetComponent<Slider>();
		if (slider != null)
		{
			slider.onValueChanged.AddListener(sliderValueChangeHandler);
		}

		Dropdown dropdown = gameObject.GetComponent<Dropdown>();
		if (dropdown != null)
		{
			dropdown.onValueChanged.AddListener(dropdownValueChangeHandler);
		}

		ScrollRect scrollrect = gameObject.GetComponent<ScrollRect>();
		if (scrollrect != null)
		{
			scrollrect.onValueChanged.AddListener(scrollrectValueChangeHandler);
		}
		//UIPlayTween playTweener = GetComponent<UIPlayTween>();
		//if (playTweener != null)
		//{
		//	EventDelegate.Add(playTweener.onFinished, OnPlayTweenFinish);
		//}
	}

	private void inputValueChangeHandler(string text)
	{
		if (inputvalueChangeAction != null)
		{
			inputvalueChangeAction(gameObject, text);
		}
	}

	private void inputEditEndHanler(string text)
	{
		if (inputeditEndAction != null)
		{
			inputeditEndAction(gameObject, text);
		}
	}

	private void toggleValueChangeHandler(bool select)
	{
		if (togglevalueChangeAction != null)
		{
			togglevalueChangeAction(gameObject, select);
		}
	}

	private void sliderValueChangeHandler(float value)
	{
		if (slidervalueChangeAction != null)
		{
			slidervalueChangeAction(gameObject, value);
		}
	}

	private void scrollbarValueChangeHandler(float value)
	{
		if (scrollbarvalueChangeAction != null) {
			scrollbarvalueChangeAction(gameObject, value);
		}
	}

	private void dropdownValueChangeHandler(int value)
	{
		if (dropdownvalueChangeAction != null) {
			dropdownvalueChangeAction(gameObject, value);
		}
	}

	private void scrollrectValueChangeHandler(Vector2 rect)
	{
		if (scrollrectvalueChangeAction != null)
		{
			scrollrectvalueChangeAction(gameObject, rect);
		}
	}

	public UnityAction<Vector2> scrollrectValueChangeHandler()
	{
		return delegate (Vector2 rect)
		{
			if (scrollrectvalueChangeAction != null)
			{
				scrollrectvalueChangeAction(gameObject,rect);
			}
		};
	}

	void OnPlayTweenFinish()
	{
        if (OnPlayTweenHandle != null) ;
			//OnPlayTweenHandle(UIPlayTween.current);
	}
}
}
//用来处理一些控件独有 不能统一处理的事件


