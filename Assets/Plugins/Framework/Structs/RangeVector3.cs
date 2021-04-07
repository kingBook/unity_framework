using UnityEngine;

[System.Serializable]
public struct RangeVector3 {
    public Vector3 min;
    public Vector3 max;
    public RangeVector3 (Vector3 minValue, Vector3 maxValue) {
        min = minValue;
        max = maxValue;
    }
}