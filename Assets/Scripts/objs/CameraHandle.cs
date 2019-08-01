using UnityEngine;
using System.Collections;

/// <summary>
/// 单点触摸控制相机的旋转，两点触摸缩放和平移相机视野。
/// </summary>
public class CameraHandle:BaseMonoBehaviour{
	[Tooltip("缩放和旋转围绕的中心")]
	public Transform pivot;
	[Tooltip("上下旋转限制的最小角度")]
	public int verticalAngleMin=10;
	[Tooltip("上下旋转限制的最大角度")]
	public int verticalAngleMax=60;
	[Tooltip("鼠标滚轮的速度倍数")]
	public float scrollWheelMultiple=10;
	[Tooltip("缩放时相机视野最小值")]
	public float fieldOfViewMin=10;
	[Tooltip("缩放时相机视野最大值")]
	public float fieldOfViewMax=100;
	[Tooltip("移动平台视野缩放的倍数")]
	public float fieldOfViewMultiple=0.1f;

	private Camera _camera;
	private float _oldDistance;

	protected override void Awake() {
		base.Awake();
		_camera=GetComponent<Camera>();
	}

	protected override void Start(){
		base.Start();
		
	}

	protected override void Update2() {
		base.Update2();
		
		if(Input.touchSupported){
			if(Input.touchCount==1){
				//单点触摸上下左右旋转
				Touch touch=Input.GetTouch(0);
				rotate(touch.deltaPosition.x*0.5f,touch.deltaPosition.y*0.1f);
			}else if(Input.touchCount==2){
				//两点触摸缩放视野
				Touch touch1=Input.GetTouch(0);
				Touch touch2=Input.GetTouch(1);
				float distance=Vector2.Distance(touch1.position,touch2.position);
				if(touch2.phase==TouchPhase.Began){
					_oldDistance=distance;
				}
				float offset=(distance-_oldDistance)*fieldOfViewMultiple;
				zoomFieldOfView(-offset);
				_oldDistance=distance;
				//两点触摸平移
				Vector2 translateVel=(touch1.deltaPosition+touch2.deltaPosition)*0.5f*-0.01f;
				translate(translateVel);
			}
		}else{
			//非移动设备按下鼠标左键旋转
			if(Input.GetMouseButton(0)){
				float h=Input.GetAxis("Mouse X");
				float v=Input.GetAxis("Mouse Y");
				rotate(h*10,v*10);
			}
			//非移动设备滚动鼠标中键缩放视野
			float scroll=scrollWheelMultiple*Input.GetAxis("Mouse ScrollWheel");
			zoomFieldOfView(scroll*10);
			//非移动设备按下鼠标右键平移
			if(Input.GetMouseButton(1)){
				float h=Input.GetAxis("Mouse X");
				float v=Input.GetAxis("Mouse Y");
				Vector2 translateVel=new Vector2(-h*0.1f,-v*0.1f);
				translate(translateVel*3);
			}
		}
	}

	/// <summary>
	/// 旋转
	/// </summary>
	private void rotate(float h,float v){
		//绕着pivot旋转Y轴，实现左右旋转
		_camera.transform.RotateAround(pivot.position,Vector3.up, h);
		//绕着pivot旋转相机朝向的右侧轴向,实现上下旋转
		int cameraAngleX=(int)_camera.transform.rotation.eulerAngles.x;
		//限制最大速度，避免出错
		const float maxV=5;
		v=Mathf.Clamp(v,-maxV,maxV);
		//
		if(v>=0){
			//限制上下旋转最小角度
			if(cameraAngleX>verticalAngleMin){
				_camera.transform.RotateAround(pivot.position,_camera.transform.right,-v);
			}
		}else{
			//限制上下旋转最大角度
			if(cameraAngleX<verticalAngleMax){
				_camera.transform.RotateAround(pivot.position,_camera.transform.right,-v);
			}
		}
	}

	/// <summary>
	/// 缩放视野
	/// </summary>
	/// <param name="velocity">缩放视野速度向量</param>
	private void zoomFieldOfView(float velocity){
		_camera.fieldOfView=Mathf.Clamp(_camera.fieldOfView+velocity,fieldOfViewMin,fieldOfViewMax);
	}

	/// <summary>
	/// 平移
	/// </summary>
	/// <param name="velocity">平移速度向量</param>
	private void translate(Vector3 velocity){
		_camera.transform.Translate(velocity);
	}
}
