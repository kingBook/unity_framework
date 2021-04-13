
using UnityEngine;
/// <summary>
/// 数学类
/// </summary>
public class Mathk {

    /// <summary>
    /// 将任意角度转换为[-180°,180°]，并返回转换后的角度
    /// </summary>
    /// <param name="rotation">需要转换的角度</param>
    /// <returns></returns>
    public static float GetRotationTo180 (float rotation) {
        rotation %= 360.0f;
        if (rotation > 180.0f) rotation -= 360.0f;
        else if (rotation < -180.0f) rotation += 360.0f;
        return rotation;
    }

    /// <summary>
    /// 计算出目标角减当前角的差（取到达目标角度的最近旋转方向）,并返回这个差
    /// </summary>
    /// <param name="rotation">当前角度</param>
    /// <param name="targetRotation">目标角度</param>
    /// <returns></returns>
    public static float GetRotationDifference (float rotation, float targetRotation) {
        rotation = GetRotationTo360(rotation);
        targetRotation = GetRotationTo360(targetRotation);
        float offset = targetRotation - rotation;
        if (Mathf.Abs(offset) > 180.0f) {
            float reDir = offset >= 0 ? -1 : 1;
            offset = reDir * (360.0f - Mathf.Abs(offset));
        }
        return offset;
    }

    /// <summary>
    /// 将任意角度转换为[0°,360°]的值,并返回转换后的值（此函数的与 Mathf.Repeat(rotation,360f) 效果一样）
    /// </summary>
    /// <param name="rotation">需要转换的角度</param>
    /// <returns></returns>
    public static float GetRotationTo360 (float rotation) {
        rotation = GetRotationTo180(rotation);
        if (rotation < 0) rotation += 360f;
        return rotation;
    }

    /// <summary> 获取点在线段的垂足（垂足会超出线段） </summary>
    public static Vector3 GetPerpendicularPoint (Vector3 point, Vector3 lineStart, Vector3 lineEnd) {
        Vector3 rhs = point - lineStart;
        Vector3 vector = lineEnd - lineStart;
        float magnitude = vector.magnitude;
        Vector3 vector2 = vector;
        if (magnitude > 1E-06f) {
            vector2 /= magnitude;
        }
        float value = Vector3.Dot(vector2, rhs);
        return lineStart + vector2 * value;
    }

    /// <summary> 点到线段的最小距离点（垂足不超出线段） </summary>
    public static Vector3 ProjectPointLine (Vector3 point, Vector3 lineStart, Vector3 lineEnd) {
        Vector3 rhs = point - lineStart;
        Vector3 vector = lineEnd - lineStart;
        float magnitude = vector.magnitude;
        Vector3 vector2 = vector;
        if (magnitude > 1E-06f) {
            vector2 /= magnitude;
        }
        float value = Vector3.Dot(vector2, rhs);
        value = Mathf.Clamp(value, 0f, magnitude);
        return lineStart + vector2 * value;
    }

    /// <summary> 点到线段的最小距离（垂足不超出线段） </summary>
    public static float DistancePointLine (Vector3 point, Vector3 lineStart, Vector3 lineEnd) {
        return Vector3.Magnitude(ProjectPointLine(point, lineStart, lineEnd) - point);
    }

    /// <summary>
    /// 获取点距离顶点列表的最近的线段的索引（顺时针方向查找），当顶点列表长度小于2时返回(-1,-1)
    /// </summary>
    /// <param name="point">点</param>
    /// <param name="vertices">顶点列表（顺时针方向）</param>
    /// <param name="isClosed">顶点列表是否闭合</param>
    /// <returns></returns>
    public static (int startIndex, int endIndex) GetClosestPolyLineToPoint (Vector3 point, Vector3[] vertices, bool isClosed) {
        var result = (-1, -1);
        float minDistance = float.MaxValue;
        for (int i = 0, len = vertices.Length; i < len; i++) {
            int lineStartIndex = i;
            int lineEndIndex = (i >= len - 1 && isClosed) ? 0 : i + 1;
            if (lineEndIndex >= len) break;
            Vector3 lineStart = vertices[lineStartIndex];
            Vector3 lineEnd = vertices[lineEndIndex];

            float distance = DistancePointLine(point, lineStart, lineEnd);
            if (distance < minDistance) {
                minDistance = distance;
                result = (lineStartIndex, lineEndIndex);
            }
        }
        return result;
    }

