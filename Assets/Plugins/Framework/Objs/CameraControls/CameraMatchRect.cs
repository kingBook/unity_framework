#pragma warning disable 0649
using UnityEngine;
using System.Collections;
/// <summary>
/// 设置透视相机的在不同的屏幕分辨率下自动设置大小匹配一个矩形（仅用于透视相机）
/// </summary>
public class CameraMatchRect:BaseMonoBehaviour{
	
	public Transform min;
	public Transform max;
	public Camera viewCamera;
	public bool isUpdate;

#if UNITY_EDITOR
	protected override void Reset() {
		base.Reset();
		if(!viewCamera){
			viewCamera=GetComponent<Camera>();
		}
	}
#endif

	protected override void Start() {
		base.Start();
		Fit();
	}

	protected override void LateUpdate2() {
		base.LateUpdate2();
		if(isUpdate)Fit();
	}

	private void Fit(){
		Vector3 center=(min.position+max.position)*0.5f;
		viewCamera.transform.LookAt(center);
		float distance=Vector3.Distance(center,viewCamera.transform.position);
		Vector3 extents=(max.position-min.position)*0.5f;

		float referenceScaleFactor=extents.x/extents.y;
		float scaleFactor=(float)Screen.width/Screen.height;

		if(scaleFactor>referenceScaleFactor){
			//匹配高度
			viewCamera.fieldOfView=Mathf.Atan(extents.y/distance)*Mathf.Rad2Deg*2f;
		}else if(scaleFactor<referenceScaleFactor){
			//匹配宽度
			float tempExtentsY=extents.x/viewCamera.aspect;
			viewCamera.fieldOfView=Mathf.Atan(tempExtentsY/distance)*Mathf.Rad2Deg*2f;
		}
	}

}