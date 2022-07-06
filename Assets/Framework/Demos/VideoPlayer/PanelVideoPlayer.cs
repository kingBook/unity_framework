using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class PanelVideoPlayer : MonoBehaviour {

    [SerializeField] VideoPlayerController m_videoPlayerController;
    [SerializeField] Button m_buttonPlay;
    [SerializeField] Button m_buttonPause;
    [SerializeField] TMP_Text m_textTime;
    [SerializeField] ProgressBar m_progressBar;
    [SerializeField] TMP_Text m_textFrame;
    [SerializeField] TMP_InputField m_inputField;
    [SerializeField] Image m_imageLoading;

    /// <summary>
    /// VideoPlayer 控制器
    /// </summary>
    public VideoPlayerController videoPlayerController => m_videoPlayerController;

    /// <summary>
    /// 视频进度条
    /// </summary>
    public ProgressBar progressBar => m_progressBar;

    #region Editor References
    public void OnClickButtonPlay() {
        m_videoPlayerController.Play();
    }

    public void OnClickButtonPause() {
        m_videoPlayerController.Pause();
    }

    public void OnClickButtonGo() {
        if (long.TryParse(m_inputField.text, out long frame)) {
            bool isPlaying = m_videoPlayerController.videoPlayer.isPlaying;
            m_videoPlayerController.Pause();
            m_videoPlayerController.SetFrame(frame, () => {
                if (isPlaying) {
                    m_videoPlayerController.Play();
                }
            });
        }
    }
    #endregion


    private void UpdateUI() {
        // 播放、暂停按钮的激活状态切换
        m_buttonPlay.gameObject.SetActive(!m_videoPlayerController.videoPlayer.isPlaying);
        m_buttonPause.gameObject.SetActive(m_videoPlayerController.videoPlayer.isPlaying);

        // 更新播放的时间文本
        const float maxTime = 215999.0f; // 如果以时分秒的形式显示时间不能超 59:59:59，就是 59*60*60+59*60+59=215999 秒
        float time = m_progressBar.frameOnPointerDown > 0 ? m_progressBar.timeOnPointerDown : m_videoPlayerController.time;
        time = Mathf.Clamp(time, 0.0f, maxTime);
        float timeTotal = Mathf.Clamp(m_videoPlayerController.timeTotal, 0.0f, maxTime);
        string timeString = TimeSpan.FromSeconds(time).ToString(@"hh\:mm\:ss");
        string timeTotalString = TimeSpan.FromSeconds(timeTotal).ToString(@"hh\:mm\:ss");
        m_textTime.text = $"{timeString}/{timeTotalString}";

        // 更新当前播放头的帧数
        m_textFrame.text = $"Frame:{m_videoPlayerController.videoPlayer.frame}";

        // 是否显示'加载圈'
        m_imageLoading.gameObject.SetActive(m_videoPlayerController.isSeeking);
    }

    private void Update() {
        UpdateUI();
    }


}
