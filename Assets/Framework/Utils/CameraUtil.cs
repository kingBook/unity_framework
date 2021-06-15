using UnityEngine;

public static class CameraUtil {

    /// <summary>
    /// 获取屏幕坐标点射线与指定平面的交点
    /// </summary>
    /// <param name="screenPoint">屏幕坐标点</param>
    /// <param name="camera">相机</param>
    /// <param name="plane">平面</param>
    /// <returns>返回射线与平面的交点</returns>
    /// <exception cref="Exception">射线没有穿过平面，即射线与平面平行或射线方向与平面相反</exception>
    public static Vector3 GetScreenRayCastToPlanePoint (Vector3 screenPoint, Camera camera, Plane plane) {
        Vector3 result = Vector3.zero;
        Ray ray = camera.ScreenPointToRay(screenPoint);
        if (plane.Raycast(ray, out float enter)) {
            result = ray.GetPoint(enter);
        } else {
            throw new System.Exception("射线没有穿过平面，即射线与平面平行或射线方向与平面相反。");
        }
        return result;
    }

}
