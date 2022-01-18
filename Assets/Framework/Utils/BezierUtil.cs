using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class BezierUtil {

    public static void Bezier1 (Vector3 p0, Vector3 p1, float t, out Vector3 output) {
        output = p0 + (p1 - p0) * t;
    }

    public static Vector3[] GetBezierPoints1 (Vector3 p0, Vector3 p1, int segmentNum, bool inclusiveStartEnd) {
        List<Vector3> points = new List<Vector3>();
        for (int i = 1; i <= segmentNum; i++) {
            float t = i / (float)segmentNum;
            Bezier1(p0, p1, t, out Vector3 point);
            points.Add(point);
        }
        if (inclusiveStartEnd) {
            points.Insert(0, p0);
            points.Add(p1);
        }
        return points.ToArray();
    }

    public static void Bezier2 (Vector3 p0, Vector3 p1, Vector3 p2, float t, out Vector3 output) {
        output = (1f - t) * (1f - t) * p0 + 2f * t * (1f - t) * p1 + t * t * p2;
    }

    public static Vector3[] GetBezierPoints2 (Vector3 p0, Vector3 p1, Vector3 p2, int segmentNum, bool inclusiveStartEnd) {
        List<Vector3> points = new List<Vector3>();
        for (int i = 1; i <= segmentNum; i++) {
            float t = i / (float)segmentNum;
            Bezier2(p0, p1, p2, t, out Vector3 point);
            points.Add(point);
        }
        if (inclusiveStartEnd) {
            points.Insert(0, p0);
            points.Add(p2);
        }
        return points.ToArray();
    }

    public static void Bezier3 (Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, float t, out Vector3 output) {
        Vector3 a = (1f - t) * (1f - t) * (1f - t) * p0;
        Vector3 b = 3f * t * (1f - t) * (1f - t) * p1;
        Vector3 c = 3f * t * t * (1f - t) * p2;
        Vector3 d = t * t * t * p3;
        output = a + b + c + d;
    }

    public static Vector3[] GetBezierPoints3 (Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, int segmentNum, bool inclusiveStartEnd) {
        List<Vector3> points = new List<Vector3>();
        for (int i = 1; i <= segmentNum; i++) {
            float t = i / (float)segmentNum;
            Bezier3(p0, p1, p2, p3, t, out Vector3 point);
            points.Add(point);
        }
        if (inclusiveStartEnd) {
            points.Insert(0, p0);
            points.Add(p3);
        }
        return points.ToArray();
    }
}