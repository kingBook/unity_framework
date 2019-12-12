using System;
using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// 手指在屏幕上水平移动旋转相机的Y轴
/// </summary>
public class ManualRotateCameraY:BaseMonoBehaviour{
	
	[Tooltip("相机看向的目标")]
	public Transform targetTransform;
	
	[System.Serializable]
	public class AdvancedOptions{
		[Tooltip("是否应用到DriftCamera组件(存在时有效)")]
		public bool isApplyToDriftCamera=true;
	}
	public AdvancedOptions advancedOptions;
	
	/// <summary>
	/// 旋转前 void(float h)
	/// </summary>
	public event Action<float> onPreRotateEvent;
	/// <summary>
	/// 旋转 void(float h)
	/// </summary>
	public event Action<float> onRotateEvent;
	
	private Camera _camera;
	private DriftCamera _driftCamera;
	private bool _isRotateBegin;
	/// <summary>
	/// 鼠标左键，在按下开始时是否接触UI
	/// </summary>
	private bool _isMouseOverUIOnBegan;
	private int _touchFingerId=-1;
	
    protected override void Start(){
		base.Start();
		_camera=GetComponent<Camera>();
		_driftCamera=GetComponent<DriftCamera>();
    }
	
	protected override void Update2(){
		base.Update2();
		if(Input.touchSupported){
			touchHandler();
		}else{
			mouseHandler();
		}
	}
	
	private void touchHandler(){
		Touch touch;
		//返回一个在触摸开始阶段时非触摸UI的触摸点
		if(_touchFingerId>-1){
			touch=InputUtil.getTouchWithFingerId(_touchFingerId);
			_touchFingerId=touch.fingerId;
		}else{
			touch=InputUtil.getTouchNonPointerOverUI(TouchPhase.Began);
			_touchFingerId=touch.fingerId;
		}
		
		if(touch.fingerId>-1){
			float h=touch.deltaPosition.x*0.5f;
			if(!_isRotateBegin){
				_isRotateBegin=true;
				onPreRotateEvent?.Invoke(h);
			}
			//单点触摸上下左右旋转
			rotate(h);
		}
	}
	
	private void mouseHandler(){
		//按下鼠标左键时，鼠标是否接触UI
		if(Input.GetMouseButtonDown(0)){
			_isMouseOverUIOnBegan=EventSystem.current.IsPointerOverGameObject();
		}
		
		//非移动设备按下鼠标左键旋转
		if(Input.GetMouseButton(0)){
			//鼠标按下左键时没有接触UI
			if(!_isMouseOverUIOnBegan){
				float h=Input.GetAxis("Mouse X");
				h*=10;
				
				if(!_isRotateBegin){
					_isRotateBegin=true;
					onPreRotateEvent?.Invoke(h);
				}
				rotate(h);
			}
		}else{
			_isRotateBegin=false;
		}
	}
	
	/// <summary>
	/// 旋转
	/// </summary>
	private void rotate(float h){
		//应用到DriftCamera
		if(advancedOptions.isApplyToDriftCamera){
			if(_driftCamera!=null){
				Quaternion rotation=Quaternion.AngleAxis(h,Vector3.up);
				_driftCamera.originPositionNormalized=rotation*_driftCamera.originPositionNormalized;
			}
		}
		//绕着pivot旋转Y轴，实现左右旋转
		_camera.transform.RotateAround(targetTransform.position,Vector3.up,h);
		//
		onRotateEvent?.Invoke(h);
	}
    
    protected override void OnDestroy(){
        base.OnDestroy();
    }
	
}