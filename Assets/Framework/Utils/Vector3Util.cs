using System.Collections;
using UnityEngine;

public static class Vector3Util {

    /// <summary> 计算两个向量在 XZ 平面上的距离 </summary>
    public static float DistanceXZ(Vector3 a,Vector3 b) {
        a.y = b.y = 0f;
        return Vector3.Distance(a,b);
    }

    /// <summary> 计算两个向量在 XY 平面上的距离 </summary>
    public static float DistanceXY (Vector3 a, Vector3 b) {
        a.z = b.z = 0f;
        return Vector3.Distance(a, b);
    }
}
