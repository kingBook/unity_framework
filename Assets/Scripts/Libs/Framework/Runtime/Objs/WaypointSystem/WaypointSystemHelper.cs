using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class WaypointSystemHelper {

    /// <summary>
    /// 重叠点的距离阀值，两个点的距离小于此值则视为重叠点
    /// </summary>
    private const float OVERLAP_THRESHOLD = 0.02f;

    public static void ConvertToVector3sNonAlloc(WaypointObject[] waypointObjects, int count, Vector3[] results, bool isReverse) {
        for (int i = 0; i < count; i++) {
            if (isReverse) {
                results[count - 1 - i] = waypointObjects[i].position;
            } else {
                results[i] = waypointObjects[i].position;
            }
        }
    }

    public static WaypointPath[] ConvertCinemachinePaths(CinemachineSmoothPath[] cmPaths) {
        int pathCount = cmPaths.Length;
        var results = new WaypointPath[pathCount];
        for (int i = 0; i < pathCount; i++) {
            var path = cmPaths[i];
            var wpos = new List<WaypointObject>();
            for (int j = 0, lenJ = path.m_Waypoints.Length; j < lenJ; j++) {
                var pt = path.m_Waypoints[j].position;
                WaypointObject wpo = new WaypointObject {
                    pathIndex = (ushort)i,
                    wayPointIndex = (ushort)j,
                    position = pt,
                    overlapPoints = null,
                    overlapPointsCount = 0
                };
                wpos.Add(wpo);
            }

            var orderType = WaypointPath.OrderType.TwoWay;
            var additionalPathData = path.GetComponent<AdditionalPathData>();
            if (additionalPathData && additionalPathData.orderType != 0) {
                orderType = additionalPathData.orderType;
            }

            var wpoPath = new WaypointPath(wpos, path.Looped, orderType);
            results[i] = wpoPath;
        }
        // 设置共点列表
        for (int i = 0; i < pathCount; i++) {
            var wpoPath = results[i];
            for (int j = 0, lenJ = wpoPath.waypoints.Count; j < lenJ; j++) {
                var wpo = wpoPath.waypoints[j];
                if (wpo.overlapPointsCount <= 0) {
                    SetOverlapPoints(results, wpo);
                }
            }
        }
        return results;
    }

    public static WaypointPath[] ConvertCinemachinePaths(CinemachinePath[] cmPaths) {
        int pathCount = cmPaths.Length;
        var results = new WaypointPath[pathCount];
        for (int i = 0; i < pathCount; i++) {
            var path = cmPaths[i];
            var wpos = new List<WaypointObject>();
            for (int j = 0, lenJ = path.m_Waypoints.Length; j < lenJ; j++) {
                var pt = path.m_Waypoints[j].position;
                WaypointObject wpo = new WaypointObject {
                    pathIndex = (ushort)i,
                    wayPointIndex = (ushort)j,
                    position = pt,
                    overlapPoints = null,
                    overlapPointsCount = 0
                };
                wpos.Add(wpo);
            }
            var wpoPath = new WaypointPath(wpos, path.Looped, WaypointPath.OrderType.TwoWay);
            results[i] = wpoPath;
        }
        // 设置共点列表
        for (int i = 0; i < pathCount; i++) {
            var wpoPath = results[i];
            for (int j = 0, lenJ = wpoPath.waypoints.Count; j < lenJ; j++) {
                var wpo = wpoPath.waypoints[j];
                if (wpo.overlapPointsCount <= 0) {
                    SetOverlapPoints(results, wpo);
                }
            }
        }
        return results;
    }

    private static List<WaypointObject> SetOverlapPoints(WaypointPath[] waypointPaths, WaypointObject wpo) {
        ref var position = ref wpo.position;
        List<WaypointObject> list = null;
        for (int i = 0, lenI = waypointPaths.Length; i < lenI; i++) {
            var wpoPath = waypointPaths[i];
            for (int j = 0, lenJ = wpoPath.waypoints.Count; j < lenJ; j++) {
                var wpoj = wpoPath.waypoints[j];
                if (wpoj.pathIndex == wpo.pathIndex && wpoj.wayPointIndex == wpo.wayPointIndex) continue;
                ref var pt = ref wpoj.position;
                float dx = position.x - pt.x;
                float dz = position.z - pt.z;
                float distance = Mathf.Sqrt(dx * dx + dz + dz);
                if (distance <= OVERLAP_THRESHOLD) {
                    if (list == null) {
                        list = new List<WaypointObject> { wpo };
                    }
                    list.Add(wpoj);
                }
            }
        }

        if (list != null) {
            int overlapPointsCount = list.Count;
            for (int i = 0; i < overlapPointsCount; i++) {
                var wpoi = list[i];
                wpoi.overlapPoints = list;
                wpoi.overlapPointsCount = (ushort)overlapPointsCount;
            }
        }
        return list;
    }
}
