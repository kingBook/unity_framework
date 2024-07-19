using System.Collections;
using UnityEngine;

public static class Vector3Util {

    /// <summary> 计算两个向量在 XZ 平面上的距离 </summary>
    public static float DistanceXZ(Vector3 a, Vector3 b) {
        a.y = b.y = 0f;
        return Vector3.Distance(a, b);
    }

    /// <summary> 计算两个向量在 XY 平面上的距离 </summary>
    public static float DistanceXY(Vector3 a, Vector3 b) {
        a.z = b.z = 0f;
        return Vector3.Distance(a, b);
    }

    public static float InverseLerp(Vector3 a, Vector3 b, Vector3 value) {
        if (a != b) {
            return Mathf.Clamp01(Vector3.Distance(a, value) / Vector3.Distance(a, b));
        }
        return 0f;
    }
}
