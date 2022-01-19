using System.Collections;
using UnityEngine;

public static class PointsUtil {

    /// <summary>
    /// 获取点列表中距离 target 最近的点（XZ平面上）
    /// </summary>
    /// <param name="points"></param>
    /// <param name="target"></param>
    /// <returns></returns>
    public static Vector3 GetClosestPointXZ (Vector3[] points, Vector3 target) {
        Vector3 closestPoint = Vector3.zero;
        float minDistance = float.MaxValue;
        for (int i = 0, length = points.Length; i < length; i++) {
            Vector3 testPoint = points[i];
            testPoint.y = target.y;
            float distance = Vector3.Distance(target, testPoint);
            if (distance < minDistance) {
                minDistance = distance;
                closestPoint = points[i]; // points[i] 不改变 y
            }
        }
        return closestPoint;
    }

    /// <summary>
    /// 获取点列表中距离 target 最近的点（XY平面上）
    /// </summary>
    /// <param name="points"></param>
    /// <param name="target"></param>
    /// <returns></returns>
    public static Vector3 GetClosestPointXY (Vector3[] points, Vector3 target) {
        Vector3 closestPoint = Vector3.zero;
        float minDistance = float.MaxValue;
        for (int i = 0, length = points.Length; i < length; i++) {
            Vector3 testPoint = points[i];
            testPoint.z = target.z;
            float distance = Vector3.Distance(target, testPoint);
            if (distance < minDistance) {
                minDistance = distance;
                closestPoint = points[i]; // points[i] 不改变 z
            }
        }
        return closestPoint;
    }

    /// <summary>
    /// 获取点列表中距离 target 最近的点
    /// </summary>
    /// <param name="points"></param>
    /// <param name="target"></param>
    /// <returns></returns>
    public static Vector3 GetClosestPoint (Vector3[] points, Vector3 target) {
        Vector3 closestPoint = Vector3.zero;
        float minDistance = float.MaxValue;
        for (int i = 0, length = points.Length; i < length; i++) {
            Vector3 testPoint = points[i];
            float distance = Vector3.Distance(target, testPoint);
            if (distance < minDistance) {
                minDistance = distance;
                closestPoint = testPoint;
            }
        }
        return closestPoint;
    }

    /// <summary>
    /// 获取点列表中距离 target 最近的点
    /// </summary>
    /// <param name="points"></param>
    /// <param name="target"></param>
    /// <returns></returns>
    public static Vector2 GetClosestPoint (Vector2[] points, Vector2 target) {
        Vector2 closestPoint = Vector2.zero;
        float minDistance = float.MaxValue;
        for (int i = 0, length = points.Length; i < length; i++) {
            Vector2 testPoint = points[i];
            float distance = Vector2.Distance(target, testPoint);
            if (distance < minDistance) {
                minDistance = distance;
                closestPoint = testPoint;
            }
        }
        return closestPoint;
    }

    public static float GetPointsDistanceXZ (Vector3[] points) {
        float distance = 0f;
        for (int i = 0, length = points.Length; i < length; i++) {
            if (i < length - 1) {
                Vector3 current = points[i];
                Vector3 next = points[i + 1];
                current.y = next.y = 0f;
                distance += Vector3.Distance(current, next);
            }
        }
        return distance;
    }

    public static float GetPointsDistanceXY (Vector3[] points) {
        float distance = 0f;
        for (int i = 0, length = points.Length; i < length; i++) {
            if (i < length - 1) {
                Vector3 current = points[i];
                Vector3 next = points[i + 1];
                current.z = next.z = 0f;
                distance += Vector3.Distance(current, next);
            }
        }
        return distance;
    }

    public static float GetPointsDistance (Vector3[] points) {
        float distance = 0f;
        for (int i = 0, length = points.Length; i < length; i++) {
            if (i < length - 1) {
                Vector3 current = points[i];
                Vector3 next = points[i + 1];
                distance += Vector3.Distance(current, next);
            }
        }
        return distance;
    }

    public static float GetPointsDistance (Vector2[] points) {
        float distance = 0f;
        for (int i = 0, length = points.Length; i < length; i++) {
            if (i < length - 1) {
                Vector2 current = points[i];
                Vector2 next = points[i + 1];
                distance += Vector2.Distance(current, next);
            }
        }
        return distance;
    }
}
