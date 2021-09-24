using UnityEngine;

[System.Serializable]
public struct RangeVector2 {
    public Vector2 min;
    public Vector2 max;
    public RangeVector2 (Vector2 minValue, Vector2 maxValue) {
        min = minValue;
        max = maxValue;
    }
}