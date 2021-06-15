using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 测试从平面上方向看向平面的点
/// </summary>
public class TestLookPlanePoint : MonoBehaviour {

    public Transform planeTransform;
    public Transform ball;

    private void Start () {


    }

    private void Update () {
        var plane = new Plane(planeTransform.up, 0);
        var pos = ball.position;
        var rotation = Quaternion.FromToRotation(plane.normal, Vector3.back);
        Vector3 pos2 = rotation * pos;
        Debug.Log($"{pos} {pos2}");
    }

}
