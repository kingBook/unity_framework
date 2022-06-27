using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// 拖 RectTransform 示例
/// </summary>
public class DragRectTransform : MonoBehaviour, IPointerDownHandler, IPointerUpHandler {

    private Canvas m_canvas;
    private RectTransform m_rectTransform;
    private bool m_isPointerDown;
    private Vector2 m_lastMousePos;
    private Vector2 m_positionOnPointerDown;


    void IPointerDownHandler.OnPointerDown(PointerEventData eventData) {
        m_lastMousePos = Input.mousePosition;
        m_isPointerDown = true;
        m_positionOnPointerDown = m_rectTransform.position;
    }

    void IPointerUpHandler.OnPointerUp(PointerEventData eventData) {
        m_isPointerDown = false;

        if (Vector2.Distance(m_rectTransform.position, m_positionOnPointerDown) <= 1.0f) {
            OnPointerClick(eventData); // 按下并释放后，如果未发生移动，则视为点击
        }
    }

    private void OnPointerClick(PointerEventData eventData) {
        Debug.Log("OnPointerClick");
    }

    #region Util
    private static void Move(RectTransform rectTransform, Vector2 velocityOnScreen, Canvas canvas, bool isLimitToScreen) {
        rectTransform.position += (Vector3)velocityOnScreen; // Canvas.RenderMode 为 Screen Space - Overlap 时，rectTransform.position 指的就是在屏幕坐标系的位置
        // 或
        //velocityOnScreen /= canvas.scaleFactor;
        //rectTransform.anchoredPosition += velocityOnScreen;

        if (isLimitToScreen) {
            LimitToScreen(rectTransform, canvas);
        }
    }

    private static void LimitToScreen(RectTransform rectTransform, Canvas canvas) {
        Vector2 sizeOnScreen = rectTransform.rect.size * canvas.scaleFactor; // rectTransform 在屏幕坐标系的大小
        //Vector2 sizeOnScreen = rectTransform.sizeDelta * canvas.scaleFactor; // 如果 rectTransform.Anchors 的四个角点在同一个点时，也可以使用此行代码

        Vector2 min = sizeOnScreen * rectTransform.pivot; // rectTransform 可移动的最小屏幕坐标值
        Vector2 max = new Vector2(Screen.width, Screen.height) - sizeOnScreen * (Vector2.one-rectTransform.pivot); // rectTransform 可移动的最大屏幕坐标值
        // 如果 rectTransform.pivot == (0.5,0.5) 时，也可以使用以下代码
        //Vector2 min = sizeOnScreen * 0.5f; 
        //Vector2 max = new Vector2(Screen.width, Screen.height) - sizeOnScreen * 0.5f;

        rectTransform.position = Vector2.Min(Vector2.Max(rectTransform.position, min), max); // 限制在 min 与 max 以内;
    }
    #endregion

    private void Awake() {
        m_rectTransform = (RectTransform)transform;
        m_canvas = m_rectTransform.root.GetComponent<Canvas>();
    }

    private void Update() {
        if (m_isPointerDown) {
            Vector2 mousePos = Input.mousePosition;
            Vector2 velocity = mousePos - m_lastMousePos;
            Move(m_rectTransform, velocity, m_canvas, true);
            m_lastMousePos = mousePos;
        }
    }
}
