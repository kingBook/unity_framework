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
	
	private Camera m_camera;
	private DriftCamera m_driftCamera;
	private bool m_isRotateBegin;
	/// <summary>
	/// 鼠标左键，在按下开始时是否接触UI
	/// </summary>
	private bool m_isMouseOverUIOnBegan;
	private int m_touchFingerId=-1;
	
    protected override void Start(){
		base.Start();
		m_camera=GetComponent<Camera>();
		m_driftCamera=GetComponent<DriftCamera>();
    }
	
	protected override void Update2(){
		base.Update2();
		if(Input.touchSupported){
			TouchHandler();
		}else{
			MouseHandler();
		}
	}
	
	private void TouchHandler(){
		Touch touch;
		//返回一个在触摸开始阶段时非触摸UI的触摸点
		if(m_touchFingerId>-1){
			touch=InputUtil.GetTouchWithFingerId(m_touchFingerId);
			m_touchFingerId=touch.fingerId;
		}else{
			touch=InputUtil.GetTouchNonPointerOverUI(TouchPhase.Began);
			m_touchFingerId=touch.fingerId;
		}
		
		if(touch.fingerId>-1){
			float h=touch.deltaPosition.x*0.5f;
			if(!m_isRotateBegin){
				m_isRotateBegin=true;
				onPreRotateEvent?.Invoke(h);
			}
			//单点触摸上下左右旋转
			Rotate(h);
		}
	}
	
	private void MouseHandler(){
		//按下鼠标左键时，鼠标是否接触UI
		if(Input.GetMouseButtonDown(0)){
			m_isMouseOverUIOnBegan=EventSystem.current.IsPointerOverGameObject();
		}
		
		//非移动设备按下鼠标左键旋转
		if(Input.GetMouseButton(0)){
			//鼠标按下左键时没有接触UI
			if(!m_isMouseOverUIOnBegan){
				float h=Input.GetAxis("Mouse X");
				h*=10;
				
				if(!m_isRotateBegin){
					m_isRotateBegin=true;
					onPreRotateEvent?.Invoke(h);
				}
				Rotate(h);
			}
		}else{
			m_isRotateBegin=false;
		}
	}
	
	/// <summary>
	/// 旋转
	/// </summary>
	private void Rotate(float h){
		//应用到DriftCamera
		if(advancedOptions.isApplyToDriftCamera){
			if(m_driftCamera!=null){
				Quaternion rotation=Quaternion.AngleAxis(h,Vector3.up);
				m_driftCamera.originPositionNormalized=rotation*m_driftCamera.originPositionNormalized;
			}
		}
		//绕着pivot旋转Y轴，实现左右旋转
		m_camera.transform.RotateAround(targetTransform.position,Vector3.up,h);
		//
		onRotateEvent?.Invoke(h);
	}
    
    protected override void OnDestroy(){
        base.OnDestroy();
    }
	
}