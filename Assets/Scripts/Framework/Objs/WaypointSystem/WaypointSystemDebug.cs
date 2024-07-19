using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class WaypointSystemDebug {

    private WaypointSystem m_waypointSystem;
    private int m_nextPathIndex;


    /// <summary>
    /// 使用 <see cref="Debug.DrawLine(Vector3, Vector3, Color, float)"/> 画寻路的结果
    /// </summary>
    /// <param name="count"></param>
    /// <param name="results"></param>
    /// <param name="color"></param>
    /// <param name="duration"></param>
    public static void DrawFindPathResults(int count, WaypointObject[] results, Color color, float duration) {
        int i = count;
        while (--i >= 1) {
            var current = results[i];
            var next = results[i - 1];
            Debug.DrawLine(current.position, next.position, color, duration);
        }
    }


    public WaypointSystemDebug(WaypointSystem waypointSystem) {
        m_waypointSystem = waypointSystem;
    }

    public string GetNextPathString() {
        StringBuilder stringBuilder = new StringBuilder();
        var path = m_waypointSystem.paths[m_nextPathIndex];
        int waypointCount = path.waypoints.Count;
        m_nextPathIndex = (m_nextPathIndex + 1) % waypointCount;
        for (int i = 0; i < waypointCount; i++) {
            var wpo = path.waypoints[i];
            stringBuilder.AppendFormat("{0};\n", wpo);
        }
        return stringBuilder.ToString();
    }

    public bool TestEqual((int pathIndex, int wayPointIndex) a, (int pathIndex, int wayPointIndex) b) {
        WaypointObject wpoa = m_waypointSystem.paths[a.pathIndex].waypoints[a.wayPointIndex];
        WaypointObject wpob = m_waypointSystem.paths[b.pathIndex].waypoints[b.wayPointIndex];
        return wpoa == wpob;
    }


}
