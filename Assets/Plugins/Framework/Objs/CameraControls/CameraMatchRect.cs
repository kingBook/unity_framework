#pragma warning disable 0649
using UnityEngine;
using System.Collections;
/// <summary>
/// （此脚本只用于透视相机）
/// </summary>
public class CameraMatchRect:BaseMonoBehaviour{
	
	public Transform min;
	public Transform max;
	public Camera viewCamera;
	public Camera.FieldOfViewAxis FOVAxis;
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
		if(FOVAxis==Camera.FieldOfViewAxis.Vertical){
			viewCamera.fieldOfView=Mathf.Atan(extents.y/distance)*Mathf.Rad2Deg*2f;
		}else if(FOVAxis==Camera.FieldOfViewAxis.Horizontal){
			float tempExtentsY=extents.x/viewCamera.aspect;
			viewCamera.fieldOfView=Mathf.Atan(tempExtentsY/distance)*Mathf.Rad2Deg*2f;
		}
	}

}