    /// <summary>
    /// 获取射线发射方向与顶点列表相交的所有线段中，射线原点与交点距离最近的线段索引顺时针方向查找），当顶点列表长度小于2时返回(-1,-1)。
    /// 注意：使用此方法必须保证顶点列表中的所有顶点与及射线的原点和方向都在同一个平面上
    /// </summary>
    /// <param name="rayOrigin">射线的原点</param>
    /// <param name="rayDirection">射线的方向</param>
    /// <param name="vertices">顶点列表（顺时针方向）</param>
    /// <param name="isClosed">顶点列表是否闭合</param>
    /// <returns></returns>
    public static (int startIndex, int endIndex) GetClosestPolyLineToRayOrigin (Vector3 rayOrigin, Vector3 rayDirection, Vector3[] vertices, bool isClosed) {
        Vector3 rayEnd = rayOrigin + rayDirection.normalized * 10;//随意定一个射线的结束点
        var result = (-1, -1);
        float minDistance = float.MaxValue;
        for (int i = 0, len = vertices.Length; i < len; i++) {
            int lineStartIndex = i;
            int lineEndIndex = (i >= len - 1 && isClosed) ? 0 : i + 1;
            if (lineEndIndex >= len) break;
            Vector3 lineStart = vertices[lineStartIndex];
            Vector3 lineEnd = vertices[lineEndIndex];
            Vector3 lineDirection = lineEnd - lineStart;
            if (GetTwoLineIntersection(rayOrigin, rayDirection, lineStart, lineDirection, out Vector3 intersection)) {
                if (PointOnWhichSideOfLineSegment(intersection, lineStart, lineEnd) == 0) {
                    if (PointOnWhichSideOfLineSegment(intersection, rayOrigin, rayEnd) != 1) {
                        float distance = Vector3.Distance(rayOrigin, intersection);
                        if (distance < minDistance) {
                            minDistance = distance;
                            result = (lineStartIndex, lineEndIndex);
                        }
                    }
                }
            }
        }
        return result;
    }

    /// <summary>
    /// 获取两条直线的交点。如果直线相交，则返回true，否则返回false。
    /// 注意：使用此方法必须两直线都在同一个平面上，交点会超出线段范围
    /// </summary>
    public static bool GetTwoLineIntersection (Vector3 lineStart1, Vector3 lineEnd1, Vector3 lineStart2, Vector3 lineEnd2, out Vector3 intersection) {
        Vector3 lineDirection1 = lineStart1 - lineEnd1;
        Vector3 lineDirection2 = lineStart2 - lineEnd2;

        Vector3 lineVec3 = lineStart2 - lineStart1;
        Vector3 crossVec1and2 = Vector3.Cross(lineDirection1, lineDirection2);
        Vector3 crossVec3and2 = Vector3.Cross(lineVec3, lineDirection2);

        float planarFactor = Vector3.Dot(lineVec3, crossVec1and2);
        //在同一个平面，且不平行
        if (Mathf.Abs(planarFactor) < 0.0001f && crossVec1and2.sqrMagnitude > 0.0001f) {
            float s = Vector3.Dot(crossVec3and2, crossVec1and2) / crossVec1and2.sqrMagnitude;
            intersection = lineStart1 + (lineDirection1 * s);
            return true;
        }
        intersection = Vector3.zero;
        return false;
    }

    /// <summary>
    /// 获取两条线段的交点。如果线段相交，则返回true，否则返回false。
    /// 注意：使用此方法必须两线段都在同一个平面上，交点不会超出线段范围
    /// </summary>
    public static bool GetTwoLineSegmentsIntersection (Vector3 lineStart1, Vector3 lineEnd1, Vector3 lineStart2, Vector3 lineEnd2, out Vector3 intersection) {
        Vector3 lineDirection1 = lineStart1 - lineEnd1;
        Vector3 lineDirection2 = lineStart2 - lineEnd2;

        Vector3 lineVec3 = lineStart2 - lineStart1;
        Vector3 crossVec1and2 = Vector3.Cross(lineDirection1, lineDirection2);
        Vector3 crossVec3and2 = Vector3.Cross(lineVec3, lineDirection2);

        float planarFactor = Vector3.Dot(lineVec3, crossVec1and2);
        //在同一个平面，且不平行
        if (Mathf.Abs(planarFactor) < 0.0001f && crossVec1and2.sqrMagnitude > 0.0001f) {
            float s = Vector3.Dot(crossVec3and2, crossVec1and2) / crossVec1and2.sqrMagnitude;
            bool isInLineSegment1 = s >= -1 && s <= 0;
            if (isInLineSegment1) {
                intersection = lineStart1 + (lineDirection1 * s);

                Vector3 lineSegment2Min = Vector3.Min(lineStart2, lineEnd2);
                Vector3 lineSegment2Max = Vector3.Max(lineStart2, lineEnd2);
                bool isInLineSegment2 = intersection.x >= lineSegment2Min.x &&
                                        intersection.y >= lineSegment2Min.y &&
                                        intersection.z >= lineSegment2Min.z &&
                                        intersection.x <= lineSegment2Max.x &&
                                        intersection.y <= lineSegment2Max.y &&
                                        intersection.z <= lineSegment2Max.z;
                if (isInLineSegment2) {
                    return true;
                }
            }
        }
        intersection = Vector3.zero;
        return false;
    }

