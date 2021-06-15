using UnityEngine;

[System.Serializable]
public struct RangeFloat {
    public float min;
    public float max;
    public RangeFloat (float minValue, float maxValue) {
        min = minValue;
        max = maxValue;
    }
}