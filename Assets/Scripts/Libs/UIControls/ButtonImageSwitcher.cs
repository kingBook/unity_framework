using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 点击按钮时，切换按钮上的图片
/// </summary>
public class ButtonImageSwitcher : ImageSwitcher {

    [Tooltip("是否侦听点击按钮事件自动切换")]
    public bool isSwapOnClick = true;

    private Button m_button;

#if UNITY_EDITOR
    protected override void InitImageOnReset() {
        if (!image) {
            Button btn = GetComponent<Button>();
            if (btn) {
                image = btn.image;
            } else {
                image = GetComponent<Image>();
            }
        }
    }
#endif

    protected override void Awake() {
        base.Awake();

        m_button = GetComponent<Button>();
        if (isSwapOnClick) {
            m_button.onClick.AddListener(OnClick);
        }
    }

    private void OnClick() {
        AutoSwap();
    }

    private void OnDestroy() {
        if (isSwapOnClick) {
            m_button.onClick.RemoveListener(OnClick);
        }
    }
}
