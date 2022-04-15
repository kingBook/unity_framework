using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestBox : MonoBehaviour {

    public BoxCollider boxCollider;
    public BoxCollider viewBoxCollider;

    void Start() {

    }

    void Update() {
        Vector3 center = boxCollider.transform.TransformPoint(boxCollider.center);
        Vector3 size = boxCollider.size;
        size.Scale(boxCollider.transform.lossyScale);
        Quaternion orientation = boxCollider.transform.rotation;
        viewBoxCollider.size = size;
        viewBoxCollider.transform.position = center;
        viewBoxCollider.transform.rotation = orientation;
    }
}
