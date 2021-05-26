using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestPointDistanceLine : MonoBehaviour {

    public Transform point;
    public Transform lineStart;
    public Transform lineEnd;
    public Transform perpendicular;

    private void Update () {
        Vector3 a = point.position - lineStart.position; 
        Vector3 b = lineEnd.position - lineStart.position;

        float distance = Mathf.Abs(Vector3.Dot(a,b.normalized)); //方法一： 求向量 a 在向量 b 上的投影长度

        Vector3 projectVector = Vector3.Project(a, b); //方法二： 求向量 a 在向量 b 上的投影向量，然后取向量长度
        perpendicular.position = lineStart.position + projectVector;

        Debug.Log($"distance:{distance}, projectVector.magnitude:{projectVector.magnitude}");

    }
}
