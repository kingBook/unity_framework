using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// 图片查看器操作输入（拖动、放大、缩小）
/// </summary>
public class PictureViewerInput : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler{

    /// <summary> 移动事件，回调格式：<code> void OnMove (Vector2 velocity) </code> </summary>
    public event System.Action<Vector2> onMoveEvent;

    /// <summary> 初始缩放事件(在 <see cref="PictureViewerInput"/> 的 Update 函数第一次执行时发出此事件)，回调格式：<code> void OnInitScale (float scaleValue) </code> </summary>
    public event System.Action<float> onInitScaleEvent;

    /// <summary> 缩放事件，回调格式：<code> void ScaleAroundPoint (float scaleValue, Vector2 aroundScreenPoint) </code> </summary>
    public event System.Action<float, Vector2> onScaleAroundPointEvent;

    [SerializeField] private RangeFloat m_scaleRange = new RangeFloat(1f, 5f);
    [SerializeField] private float m_scaleValue = 1f;


    private int m_firstFingerId = -1;
    private int m_secondFingerId = -1;
    private Vector2? m_firstPos;
    private Vector2? m_secondPos;
    private float m_doubleTouchBeganDistance;
    private float m_scaleOnDoubleTouchBegan;
    private Vector2? m_lastMousePos;
    private bool m_isPointerEnter;
    private bool m_isInited;

    public float scaleValue => m_scaleValue;
    public RangeFloat scaleRange => m_scaleRange;


    void IPointerEnterHandler.OnPointerEnter (PointerEventData eventData) {
        m_isPointerEnter = true;
    }

    void IPointerExitHandler.OnPointerExit (PointerEventData eventData) {
        m_isPointerEnter = false;
    }

    private void DisposeScale () {
        if (m_firstFingerId > -1 && m_secondFingerId > -1) {
            // 退出缩放时也 DisposeMove，因为在缩放时也移动，否则由双指切换为单指会出现位移
            DisposeMove();
        }
        m_firstFingerId = -1;
        m_secondFingerId = -1;
        m_firstPos = null;
        m_secondPos = null;
        m_doubleTouchBeganDistance = 0f;
    }

    private void DisposeMove () {
        m_lastMousePos = null;
    }

    private void DispatchMoveEvent () {
        Vector2 mousePos = Input.mousePosition;
        if (m_lastMousePos == null) {
            m_lastMousePos = mousePos;
        }
        Vector2 velocity = mousePos - m_lastMousePos.Value;
        onMoveEvent?.Invoke(velocity);
        m_lastMousePos = mousePos;
    }

    private void CheckMouseInput () {
        if (m_isPointerEnter && (Input.GetMouseButton(0) || Input.GetMouseButton(2))) {
            DispatchMoveEvent();
        } else {
            DisposeMove();
        }

        if (m_isPointerEnter && Input.mouseScrollDelta.y != 0f) {
            m_scaleValue += Input.mouseScrollDelta.y * 0.1f;
            m_scaleValue = Mathf.Clamp(m_scaleValue, m_scaleRange.min, m_scaleRange.max);
            onScaleAroundPointEvent?.Invoke(m_scaleValue, Input.mousePosition);
        }
    }

    private void CheckTouchInput () {
        if (m_isPointerEnter && Input.touchCount >= 2) {
            for (int i = 0, len = Input.touchCount; i < len; i++) {
                Touch touch = Input.GetTouch(i);
                if (!IsPressingTouch(touch)) continue;

                if (touch.fingerId == m_firstFingerId) {
                    m_firstPos = touch.position;
                } else if (touch.fingerId == m_secondFingerId) {
                    m_secondPos = touch.position;
                } else {
                    if (m_firstFingerId == -1) {
                        m_firstFingerId = touch.fingerId;
                        m_firstPos = touch.position;
                    } else {
                        m_secondFingerId = touch.fingerId;
                        m_secondPos = touch.position;
                        m_doubleTouchBeganDistance = Vector2.Distance(m_firstPos.Value, m_secondPos.Value);
                        m_scaleOnDoubleTouchBegan = m_scaleValue;
                        DisposeMove();
                    }
                }
            }
            if (m_firstPos != null && m_secondPos != null) {
                if (m_doubleTouchBeganDistance > 0) {
                    float distance = Vector2.Distance(m_firstPos.Value, m_secondPos.Value);
                    m_scaleValue = m_scaleOnDoubleTouchBegan * (distance / m_doubleTouchBeganDistance);
                    m_scaleValue = Mathf.Clamp(m_scaleValue, m_scaleRange.min, m_scaleRange.max);
                    onScaleAroundPointEvent?.Invoke(m_scaleValue, Vector2.Lerp(m_firstPos.Value, m_secondPos.Value, 0.5f));
                }
            }

            DispatchMoveEvent();
        } else if (m_isPointerEnter && Input.touchCount == 1) {
            DisposeScale();
            DispatchMoveEvent();
        } else {
            DisposeScale();
            DisposeMove();
        }
    }

    private bool IsPressingTouch (Touch touch) {
        return touch.phase == TouchPhase.Began || touch.phase == TouchPhase.Moved || touch.phase == TouchPhase.Stationary;
    }

    private void Init () {
        m_isInited = true;
        onInitScaleEvent?.Invoke(m_scaleValue);
    }

    private void Update () {
        if (!m_isInited) {
            Init();
        }

        if (Input.touchSupported) {
            CheckTouchInput();
        } else {
            CheckMouseInput();
        }
    }

    private void OnDisable () {
        m_isInited = false;
    }

#if UNITY_EDITOR
    private void OnValidate () {
        m_scaleValue = Mathf.Clamp(m_scaleValue, m_scaleRange.min, m_scaleRange.max);
    }
#endif

}
