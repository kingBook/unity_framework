using Cinemachine;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(CinemachineFreeLook))]
public class CMFreeLookOnlyMouseDown : MonoBehaviour {

    /// <summary>
    /// <para> 此值类似 Edit -> Project Settings -> Input Manager -> Axes -> Mouse X -> Sensitivity </para>
    /// <para> * 因为 Touch.deltaPosition 是基于屏幕坐标计算的，和 Input.GetAxis("Mouse X") 是不一样的，直接使用 Touch.deltaPosition 会出现 PC 平台与移动平台不兼容 </para>
    /// <para> * 移动平台如果 使用 Input.GetAxis("Mouse X") ,会出现每次触摸开始时都会出现很大的值，即使未滑动</para>
    /// </summary>
    [SerializeField, Tooltip("触摸的敏感度（仅用于移动平台）")] private float m_touchSensitivity = 0.01f;
    [SerializeField] private bool m_ignorePointerOverUI = true;


    private CinemachineFreeLook m_cinemachineFreeLook;

    private void Awake() {
        m_cinemachineFreeLook = GetComponent<CinemachineFreeLook>();

        m_cinemachineFreeLook.m_XAxis.m_InputAxisName = "";
        m_cinemachineFreeLook.m_YAxis.m_InputAxisName = "";
    }

    private void Update() {
        if (Input.GetMouseButton(0)) {
            if (!Input.touchSupported) {
                // PC
                bool isPointerOverUI = EventSystem.current && EventSystem.current.IsPointerOverGameObject();
                if (m_ignorePointerOverUI && isPointerOverUI) {
                    // 触摸 UI时，忽略
                } else {
                    m_cinemachineFreeLook.m_XAxis.m_InputAxisValue = Input.GetAxis("Mouse X");
                    m_cinemachineFreeLook.m_YAxis.m_InputAxisValue = Input.GetAxis("Mouse Y");
                }
            } else {
                // 移动平台
                if (Input.touchCount > 0) {
                    Touch touch = Input.GetTouch(0);
                    bool isPointerOverUI = EventSystem.current && EventSystem.current.IsPointerOverGameObject(touch.fingerId);
                    if (m_ignorePointerOverUI && isPointerOverUI) {
                        // 触摸 UI时，忽略
                    } else {
                        m_cinemachineFreeLook.m_XAxis.m_InputAxisValue = touch.deltaPosition.x * m_touchSensitivity;
                        m_cinemachineFreeLook.m_YAxis.m_InputAxisValue = touch.deltaPosition.y * m_touchSensitivity;
                    }
                }
            }
        } else {
            m_cinemachineFreeLook.m_XAxis.m_InputAxisValue = 0f;
            m_cinemachineFreeLook.m_YAxis.m_InputAxisValue = 0f;
        }
    }

}
