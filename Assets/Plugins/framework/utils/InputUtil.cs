using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
/// <summary>
/// 设备输入工具类
/// </summary>
public static class InputUtil{
	
	/// <summary>
	/// 返回在TouchBegan时非接触UI的触摸点，未找到时touch.fingerId等于-1
	/// </summary>
	/// <param name="phase">判断触摸的阶段</param>
	/// <returns>返回在TouchBegan时非接触UI的触摸点</returns>
	public static Touch GetTouchNonPointerOverUI(TouchPhase phase){
		Touch touch;
		for(int i=0;i<Input.touchCount;i++){
			touch=Input.GetTouch(i);
			if(touch.phase!=phase)continue;
			if(!EventSystem.current.IsPointerOverGameObject(touch.fingerId)){
				return touch;
			}
		}
		//
		touch=new Touch();
		touch.fingerId=-1;
		return touch;
	}
	
	/// <summary>
	/// 返回指定手指Id的Touch，未找到时touch.fingerId等于-1
	/// </summary>
	/// <param name="fingerId"></param>
	/// <returns></returns>
	public static Touch GetTouchWithFingerId(int fingerId){
		return GetTouchWithFingerId(fingerId,0,false);
	}
	public static Touch GetTouchWithFingerId(int fingerId,TouchPhase phase){
		return GetTouchWithFingerId(fingerId,phase,true);
	}
	private static Touch GetTouchWithFingerId(int fingerId,TouchPhase phase,bool isCheckPhase){
		Touch touch;
		for(int i=0;i<Input.touchCount;i++){
			touch=Input.GetTouch(i);
			if(isCheckPhase&&touch.phase!=phase)continue;
			if(touch.fingerId==fingerId){
				return touch;
			}
		}
		//
		touch=new Touch();
		touch.fingerId=-1;
		return touch;
	}
	
	/// <summary>
	/// 鼠标按下/触摸开始时返回true,并输出坐标。
	/// <br>鼠标未按下/未发生触摸时并返回false,并输出(0,0,0)。</br>
	/// <br>注意：只在鼠标左键按下时/触摸在Began阶段才返回true，并输出坐标</br>
	/// </summary>
	/// <param name="screenPoint">输出鼠标/触摸点的屏幕坐标</param>
	/// <param name="isIgnorePointerOverUI">忽略UI上的点击，默认true</param>
	/// <returns></returns>
	public static bool GetMouseDownScreenPoint(out Vector3 screenPoint,bool isIgnorePointerOverUI=true){
		screenPoint=new Vector3();
		if(isIgnorePointerOverUI&&IsPointerOverUI()){
			//忽略UI上的点击
		}else if(Input.touchSupported){
			if(Input.touchCount>0){
				Touch touch=Input.GetTouch(0);
				if(touch.phase==TouchPhase.Began){
					screenPoint=touch.position;
					return true;
				}
			}
		}else{
			if(Input.GetMouseButtonDown(0)){
				screenPoint=Input.mousePosition;
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
	
}
