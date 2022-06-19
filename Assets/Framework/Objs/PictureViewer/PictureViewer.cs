using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PictureViewer : MonoBehaviour {

    /// <summary>
    /// 初始缩放事件，回调函数格式：<code> void OnInitScaleHandler(float scaleValue) </code>
    /// <para> 可以接收此事件，通过<see cref="MoveToScreenPoint"/>方法设置初始时聚焦的位置 </para>
    /// </summary>
    public event System.Action<float> onInitScaleEvent;

    [SerializeField] private RectTransform m_content;

    private PictureViewerInput m_viewerInput;
    private Canvas m_canvas;
    private RectTransform m_rectTransform;


    /// <summary>
    /// 聚焦到指定的屏幕点
    /// </summary>
    /// <param name="screenPoint"></param>
    public void MoveToScreenPoint(Vector2 screenPoint) {
        Vector2 focusPoint = m_rectTransform.position;
        Vector2 velocity = focusPoint - screenPoint;
        RectTransformUtil.Move(m_content, velocity, m_canvas, true);
    }

    private void OnInitScale(float scaleValue) {
        onInitScaleEvent?.Invoke(scaleValue);
    }

    private void OnMove(Vector2 velocity) {
        RectTransformUtil.Move(m_content, velocity, m_canvas, true);
    }

    private void OnScaleAroundPoint(float scaleValue, Vector2 aroundScreenPoint) {
        RectTransformUtil.ScaleAroundPoint(m_content, aroundScreenPoint, scaleValue, m_canvas, true);
    }

    private void Awake() {
        m_viewerInput = GetComponent<PictureViewerInput>();
        m_canvas = GetComponentInParent<Canvas>();
        m_rectTransform = (RectTransform)transform;

        m_viewerInput.onInitScaleEvent += OnInitScale;
        m_viewerInput.onMoveEvent += OnMove;
        m_viewerInput.onScaleAroundPointEvent += OnScaleAroundPoint;
    }

    private void OnDestroy() {
        if (m_viewerInput) {
            m_viewerInput.onMoveEvent -= OnMove;
            m_viewerInput.onScaleAroundPointEvent -= OnScaleAroundPoint;
        }
    }



}
