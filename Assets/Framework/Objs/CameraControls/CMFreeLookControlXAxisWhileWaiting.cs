using Cinemachine;
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(CinemachineFreeLook))]
public class CMFreeLookControlXAxisWhileWaiting : MonoBehaviour {

    [SerializeField, Tooltip("旋转的速度")] private float m_speed = 0.01f;
    [SerializeField, Tooltip("默认的旋转方向")] private int m_defaultSign = 1;
    [SerializeField, Tooltip("当 InputAxisValue 为 0 时，超过这个时间则开始旋转")] private float m_waitTime = 1f;

    private CinemachineFreeLook m_cinemachineFreeLook;

    private int m_lastInputSign;
    private float m_elapsedTime;

    private void Awake () {
        m_cinemachineFreeLook = GetComponent<CinemachineFreeLook>();
        m_lastInputSign = m_defaultSign;
    }

    private void Update () {
        if (m_cinemachineFreeLook.m_XAxis.m_InputAxisValue != 0f) {
            m_lastInputSign = (int)Mathf.Sign(m_cinemachineFreeLook.m_XAxis.m_InputAxisValue);
            if (m_lastInputSign == 0) {
                m_lastInputSign = m_defaultSign;
            }
            m_elapsedTime = 0f;
        } else {
            m_elapsedTime += Time.deltaTime;
            if (m_elapsedTime >= m_waitTime) {
                m_cinemachineFreeLook.m_XAxis.m_InputAxisValue = m_speed * m_lastInputSign;
            }
        }
    }
}
