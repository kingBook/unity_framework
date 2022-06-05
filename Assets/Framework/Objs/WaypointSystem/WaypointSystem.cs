using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using OrderType = WaypointPath.OrderType;

/// <summary>
/// 路径点寻路系统（基于xz平面）
/// <para> 基于A*寻路 https://zhuanlan.zhihu.com/p/348522882 </para>
/// </summary>
public class WaypointSystem {

    private WaypointPath[] m_paths;
    private List<WaypointObject> m_openList;
    private List<WaypointObject> m_closeList;
    private WaypointObject[] m_neighorList;


    public WaypointPath[] paths => m_paths;


    public WaypointSystem(WaypointPath[] paths) {
        m_paths = paths;
        m_openList = new List<WaypointObject>(1024);
        m_closeList = new List<WaypointObject>(1024);
        m_neighorList = new WaypointObject[32];
    }

    private void InitFind() {
        // 清空所有 WaypointObject 的 cost、parent
        ClearAllWaypointsCostAndParent();
        m_openList.Clear();
        m_closeList.Clear();
    }

    /// <summary>
    /// 寻路
    /// <para> 注意寻路的结果是反向的 </para>
    /// </summary>
    /// <param name="start"></param>
    /// <param name="target"></param>
    /// <param name="results"></param>
    /// <returns></returns>
    public int FindPathNonAlloc((int pathIndex, int wayPointIndex) start, (int pathIndex, int wayPointIndex) target, WaypointObject[] results) {
        int resultsWaypointsCount = 0;

        var startWaypoint = m_paths[start.pathIndex].waypoints[start.wayPointIndex];
        var targetWaypoint = m_paths[target.pathIndex].waypoints[target.wayPointIndex];

        InitFind();
        m_openList.Add(startWaypoint);
        while (m_openList.Count > 0) {
            // 寻找 open 列表中的F最小的点，如果F相同，选取H最小的
            var currentWaypoint = GetLowestCostWaypointInOpenList(out int inOpenIndex);

            // 把当前点从 open 列表中移除，并加入到 close 列表
            m_openList.RemoveAt(inOpenIndex);
            m_closeList.Add(currentWaypoint);


            // 如果是目标点，返回
            if (currentWaypoint == targetWaypoint) {
                resultsWaypointsCount = GeneratePath(startWaypoint, targetWaypoint, results);
                break;
            }

            // 搜索当前点的所有相邻点
            int neighorCount = GetNeighorNonAlloc(currentWaypoint, m_neighorList);
            for (int i = 0; i < neighorCount; i++) {
                var waypoint = m_neighorList[i];
                // 如果点已在关闭列表中，跳过
                if (m_closeList.Contains(waypoint)) continue;
                float gCost = currentWaypoint.gCost + GetDistanceWaypoints(currentWaypoint, waypoint);
                // 如果新路径到相邻点的距离更短 或不在 open 列表中
                bool isInOpenList = m_openList.Contains(waypoint);
                if (gCost < waypoint.gCost || !isInOpenList) {
                    // 更新相邻点的F，G，H
                    waypoint.gCost = gCost;
                    waypoint.hCost = GetDistanceWaypoints(waypoint, targetWaypoint);
                    // 设置相邻点的父级点为当前点
                    waypoint.parent = currentWaypoint;
                    // 如果不在 open 列表中，加入到 open 列表
                    if (!isInOpenList) {
                        m_openList.Add(waypoint);
                    }
                }
            }
        }
        return resultsWaypointsCount;
    }

    /// <summary>
    /// 寻找 open 列表中的F最小的点，如果F相同，选取H最小的
    /// </summary>
    /// <param name="inOpenIndex"> 返回的点在 open 列表中的索引 </param>
    /// <returns></returns>
    private WaypointObject GetLowestCostWaypointInOpenList(out int inOpenIndex) {
        inOpenIndex = 0;
        WaypointObject currentWaypoint = m_openList[0];
        for (int i = 0, length = m_openList.Count; i < length; i++) {
            var waypoint = m_openList[i];
            if (waypoint.fCost < currentWaypoint.fCost || waypoint.fCost == currentWaypoint.fCost && waypoint.hCost < currentWaypoint.hCost) {
                inOpenIndex = i;
                currentWaypoint = waypoint;
            }
        }
        return currentWaypoint;
    }


