using System;
using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// 手指在屏幕上水平移动旋转相机的Y轴
/// </summary>
public class ManualRotateCameraY:BaseMonoBehaviour{
	[System.Serializable]
    public class AdvancedOptions{
		public bool isLookToTargetOnStart=true;
	}
	public Transform lookAtTarget;
	[Tooltip("相机相对于目标点的单位化位置")]
	public Vector3 originPositionNormalized=new Vector3(0.2f,0.68f,-1.0f);
	[Tooltip("相机与目标点的距离")]
	public float distance=4.0f;
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
	private bool _isRotateBegin;
	/// <summary>
	/// 触摸点0/鼠标左键，在触摸/按下开始时是否接触UI
	/// </summary>
	private bool _isPointerOverUIOnBegan;
	private int _touchFingerId=-1;
    
	protected override void Awake() {
		base.Awake();
		_camera=GetComponent<Camera>();
	}
	
    protected override void Start(){
		base.Start();
		if(advancedOptions.isLookToTargetOnStart){
			lookToTarget();
		}
    }
    
    private void lookToTarget(){
		if(lookAtTarget==null)return;
        //计算相机原点
		Vector3 offset=originPositionNormalized*distance;
		offset=lookAtTarget.rotation*offset;
		Vector3 positionTarget=lookAtTarget.position+offset;
        //移动相机
		transform.position=positionTarget;
        //旋转相机朝向
		transform.LookAt(lookAtTarget);
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
			_isPointerOverUIOnBegan=EventSystem.current.IsPointerOverGameObject();
		}
		
		//非移动设备按下鼠标左键旋转
		if(Input.GetMouseButton(0)){
			//鼠标按下左键时没有接触UI
			if(!_isPointerOverUIOnBegan){
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
		//绕着pivot旋转Y轴，实现左右旋转
		_camera.transform.RotateAround(lookAtTarget.position,Vector3.up,h);
		//绕着pivot旋转相机朝向的右侧轴向,实现上下旋转
		int cameraAngleX=(int)_camera.transform.rotation.eulerAngles.x;
		onRotateEvent?.Invoke(h);
	}
    
    protected override void OnDestroy(){
        base.OnDestroy();
    }
	
}