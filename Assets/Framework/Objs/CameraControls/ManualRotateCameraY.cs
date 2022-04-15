using System;
using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// 手指在屏幕上水平移动旋转相机的Y轴
/// </summary>
public class ManualRotateCameraY : MonoBehaviour {

    [System.Serializable]
    public enum ActiveArea { FullScreen, RightScreen, LeftScreen }

    [Tooltip("相机看向的目标")]
    public Transform targetTransform;

    [System.Serializable]
    public class AdvancedOptions {
        [Tooltip("跟随相机旋转的目标（如果isApplyToDriftCamera==true,会改变DriftCamera.originPositionNormalized，此目标旋转会受到影响）")]
        public Transform targetTransformFollowRotation;
        [Tooltip("是否应用到DriftCamera组件(存在时有效,将会旋转DriftCamera.originPositionNormalized)")]
        public bool isApplyToDriftCamera = true;
        [Tooltip("活动区域(在活动区域内划屏才能旋转相机)")]
        public ActiveArea activeArea = ActiveArea.FullScreen;
    }
    public AdvancedOptions advancedOptions;

    /// <summary>旋转前 void(float h)</summary>
    public event Action<float> onPreRotateEvent;
    /// <summary>旋转 void(float h)</summary>
    public event Action<float> onRotateEvent;

    private Camera m_camera;
    private CameraFollow m_cameraFollow;
    private bool m_isRotateBegin;
    /// <summary>鼠标左键，在按下开始时是否接触UI</summary>
    private bool m_isMouseOverUIOnBegan;
    private int m_touchFingerId = -1;

    private void Start() {
        m_camera = GetComponent<Camera>();
        m_cameraFollow = GetComponent<CameraFollow>();
    }

    private void Update() {
        if (Input.touchSupported) {
            TouchHandler();
        } else {
            MouseHandler();
        }
    }

    private void TouchHandler() {
        Touch touch;
        //返回一个在触摸开始阶段时非触摸UI的触摸点
        if (m_touchFingerId > -1) {
            touch = InputUtil.GetTouchWithFingerId(m_touchFingerId, false, TouchPhase.Moved, TouchPhase.Stationary);
            m_touchFingerId = touch.fingerId;
        } else {
            touch = InputUtil.GetFirstTouch(TouchPhase.Began, true);
            m_touchFingerId = touch.fingerId;
        }

        if (touch.fingerId > -1) {
            if (GetPositionOnActiveArea(touch.position, advancedOptions.activeArea)) {
                float h = touch.deltaPosition.x * 0.5f;
                if (!m_isRotateBegin) {
                    m_isRotateBegin = true;
                    onPreRotateEvent?.Invoke(h);
                }
                Rotate(h);
            }
        }
    }

    private void MouseHandler() {
        //按下鼠标左键时，鼠标是否接触UI
        if (Input.GetMouseButtonDown(0)) {
            m_isMouseOverUIOnBegan = EventSystem.current.IsPointerOverGameObject();
        }

        //非移动设备按下鼠标左键旋转
        if (Input.GetMouseButton(0)) {
            //鼠标按下左键时没有接触UI
            if (!m_isMouseOverUIOnBegan) {
                if (GetPositionOnActiveArea(Input.mousePosition, advancedOptions.activeArea)) {
                    float h = Input.GetAxis("Mouse X");
                    h *= 10;

                    if (!m_isRotateBegin) {
                        m_isRotateBegin = true;
                        onPreRotateEvent?.Invoke(h);
                    }
                    Rotate(h);
                }
            }
        } else {
            m_isRotateBegin = false;
        }
    }

    /// <summary>
    /// 旋转
    /// </summary>
    private void Rotate(float h) {
        //应用到DriftCamera
        if (advancedOptions.isApplyToDriftCamera) {
            if (m_cameraFollow != null) {
                Quaternion rotation = Quaternion.AngleAxis(h, Vector3.up);
                m_cameraFollow.originPositionNormalized = rotation * m_cameraFollow.originPositionNormalized;
            }
        }
        //绕着pivot旋转Y轴，实现左右旋转
        m_camera.transform.RotateAround(targetTransform.position, Vector3.up, h);
        //跟随相机旋转的目标Transform
        if (advancedOptions.targetTransformFollowRotation != null) {
            advancedOptions.targetTransformFollowRotation.Rotate(0, h, 0);
        }
        //
        onRotateEvent?.Invoke(h);
    }

    /// <summary>返回指定的位置是否在活动区域</summary>
    private bool GetPositionOnActiveArea(Vector2 position, ActiveArea activeArea) {
        bool result = true;
        if (activeArea == ActiveArea.RightScreen) {
            result = position.x > Screen.width * 0.5f;
        } else if (activeArea == ActiveArea.LeftScreen) {
            result = position.x < Screen.width * 0.5f;
        }
        return result;
    }

    private void OnDestroy() {

    }

}