#pragma warning disable 0649

using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class CityBloxxBlock : MonoBehaviour {

    private Transform m_transform;
    private int m_id; // 楼层ID, 0 开始

    public void SetId (int value) {
        m_id = value;
    }

    private void Start () {
        m_transform = transform;
    }


    /*private void FixedUpdate () {
        float radius = 2f;      // 阴影距离parent的半径
        float maxAngle = 10.0f; //2.0f;  // 阴影左右摇摆的最大角度
        float period = 4.0f;    // 一次摇摆需要多少秒
        float phase = Time.time * (2.0f * Mathf.PI) / period;
        float angle = Mathf.Sin(phase) * (Mathf.Deg2Rad * maxAngle * m_id);
        float sinAngle = Mathf.Sin(angle);
        float x =  sinAngle * radius;

        m_transform.localEulerAngles = new Vector3(0, 0, x);
    }*/



    private float m_time; // 实际使用时应该所有方块使用一个统一全局的变量而不是每一个方块都声明一个
    private bool m_isPressScreen;
    private bool m_isSpeedup;

    private void FixedUpdate () {
        if (m_isSpeedup) {
            // 加速旋转
            m_time += Time.deltaTime * 3f;
        } else {
            m_time += Time.deltaTime;
        }

        float radius = 40f;      // 阴影距离parent的半径
        float maxAngle = 10.0f; //2.0f;  // 阴影左右摇摆的最大角度
        float period = 4.0f;    // 一次摇摆需要多少秒
        float phase = m_time * (2.0f * Mathf.PI) / period;
        float angle = Mathf.Sin(phase) * (Mathf.Deg2Rad * maxAngle * m_id);
        float sinAngle = Mathf.Sin(angle);
        float x = sinAngle * radius;

        m_transform.localEulerAngles = new Vector3(0, 0, x);
    }

    private void Update () {
        m_isPressScreen = Input.GetMouseButton(0);
        if (Input.GetMouseButtonDown(0)) {
            float tempTime = m_time % 4f;

            if (tempTime < 2f) { // 位于左侧时
                m_isSpeedup = true; // 测试只在位于左侧时反向并加速旋转
            } else if (tempTime > 2f) { // 位于右侧时
                m_isSpeedup = true; // 测试只在位于右侧时反向并加速旋转
            }

            if (m_isSpeedup) {
                if (m_time % 2f < 1f) { // 从0度向左/右旋转时才反向旋转，否则直接加速旋转
                    m_time += (1f - m_time % 1f) * 2f; // 转向
                }
            }
        }

        if (!m_isPressScreen) {
            m_isSpeedup = false;
        }
    }
}
