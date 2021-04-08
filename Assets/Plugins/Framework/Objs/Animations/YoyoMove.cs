using UnityEngine;
using System.Collections;
/// <summary>
/// Yoyo 移动
/// </summary>
public class YoyoMove : MonoBehaviour {

    [Tooltip("运动方向")]
    public Vector3 rotation = new Vector3(0f, 0f, 90f);

    [Tooltip("运动半径"), Range(0, 100000)]
    public float radius = 1;

    [Tooltip("运动速度"), Range(1, 100)]
    public int speed = 5;

    [Range(0, 360), Tooltip("用于三角函数计算的初始欧拉角，调整此值会改变在半径范围内初始的位置")]
    public int eulerAngle;

    public bool isDoLocalPosition;

    private Transform m_transform;
    private Vector3 m_origin;

    private void Awake () {
        m_transform = transform;
        if (isDoLocalPosition) {
            m_origin = m_transform.localPosition;
        } else {
            m_origin = m_transform.position;
        }
    }

    private void FixedUpdate () {
        //m_deg:[0,360]
        eulerAngle = (eulerAngle + speed) % 360;
        //[-1,1]
        float t = Mathf.Sin(eulerAngle * Mathf.Deg2Rad);

        Vector3 direction = Quaternion.Euler(rotation) * Vector3.right;
        float distance = radius * t;

        if (isDoLocalPosition) {
            m_transform.localPosition = m_origin + distance * direction;
        } else {
            m_transform.position = m_origin + distance * direction;
        }
    }
}
