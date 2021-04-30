using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestLineSegmentIntersectCircle : MonoBehaviour {

    public Transform lineStart;
    public Transform lineEnd;

    public Transform circleCenter;
    public float radius = 1;

    /// <summary> 交点1 </summary>
    private Vector2 m_intersection1;
    private Vector2 m_intersection2;


    private void DrawGizmosCircle (Vector3 circleCenter, float radius, float thetaStep = 0.1f) {
        thetaStep = Mathf.Max(thetaStep, 1e-4f);
        Vector3 firstPoint = Vector3.zero;
        Vector3 lastPoint = Vector3.zero;
        Vector3 currentPoint = Vector3.zero;
        for (float theta = 0; theta < 2 * Mathf.PI; theta += thetaStep) {
            float x = radius * Mathf.Cos(theta);
            float y = radius * Mathf.Sin(theta);
            if (theta == 0) {
                firstPoint.Set(x, y, 0);
                lastPoint.Set(x, y, 0);
            } else {
                currentPoint.Set(x, y, 0);
                Gizmos.DrawLine(lastPoint + circleCenter, currentPoint + circleCenter);
                lastPoint.Set(x, y, 0);
            }
        }
        Gizmos.DrawLine(firstPoint + circleCenter, lastPoint + circleCenter); // 绘制最后一条线段
    }

    private void Update () {
        Vector2 direction = lineEnd.position - lineStart.position;

        //GeomUtil.Ray2DIntersectCircle(lineStart.position, direction, circleCenter.position, radius, out m_intersection1, out m_intersection2);

        int intersectionCount = GeomUtil.LineSegmentIntersectCircle(lineStart.position, lineEnd.position, circleCenter.position, radius, out m_intersection1, out m_intersection2);
        Debug.Log("线段与圆交点的数量："+intersectionCount);
    }

    private void OnDrawGizmos () {
        if (!Application.isPlaying) return;
        // 画圆
        DrawGizmosCircle(circleCenter.position, radius);
        // 画直线
        Gizmos.DrawLine(lineStart.position, lineEnd.position);
        // 画交点
        Gizmos.DrawWireSphere(m_intersection1, 0.2f);
        Gizmos.DrawWireSphere(m_intersection2, 0.2f);
    }
}
