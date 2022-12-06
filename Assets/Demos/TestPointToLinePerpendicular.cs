using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestPointToLinePerpendicular : MonoBehaviour {

    public Transform lineStart;
    public Transform lineEnd;
    public Transform target;
    public Transform perpendicular;

    void Start() {

    }

    void Update() {
        // 1. 垂足会超出线段
        perpendicular.position = GeomUtil.GetPerpendicularOnLine(target.position, lineStart.position, lineEnd.position, out float t);
        // 2. 垂足不会超出线段
        //perpendicular.position = GeomUtil.ProjectPointLine(target.position, lineStart.position, lineEnd.position, out float t);

        Debug.Log($"t:{t}");

        Debug.DrawLine(lineStart.position, lineEnd.position);
        Debug.DrawLine(target.position, perpendicular.position);
    }
}
