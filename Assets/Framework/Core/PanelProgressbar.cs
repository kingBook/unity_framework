using TMPro;
using UnityEngine;
using UnityEngine.UI;
#pragma warning disable 0649

public class PanelProgressbar : MonoBehaviour {

    [Tooltip("进度条滑块"), SerializeField]
    private Slider m_slider;

    [Tooltip("百分比文本框"), SerializeField]
    private TMP_Text m_text;

    /// <summary>
    /// 设置显示的进度
    /// </summary>
    /// <param name="ratio">范围：[0,1]</param>
    public void SetProgress(float ratio) {
        m_slider.value = Mathf.Clamp01(ratio);
    }

    /// <summary>
    /// 设置进度条上的文本
    /// </summary>
    /// <param name="textString">显示的字符串</param>
    public void SetText(string textString) {
        m_text.text = textString;
    }
}
