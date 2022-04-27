using System.Collections;
using UnityEngine;

public static class Vector2Util {

    public static float InverseLerp(Vector2 a, Vector2 b, Vector2 value) {
        if (a != b) {
            return Mathf.Clamp01(Vector2.Distance(a, value) / Vector2.Distance(a, b));
        }
        return 0f;
    }
}