    private void ClearAllWaypointsCostAndParent() {
        for (int i = 0, lenI = m_paths.Length; i < lenI; i++) {
            var path = m_paths[i];
            for (int j = 0, lenJ = path.waypoints.Count; j < lenJ; j++) {
                var waypoint = path.waypoints[j];
                waypoint.gCost = 0.0f;
                waypoint.hCost = 0.0f;
                waypoint.parent = null;
            }
        }
    }

    private int GetNeighorNonAlloc(WaypointObject waypoint, WaypointObject[] results) {
        int resultsIndex = -1;
        int overlapPointsCount = waypoint.overlapPointsCount;
        if (overlapPointsCount <= 0) {
            var path = m_paths[waypoint.pathIndex];
            GetNeighorOnPathNonAlloc(path, waypoint.wayPointIndex, results, ref resultsIndex);
        }
        // 是重叠点时
        for (int i = 0; i < overlapPointsCount; i++) {
            WaypointObject overlapWaypoint = waypoint.overlapPoints[i];
            var path = m_paths[overlapWaypoint.pathIndex];
            GetNeighorOnPathNonAlloc(path, overlapWaypoint.wayPointIndex, results, ref resultsIndex);
        }
        return resultsIndex + 1;
    }

    private void GetNeighorOnPathNonAlloc(WaypointPath path, int waypointIndex, WaypointObject[] results, ref int resultsIndex) {
        int pathWaypointsCount = path.waypoints.Count;
        int ia, ib; // ia表示后退的索引，ib表示前进的索引

        switch (path.orderType) {
            case OrderType.TwoWay:
                if (path.isClose) {
                    ia = (waypointIndex - 1 + pathWaypointsCount) % pathWaypointsCount;
                    ib = (waypointIndex + 1) % pathWaypointsCount;
                    results[++resultsIndex] = path.waypoints[ia];
                    results[++resultsIndex] = path.waypoints[ib];
                } else {
                    if (waypointIndex > 0) {
                        ia = waypointIndex - 1;
                        results[++resultsIndex] = path.waypoints[ia];
                    }
                    if (waypointIndex < pathWaypointsCount - 1) {
                        ib = waypointIndex + 1;
                        results[++resultsIndex] = path.waypoints[ib];
                    }
                }
                break;
            case OrderType.OneWayAZ:
                if (path.isClose) {
                    ib = (waypointIndex + 1) % pathWaypointsCount;
                    results[++resultsIndex] = path.waypoints[ib];
                } else {
                    if (waypointIndex < pathWaypointsCount - 1) {
                        ib = waypointIndex + 1;
                        results[++resultsIndex] = path.waypoints[ib];
                    }
                }
                break;
            case OrderType.OneWayZA:
                if (path.isClose) {
                    ia = (waypointIndex - 1 + pathWaypointsCount) % pathWaypointsCount;
                    results[++resultsIndex] = path.waypoints[ia];
                } else {
                    if (waypointIndex > 0) {
                        ia = waypointIndex - 1;
                        results[++resultsIndex] = path.waypoints[ia];
                    }
                }
                break;
        }
    }

    private float GetDistanceWaypoints(WaypointObject waypoint1, WaypointObject waypoint2) {
        float dx = Mathf.Abs(waypoint2.position.x - waypoint1.position.x);
        float dz = Mathf.Abs(waypoint2.position.z - waypoint2.position.z);
        //return Mathf.Sqrt(dx * dx + dz * dz);
        return dx + dz; // 曼哈顿估价
    }

    private int GeneratePath(WaypointObject startWaypoint, WaypointObject targetWaypoint, WaypointObject[] results) {
        int index = -1;
        var waypoint = targetWaypoint;
        while (waypoint != startWaypoint) {
            results[++index] = waypoint;
            waypoint = waypoint.parent;
        }
        results[++index] = startWaypoint;
        return index + 1;
    }

}
