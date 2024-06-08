// https://www.cnblogs.com/kingBook/p/17123253.html
using System;
using UnityEngine;
using System.Collections;
using UnityEngine.Serialization;


public class Pendulum : MonoBehaviour {

    public Transform anchor; // 圆点
    public float g = 9.8f; // 重力加速度

    private float m_w = 0; // 角速度(单位：弧度/秒)
    private float m_l; // 摆长
    private Vector3 m_rotateAxis; // 旋转轴

    private void Start() {
        // 求出旋转轴
        m_rotateAxis = Vector3.Cross(transform.position - anchor.position, Vector3.down);
        // 求摆长
        m_l = Vector3.Distance(anchor.position, transform.position);
    }

    private void Update() {
        float d = Vector3.Distance(new Vector3(anchor.position.x, transform.position.y, anchor.position.z), transform.position);
        // 当钟摆摆动到另外一侧时，d为负，则角加速度a为负。
        Vector3 axis = Vector3.Cross(transform.position - anchor.position, Vector3.down);
        if (Vector3.Dot(axis, m_rotateAxis) < 0) {
            d = -d;
        }
        float sinAlpha = d / m_l;
        // 求角加速度
        float a = (sinAlpha * g) / m_l;
        // 累计角速度(单位：弧度/秒)
        m_w += a * Time.deltaTime;
        // 求角位移(乘以 Mathf.Rad2Deg 是为了将弧度转换为角度)
        float thelta = m_w * Time.deltaTime * Mathf.Rad2Deg;
        // 绕圆点ahchor的旋转轴m_rotateAxis旋转thelta角度
        transform.RotateAround(anchor.position, m_rotateAxis, thelta);
    }
}