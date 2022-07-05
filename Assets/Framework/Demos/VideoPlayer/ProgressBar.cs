using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ProgressBar : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler {

    private VideoPlayerController m_videoPlayerController;
    private Slider m_slider;
    private bool m_isPointerDown;
    private bool m_isPlayingOnPointerDown;
    private long m_frameOnPointerDown;

    /// <summary>
    /// 按下/拖动滑块时，滑块当前位置表示的帧数
    /// <para> 注意： </para>
    /// <para> 在 VideoPlayer 播放头真正到达此帧数且不在进度条中按下时，此变量才重置为 0 </para>
    /// </summary>
    public long frameOnPointerDown => m_frameOnPointerDown;

    /// <summary>
    /// 按下/拖动滑块时，滑块当前位置表示的时间（秒）
    /// <para> 注意： </para>
    /// <para> 在 VideoPlayer 播放头真正到达此时间且不在进度条中按下时，此变量才重置为 0 </para>
    /// </summary>
    public float timeOnPointerDown => m_frameOnPointerDown / m_videoPlayerController.videoPlayer.frameRate;

    void IPointerDownHandler.OnPointerDown(PointerEventData eventData) {
        m_isPointerDown = true;
        m_isPlayingOnPointerDown = m_videoPlayerController.videoPlayer.isPlaying;
        m_frameOnPointerDown = (long)(m_videoPlayerController.videoPlayer.frameCount * m_slider.value);
        m_videoPlayerController.Pause();
    }

    void IDragHandler.OnDrag(PointerEventData eventData) {
        m_frameOnPointerDown = (long)(m_videoPlayerController.videoPlayer.frameCount * m_slider.value);
        m_videoPlayerController.SetFrame(m_frameOnPointerDown, null);
    }

    void IPointerUpHandler.OnPointerUp(PointerEventData eventData) {
        m_isPointerDown = false;
        m_videoPlayerController.SetFrame(m_frameOnPointerDown, () => {
            if (m_isPlayingOnPointerDown) {
                m_videoPlayerController.Play();
            }
        });
    }

    private void Awake() {
        m_videoPlayerController = GetComponentInParent<VideoPlayerController>();
        m_slider = GetComponent<Slider>();
    }

    private void Update() {
        // 更新播放进度条
        if (!m_isPointerDown && m_frameOnPointerDown > 0) {
            if (m_videoPlayerController.videoPlayer.frame == m_frameOnPointerDown) {
                m_frameOnPointerDown = 0;
            }
        }
        if (m_frameOnPointerDown > 0) {
            m_slider.value = (float)m_frameOnPointerDown / m_videoPlayerController.videoPlayer.frameCount;
        } else {
            m_slider.value = (float)m_videoPlayerController.videoPlayer.frame / m_videoPlayerController.videoPlayer.frameCount;
        }


    }


}
