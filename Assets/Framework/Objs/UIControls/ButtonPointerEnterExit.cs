using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// 用于实现滑入、滑出响应按钮
/// </summary>
[RequireComponent(typeof(Button))]
public class ButtonPointerEnterExit : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler {

    public UnityEvent onEnterEvent;
    public UnityEvent onUpEvent;

    [SerializeField, Tooltip("在支持鼠标的平台是否在鼠标左键按下并滑入按钮时才触发回调 ")] private bool m_isCheckMouseDown = true;

    private Button m_button;
    private bool m_isMouseDown;
    private bool m_isDown;

    void IPointerEnterHandler.OnPointerEnter(PointerEventData eventData) {
        if (Input.touchSupported) {
            OnDown(eventData);
        } else {

            if (m_isCheckMouseDown) {
                if (m_isMouseDown) {
                    OnDown(eventData);
                }
            } else {
                OnDown(eventData);
            }
        }
    }

    void IPointerExitHandler.OnPointerExit(PointerEventData eventData) {
        OnUp(eventData);
    }

    private void OnDown(PointerEventData eventData) {
        if (m_isDown) return;
        m_isDown = true;

        m_button.OnPointerDown(eventData);
        onEnterEvent?.Invoke();
    }

    private void OnUp(PointerEventData eventData) {
        if (!m_isDown) return;
        m_isDown = false;

        m_button.OnPointerUp(eventData);
        onUpEvent?.Invoke();
    }

    private void SetMouseDown(bool value) {
        if (m_isMouseDown == value) return;
        if (!value) {
            // 释放鼠标时
            OnUp(new PointerEventData(EventSystem.current) {
                position = Input.mousePosition
            });
        }
        m_isMouseDown = value;
    }

    private void Awake() {
        m_button = GetComponent<Button>();
    }

    private void Update() {
        if (m_isCheckMouseDown) {
            SetMouseDown(Input.GetMouseButton(0));
        }
    }


}