    /// <summary>
    /// 输出3D空间中不平行的两条直线距离彼此最近的两个点，如果两条直线不平行则返回True
    /// <param name="lineStart1"></param> 
    /// <param name="lineEnd1"></param>  
    /// <param name="lineStart2"></param>
    /// <param name="lineEnd2"></param> 
    /// <param name="closestPointLine1"></param>
    /// <param name="closestPointLine2"></param>
    /// <returns></returns>
    /// </summary>
    public static bool GetClosestPointsOnTwo3DLines (Vector3 lineStart1, Vector3 lineEnd1, Vector3 lineStart2, Vector3 lineEnd2, out Vector3 closestPointLine1, out Vector3 closestPointLine2) {
        closestPointLine1 = Vector3.zero;
        closestPointLine2 = Vector3.zero;

        Vector3 lineDirection1 = lineStart1 - lineEnd1;
        Vector3 lineDirection2 = lineStart2 - lineEnd2;

        float a = Vector3.Dot(lineDirection1, lineDirection1);
        float b = Vector3.Dot(lineDirection1, lineDirection2);
        float e = Vector3.Dot(lineDirection2, lineDirection2);

        float d = a * e - b * b;

        //线段不平行
        if (d != 0.0f) {
            Vector3 r = lineStart1 - lineStart2;
            float c = Vector3.Dot(lineDirection1, r);
            float f = Vector3.Dot(lineDirection2, r);

            float s = (b * f - c * e) / d;
            float t = (a * f - c * b) / d;

            closestPointLine1 = lineStart1 + lineDirection1 * s;
            closestPointLine2 = lineStart2 + lineDirection2 * t;
            return true;
        }
        return false;
    }

    // <summary>
    /// 输出3D空间中不平行的两条线段彼此最近的两个点，如果两条线段不平行且有交点则返回True
    /// <param name="lineStart1"></param> 
    /// <param name="lineEnd1"></param> 
    /// <param name="lineStart2"></param>
    /// <param name="lineEnd2"></param> 
    /// <param name="closestPointLine1"></param>
    /// <param name="closestPointLine2"></param>
    /// <returns></returns>
    /// </summary>
    public static bool GetClosestPointsOnTwo3DLineSegments (Vector3 lineStart1, Vector3 lineEnd1, Vector3 lineStart2, Vector3 lineEnd2, out Vector3 closestPointLine1, out Vector3 closestPointLine2) {
        closestPointLine1 = Vector3.zero;
        closestPointLine2 = Vector3.zero;

        Vector3 lineDirection1 = lineStart1 - lineEnd1;
        Vector3 lineDirection2 = lineStart2 - lineEnd2;


        float a = Vector3.Dot(lineDirection1, lineDirection1);
        float b = Vector3.Dot(lineDirection1, lineDirection2);
        float e = Vector3.Dot(lineDirection2, lineDirection2);

        float d = a * e - b * b;

        //线段不平行
        if (d != 0.0f) {
            Vector3 r = lineStart1 - lineStart2;
            float c = Vector3.Dot(lineDirection1, r);
            float f = Vector3.Dot(lineDirection2, r);

            float s = (b * f - c * e) / d;
            float t = (a * f - c * b) / d;

            if (s >= -1f && s <= 0f && t >= -1f && t <= 0f) {
                closestPointLine1 = lineStart1 + lineDirection1 * s;
                closestPointLine2 = lineStart2 + lineDirection2 * t;
                return true;
            }
        }
        return false;
    }

    /// <summary>
    /// 此函数用于找出点位于线段的哪一侧。
    /// 假设该点位于由 linePoint1 和 linePoint2 创建的直线上。如果这个点不在线段，
    /// 首先使用 ProjectPointLine() 将其投影到线上。
    /// 如果点在线段上，则返回0。
    /// 如果点在线段之外且位于 linePoint1 的一侧，则返回1。
    /// 如果点在线段之外且位于 linePoint2 的一侧，则返回2。
    /// </summary>
    public static int PointOnWhichSideOfLineSegment (Vector3 point, Vector3 linePoint1, Vector3 linePoint2) {
        Vector3 lineVec = linePoint2 - linePoint1;
        Vector3 pointVec = point - linePoint1;

        float dot = Vector3.Dot(pointVec, lineVec);

        //与linePoint1相比，点位于linePoint2的侧面
        if (dot > 0) {
            if (pointVec.magnitude <= lineVec.magnitude) {
                //点在线段上
                return 0;
            } else {
                //点不在线段上，它在linePoint2的侧面
                return 2;
            }
        } else {
            //与linePoint1相比，点不在linePoint2的一侧。
            //点不在线段上，它在linePoint1的一侧。
            return 1;
        }
    }
}
