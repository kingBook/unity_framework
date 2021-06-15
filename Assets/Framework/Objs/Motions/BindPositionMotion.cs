using System.Collections;
using UnityEngine;
/// <summary>
/// 将当前位置绑定到目标 Transform
/// </summary>
public class BindPositionMotion : MonoBehaviour {

    [Tooltip("绑定的目标")] public Transform target;
    [Tooltip("当前与目标的相对偏移量")] public Vector3 offset;

    private void Start () {
        SyncPosition();
    }

    private void FixedUpdate () {
        SyncPosition();
    }

    private void SyncPosition () {
        Vector3 targetPosition = target.position;
        targetPosition += offset;
        transform.position = targetPosition;
    }

}
