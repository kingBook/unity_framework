#pragma warning disable 0649

using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PanelProgressbar : MonoBehaviour {

    [Tooltip("进度条滑块"), SerializeField]
    private Slider m_slider;

    [Tooltip("百分比文本框"), SerializeField]
    private TMP_Text m_text;

    /// <summary> 设置显示的进度, 范围：[0,1] </summary>
    public void SetProgress(float progress) {
        m_slider.value = Mathf.Clamp01(progress);
        m_text.text = $"Loading {Mathf.FloorToInt(progress * 100)}%";
    }

}
