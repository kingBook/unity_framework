using UnityEngine;

public static class CameraUtil{
	
	/// <summary>
	/// 获取屏幕点在与相机相同旋转的平面上的点
	/// </summary>
	/// <param name="screenPoint">屏幕坐标点</param>
	/// <param name="camera">相机</param>
	/// <param name="planeInPoint">平面穿过的点</param>
	/// <returns></returns>
	public static Vector3 GetScreenPointToCameraRotationPlane(Vector3 screenPoint,Camera camera,Vector3 planeInPoint){
		Vector3 sreenToWorldPoint=camera.ScreenToWorldPoint(screenPoint);
		Vector3 cameraRotationNormalized=camera.transform.rotation*Vector3.forward;
		Plane plane=new Plane(cameraRotationNormalized,planeInPoint);
		return plane.ClosestPointOnPlane(sreenToWorldPoint);
	}
	
}
