using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestTwoLineIntersection : MonoBehaviour {

    public Transform lineStart1;
    public Transform lineEnd1;
    [Space]
    public Transform lineStart2;
    public Transform lineEnd2;
    [Space]
    public Transform intersection;

    void Start() {

    }

    void Update() {
        //bool intersect = GeomUtil.GetTwoLineIntersection(lineStart1.position,lineEnd1.position, lineStart2.position, lineEnd2.position, out Vector3 intersectionPoint, out float t1, out float t2);
        bool intersect = GeomUtil.GetTwoLineSegmentsIntersection(lineStart1.position, lineEnd1.position, lineStart2.position, lineEnd2.position, out Vector3 intersectionPoint, out float t1, out float t2);
        if (intersect) {
            intersection.position = intersectionPoint;
            Debug.Log($"t1:{t1}, t2:{t2}");
        } else {
            intersection.position = Vector3.zero;
        }
    }
}
