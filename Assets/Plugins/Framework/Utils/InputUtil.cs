using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using System;
/// <summary>
/// 用户输入工具类
/// </summary>
public static class InputUtil{
	
	/// <summary>
	/// 返回指定TouchPhase的第一个触摸点，未找到时touch.fingerId等于-1
	/// </summary>
	/// <param name="phase">触摸的阶段</param>
	/// <param name="ignorePointerOverUI">是否过滤触摸UI的触摸点</param>
	/// <returns>返回指定TouchPhase的第一个触摸点</returns>
	public static Touch GetFirstTouch(TouchPhase phase,bool ignorePointerOverUI){
		Touch touch;
		for(int i=0,len=Input.touchCount;i<len;i++){
			touch=Input.GetTouch(i);
			if(touch.phase!=phase)continue;
			if(ignorePointerOverUI&&EventSystem.current.IsPointerOverGameObject(touch.fingerId))continue;
			return touch;
		}
		//
		touch=new Touch{ fingerId=-1 };
		return touch;
	}
	
	/// <summary>
	/// 返回指定手指Id的Touch，未找到时touch.fingerId等于-1
	/// </summary>
	/// <param name="fingerId">手指 ID</param>
	/// <param name="isIgnorePointerOverUI">当手指ID指定的 Touch 位置在UI上时是否跳过</param>
	/// <param name="phases">触摸阶段</param>
	/// <returns></returns>
	public static Touch GetTouchWithFingerId(int fingerId,bool isIgnorePointerOverUI,params TouchPhase[] phases){
		Touch touch;
		for(int i=0,len=Input.touchCount;i<len;i++){
			touch=Input.GetTouch(i);
			if(touch.fingerId!=fingerId)continue;
			if(isIgnorePointerOverUI && EventSystem.current.IsPointerOverGameObject(fingerId))continue;
			if(phases.Length>0 && Array.IndexOf(phases,touch.phase)>-1){
				return touch;
			}else{
				return touch;
			}
		}
		//
		touch=new Touch{ fingerId=-1 };
		return touch;
	}
	
	/// <summary>
	/// 鼠标按下/触摸开始时返回true,并输出坐标。
	/// <br>鼠标未按下/未发生触摸时并返回false,并输出(0,0,0)。</br>
	/// <br>注意：只在鼠标左键按下时/触摸在Began阶段才返回true，并输出坐标</br>
	/// </summary>
	/// <param name="screenPoint">输出鼠标/触摸点的屏幕坐标</param>
	/// <param name="fingerId">鼠标模式输出0，触摸模式输出手指id，鼠标未按下/未发生触摸时输出-1</param>
	/// <param name="isIgnorePointerOverUI">忽略UI上的点击，默认true</param>
	/// <returns></returns>
	public static bool GetInputScreenPoint(out Vector3 screenPoint,out int fingerId,bool isIgnorePointerOverUI=true){
		fingerId=-1;
		screenPoint=new Vector3();
		if(isIgnorePointerOverUI&&IsPointerOverUI()){
			//忽略UI上的点击
		}else if(Input.touchSupported){
			if(Input.touchCount>0){
				Touch touch=Input.GetTouch(0);
				if(touch.phase==TouchPhase.Began){
					screenPoint=touch.position;
					fingerId=touch.fingerId;
					return true;
				}
			}
		}else{
			if(Input.GetMouseButtonDown(0)){
				screenPoint=Input.mousePosition;
				fingerId=0;
				return true;
			}
		}
		return false;
	}

	/// <summary>
	/// 检测鼠标左键按下时/第一个触摸点在Began阶段是否接触UI
	/// </summary>
	/// <returns></returns>
	public static bool IsPointerOverUI(){
		bool result=false;
		if(Input.touchSupported){
			if(Input.touchCount>0) {
				Touch touch=Input.GetTouch(0);
				if(touch.phase==TouchPhase.Began){
					if(EventSystem.current.IsPointerOverGameObject(touch.fingerId)){
						result=true;
					}
				}
			}
		}else{
			if(Input.GetMouseButton(0)){
				if(EventSystem.current.IsPointerOverGameObject()){
					result=true;
				}
			}
		}
		return result;
	}

	/// <summary>
	/// 检测鼠标左键/指定fingerId的触摸是否释放
	/// </summary>
	/// <param name="fingerId"></param>
	/// <returns></returns>
	public static bool IsTouchUp(int fingerId){
		bool result=true;
		if(Input.touchSupported){
			Touch touch=GetTouchWithFingerId(fingerId,false);
			if(touch.fingerId>-1){
				if(touch.phase==TouchPhase.Began || touch.phase==TouchPhase.Moved ||touch.phase==TouchPhase.Stationary){
					result=false;
				}
			}
		}else{
			if(Input.GetMouseButton(0)){
				result=false;
			}
		}
		return result;
	}
	
}
