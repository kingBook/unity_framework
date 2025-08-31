using Cinemachine;
using System.Collections;
using UnityEngine;

public static class CinemachineUtil {

    /// <summary>
    /// 获取在路径上距离目标最近的点
    /// </summary>
    /// <param name="path"> CinemachinePath </param>
    /// <param name="target"> 目标 (世界坐标) </param>
    /// <param name="pathUnitsValue"> 输出在路径上的 PathUnits 值 </param>
    /// <returns> 返回最近的点（世界坐标） </returns>
    public static Vector3 GetClosestPointOnPath (CinemachinePathBase path, Vector3 target, out float pathUnitsValue) {
        pathUnitsValue = path.FindClosestPoint(target, 0, -1, 0);
        Vector3 closestPointOnPath = path.EvaluatePositionAtUnit(pathUnitsValue, CinemachinePathBase.PositionUnits.PathUnits);
        return closestPointOnPath;
    }

    /// <summary>
    /// 获取在路径列表中距离目标最近的路径上的最近的点
    /// </summary>
    /// <param name="paths"> 路径列表 </param>
    /// <param name="target"> 目标 (世界坐标) </param>
    /// <param name="pathIndex"> 输出距离最近的路径索引 </param>
    /// <param name="pathUnitsValue"> 输出在最近路径上的 PathUnits 值 </param>
    /// <returns> 返回距离最近的路径上最近的点（世界坐标） </returns>
    public static Vector3 GetClosestPointOnPaths (CinemachinePathBase[] paths, Vector3 target, out int pathIndex, out float pathUnitsValue) {
        pathIndex = -1;
        pathUnitsValue = 0f;
        Vector3 resultPos = Vector3.zero;

        float minDistance = float.MaxValue;
        for (int i = 0, len = paths.Length; i < len; i++) {
            CinemachinePathBase path = paths[i];
            Vector3 pointOnPath = GetClosestPointOnPath(path, target, out float pathUnits);
            float distance = Vector3Util.DistanceXZ(target, pointOnPath);
            if (distance < minDistance) {
                minDistance = distance;
                pathIndex = i;
                pathUnitsValue = pathUnits;
                resultPos = pointOnPath;
            }
        }
        return resultPos;
    }
}
