using System.Collections;
using UnityEngine;

public static class GeomUtil {

    /// <summary>
    /// 计算线段与圆的交点
    /// </summary>
    /// <param name="lineStart"> 线段的起始点 </param>
    /// <param name="lineEnd"> 线段的终点 </param>
    /// <param name="circleCenter"> 圆心 </param>
    /// <param name="radius"> 半径 </param>
    /// <param name="intersection1"> 输出的交点1 </param>
    /// <param name="intersection2"> 输出的交点2 </param>
    /// <returns> 返回交点的数量，0：不相交；1：有1个交点；2：有2个交点 </returns>
    public static int LineSegmentIntersectCircle (Vector2 lineStart, Vector2 lineEnd, Vector2 circleCenter, float radius, out Vector2 intersection1, out Vector2 intersection2) {
        int intersectCount = 0;
        bool intersect = Ray2DIntersectCircle(lineStart, lineEnd - lineStart, circleCenter, radius, out intersection1, out intersection2);
        if (intersect) {
            float lineSegmentLength = Vector2.Distance(lineStart, lineEnd);
            float d1 = Vector2.Distance(lineStart, intersection1 - lineStart);
            float d2 = Vector2.Distance(lineStart, intersection2 - lineStart);
            if (d1 <= lineSegmentLength) {
                intersectCount++;
            }
            if (d2 <= lineSegmentLength) {
                intersectCount++;
            }
        }
        return intersectCount;
    }

    /// <summary>
    /// 计算射线与圆的交点
    /// </summary>
    /// <param name="ray"> 2D射线 </param>
    /// <param name="circleCenter"> 圆心 </param>
    /// <param name="radius"> 半径 </param>
    /// <param name="intersection1"> 输出的交点1 </param>
    /// <param name="intersection2"> 输出的交点2 </param>
    /// <returns> 是否相交 </returns>
    public static bool Ray2DIntersectCircle (Ray2D ray, Vector2 circleCenter, float radius, out Vector2 intersection1, out Vector2 intersection2) {
        intersection1 = Vector2.zero;
        intersection2 = Vector2.zero;

        Vector2 e = circleCenter - ray.origin;
        float a = Vector2.Dot(ray.direction, e);
        float fSqr = radius * radius - e.sqrMagnitude + a * a;
        if (fSqr >= 0) {
            float f = Mathf.Sqrt(fSqr);
            float t = a - f;
            if (t >= 0) {
                intersection1 = ray.origin + ray.direction * t;
                intersection2 = ray.origin + ray.direction * (t + 2f * f);
                return true;
            }
        }
        return false;
    }

    /// <summary>
    /// 计算射线与圆的交点
    /// </summary>
    /// <param name="origin"> 2D射线的原点 </param>
    /// <param name="direction"> 2D射线的方向 </param>
    /// <param name="circleCenter"> 圆心 </param>
    /// <param name="radius"> 半径 </param>
    /// <param name="intersection1"> 输出的交点1 </param>
    /// <param name="intersection2"> 输出的交点2 </param>
    /// <returns> 是否相交 </returns>
    public static bool Ray2DIntersectCircle (Vector2 origin, Vector2 direction, Vector2 circleCenter, float radius, out Vector2 intersection1, out Vector2 intersection2) {
        Ray2D ray = new Ray2D(origin, direction);
        return Ray2DIntersectCircle(ray, circleCenter, radius, out intersection1, out intersection2);
    }
}
