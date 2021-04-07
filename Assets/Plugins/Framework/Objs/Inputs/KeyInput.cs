#pragma warning disable 0649
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;


/// <summary>
/// 按键手柄
/// </summary>
public class KeyInput : MonoBehaviour {

    public enum Mode { Handle, Automatic }

    public event System.Action onKeyDownEvent;
    public event System.Action onKeyUpEvent;

    [SerializeField] private Button m_buttonHandle;
    [SerializeField] private float m_disableAlpha=0.5f;
    [SerializeField] private float m_enableAlpha=1.0f;
    [SerializeField] private KeyCode m_keyCode=KeyCode.J;
    [SerializeField] private Mode m_mode=Mode.Automatic;

    private bool m_enableHandle;
    private Image m_buttonHandleImage;

    /// <summary> 是否按下按钮/键盘上的 <see cref="m_keyCode"/> </summary>
    public bool isPressed { get; private set; }

    private void Awake () {
        m_buttonHandleImage = m_buttonHandle.GetComponent<Image>();
        //根据模式设置 m_enableHandle
        if (m_mode == Mode.Automatic) {
            m_enableHandle = Input.touchSupported;
        } else if (m_mode == Mode.Handle) {
            m_enableHandle = true;
        }
        //设置未激活状态透明度
        Color color=m_buttonHandleImage.color;
        color.a = m_disableAlpha;
        m_buttonHandleImage.color = color;
        //根据 m_enabledHandle 显示/隐藏UI按钮
        m_buttonHandleImage.gameObject.SetActive(m_enableHandle);
        //启动UI按键时，侦听按下、释放
        if (m_enableHandle) {
            EventTrigger eventTrigger=m_buttonHandle.gameObject.AddComponent<EventTrigger>();
            //按下
            EventTrigger.Entry entry=new EventTrigger.Entry();
            entry.eventID = EventTriggerType.PointerDown;
            entry.callback.AddListener((eventData) => {
                OnKeyDown();
            });
            eventTrigger.triggers.Add(entry);
            //释放
            entry = new EventTrigger.Entry();
            entry.eventID = EventTriggerType.PointerUp;
            entry.callback.AddListener((eventData) => {
                OnKeyUp();
            });
            eventTrigger.triggers.Add(entry);
        }
    }

    private void Update () {
        //未启用UI按钮时，侦听键盘输入
        if (!m_enableHandle) {
            if (Input.GetKeyDown(m_keyCode)) {
                OnKeyDown();
            } else if (Input.GetKeyUp(m_keyCode)) {
                OnKeyUp();
            }
        }
    }

    private void OnKeyDown () {
        isPressed = true;
        //设置未激活状态透明度
        Color color=m_buttonHandleImage.color;
        color.a = m_enableAlpha;
        m_buttonHandleImage.color = color;

        onKeyDownEvent?.Invoke();
    }

    private void OnKeyUp () {
        isPressed = false;
        //设置激活状态透明度
        Color color=m_buttonHandleImage.color;
        color.a = m_disableAlpha;
        m_buttonHandleImage.color = color;

        onKeyUpEvent?.Invoke();
    }



}
