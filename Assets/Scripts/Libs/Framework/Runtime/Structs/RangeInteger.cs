using UnityEngine;

[System.Serializable]
public class RangeInteger {

    public int min;
    public int max;

    public RangeInteger(int minValue, int maxValue) {
        min = minValue;
        max = maxValue;
    }

    public float Lerp(float t) {
        return Mathf.Lerp(min, max, t);
    }
}
