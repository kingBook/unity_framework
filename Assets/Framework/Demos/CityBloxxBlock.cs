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


    private void FixedUpdate () {
        float radius = 2f;      // 阴影距离parent的半径
        float maxAngle = 10.0f; //2.0f;  // 阴影左右摇摆的最大角度
        float period = 4.0f;    // 一次摇摆需要多少秒
        float phase = Time.time * (2.0f * Mathf.PI) / period;
        float angle = Mathf.Sin(phase) * (Mathf.Deg2Rad * maxAngle * m_id);
        float sinAngle = Mathf.Sin(angle);
        float x = sinAngle * radius;

        if (gameObject.name == "CityBloxxBlock 0") {
            if (Input.GetMouseButton(0)) {


                Debug.Log("press");
            }
        }

        m_transform.localEulerAngles = new Vector3(0, 0, x);
    }
}
