// https://gwb.tencent.com/community/detail/110355
using UnityEngine;
using System.Collections;

public class Pendulum : MonoBehaviour {

    public Transform m_anchor; // 圆点
    public float g = 9.8f; // 重力加速度
    private Vector3 m_rotateAxis; // 旋转轴

    private float w = 0; // 角速度(单位：弧度/秒)

    private void Start() {
        // 求出旋转轴
        m_rotateAxis = Vector3.Cross(transform.position - m_anchor.position, Vector3.down);
    }

    private void Update() {
        float r = Vector3.Distance(m_anchor.position, transform.position);
        float l = Vector3.Distance(new Vector3(m_anchor.position.x, transform.position.y, m_anchor.position.z), transform.position);
        // 当钟摆摆动到另外一侧时，l为负，则角加速度alpha为负。
        Vector3 axis = Vector3.Cross(transform.position - m_anchor.position, Vector3.down);
        if (Vector3.Dot(axis, m_rotateAxis) < 0) {
            l = -l;
        }
        float sinAlpha = l / r;
        //求角加速度
        float alpha = (sinAlpha * g) / r;
        //累计角速度(单位：弧度/秒)
        w += alpha * Time.deltaTime;
        //求角位移(乘以 Mathf.Rad2Deg 是为了将弧度转换为角度)
        float thelta = w * Time.deltaTime * Mathf.Rad2Deg;
        //绕圆点m_ahchor的旋转轴m_rotateAxis旋转thelta角度
        transform.RotateAround(m_anchor.position, m_rotateAxis, thelta);
    }
}