using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

[RequireComponent(typeof(VideoPlayer))]
[RequireComponent(typeof(RawImage))]
[RequireComponent(typeof(RenderTextureGetter))]
public class VideoPlayerController : MonoBehaviour {

    private VideoPlayer m_videoPlayer;
    private RawImage m_rawImage;
    private RenderTextureGetter m_renderTextureGetter;
    private System.Action m_gotoFrameCompleteAction;
    private long m_gotoFrame;

    /// <summary>
    /// 播放器准备就绪事件，回调函数格式：<code> void OnPrepareCompleteHandler() </code>
    /// </summary>
    public event System.Action onPrepareCompleteEvent;

    /// <summary>
    /// 播放完成事件，回调函数格式：<code> void OnPlayCompleteHandler() </code>
    /// </summary>
    public event System.Action onPlayCompleteEvent;

    /// <summary>
    /// 控制的 VideoPlayer
    /// </summary>
    public VideoPlayer videoPlayer => m_videoPlayer;

    /// <summary>
    /// 缓冲中...
    /// <para> 注意： </para>
    /// <para> 当执行跳帧/时间操作时，此变量不能作为是否已跳到指定帧/时间的必要条件 </para>
    /// </summary>
    public bool isSeeking { get; private set; }

    /// <summary>
    /// 已准备就绪的帧索引
    /// </summary>
    public long frameNumberReadied { get; private set; }

    /// <summary>
    /// 播放头的时间 (秒)，如果播放内容未准备就绪（videoPlayer.isPrepared==false）时，则返回 0
    /// </summary>
    public float time => m_videoPlayer.isPrepared ? m_videoPlayer.frame / m_videoPlayer.frameRate : 0.0f;

    /// <summary>
    /// 视频总时长（秒），如果播放内容未准备就绪（videoPlayer.isPrepared==false）时，则返回 0
    /// </summary>
    public float timeTotal => m_videoPlayer.isPrepared ? m_videoPlayer.frameCount / m_videoPlayer.frameRate : 0.0f;

    public void Play() {
        m_videoPlayer.Play();
    }

    public void Pause() {
        m_videoPlayer.Pause();
    }

    public void Stop() {
        m_videoPlayer.Stop();
    }

    /// <summary>
    /// 设置播放时间点
    /// <para> 注意： </para>
    /// <para> 1). 播放引擎必须准备就绪（videoPlayer.isPrepare==true）</para>
    /// <para> 2). 调用此方法前必须暂停播放 videoPlayer.Pause() </para>
    /// <para> 3). 每次设置跳到不同的帧/时间都会触发缓冲，在 videoPlayer.seekCompleted 及 videoPlayer.frameReady 事件都发出后，才真正跳到指定的帧/时间 </para>
    /// </summary>
    /// <param name="time"> 秒 </param>
    /// <param name="onComplete"> 已跳到指定帧/时间后执行，可以 null </param>
    public void SetTime(float time, System.Action onComplete) {
        SetFrame((long)(time * m_videoPlayer.frameRate), onComplete);
    }

    /// <summary>
    /// 根据[0,1]的进度条值，设置播放时间点
    /// <para> 注意： </para>
    /// <para> 1). 播放引擎必须准备就绪（videoPlayer.isPrepare==true）</para>
    /// <para> 2). 调用此方法前必须暂停播放 videoPlayer.Pause() </para>
    /// <para> 3). 每次设置跳到不同的帧/时间都会触发缓冲，在 videoPlayer.seekCompleted 及 videoPlayer.frameReady 事件都发出后，才真正跳到指定的帧/时间 </para>
    /// </summary>
    /// <param name="sliderValue"> [0,1] </param>
    /// <param name="onComplete"> 已跳到指定帧/时间后执行，可以 null </param>
    public void SetTimeWithSlider(float sliderValue, System.Action onComplete) {
        sliderValue = Mathf.Clamp01(sliderValue);
        SetFrame((long)(m_videoPlayer.frameCount * sliderValue), onComplete);
    }

    /// <summary>
    /// 设置跳到指定帧
    /// <para> 注意： </para>
    /// <para> 1). 播放引擎必须准备就绪（videoPlayer.isPrepare==true）</para>
    /// <para> 2). 调用此方法前必须暂停播放 videoPlayer.Pause() </para>
    /// <para> 3). 每次设置跳到不同的帧/时间都会触发缓冲，在 videoPlayer.seekCompleted 及 videoPlayer.frameReady 事件都发出后，才真正跳到指定的帧/时间 </para>
    /// </summary>
    /// <param name="frame"> 范围 [0, videoPlayer.frameCount-1]</param>
    /// <param name="onComplete"> 已跳到指定帧/时间后执行，可以 null </param>
    public void SetFrame(long frame, System.Action onComplete) {
        if (m_videoPlayer.isPrepared) {
            m_gotoFrame = frame >= (long)m_videoPlayer.frameCount ? (long)m_videoPlayer.frameCount - 1 : frame;
            m_gotoFrame = m_gotoFrame < 0 ? 0 : m_gotoFrame;
            m_videoPlayer.frame = m_gotoFrame;
            m_gotoFrameCompleteAction = onComplete;
            isSeeking = true;
            // 如果播放头在要跳到的帧，则直接完成
            CheckGotoFrameCompleted();
        } else {
            // 播放引擎未准备就绪，请调用 m_videoPlayer.Play()、m_videoPlayer.Prepare() 方法
            // 侦听 m_videoPlayer.prepareCompleted 事件，确认已准备就绪
            Debug.LogError("播放引擎未准备好播放内容，不能跳到指定帧");
        }
    }

    private void OnPrepareCompletedHandler(VideoPlayer source) {
        onPrepareCompleteEvent?.Invoke();
    }

    private void OnSeekCompletedHandler(VideoPlayer source) {
        isSeeking = false;
        CheckGotoFrameCompleted();
    }

    private void OnFrameReadiedHandler(VideoPlayer source, long frameIdx) {
        frameNumberReadied = frameIdx;
        CheckGotoFrameCompleted();
        CheckComplete();
    }

    private void CheckGotoFrameCompleted() {
        if (m_gotoFrame > 0) {
            if (m_videoPlayer.frame == m_gotoFrame) {
                if (m_gotoFrameCompleteAction != null) {
                    m_gotoFrameCompleteAction.Invoke();
                    m_gotoFrameCompleteAction = null;
                }
                m_gotoFrame = 0;
            }
        }
    }

    private void CheckComplete() {
        if (!m_videoPlayer.isPrepared) return;
        if (m_videoPlayer.frame > 0 && m_videoPlayer.frameCount > 0 && m_videoPlayer.frame >= (long)m_videoPlayer.frameCount - 1) {
            if (onPlayCompleteEvent != null) {
                Debug.Log($"onPlayComplete, frame:{m_videoPlayer.frame}, frameCount:{m_videoPlayer.frameCount}");
                onPlayCompleteEvent.Invoke();
            }
        }
    }

    private void OnClockResyncOccurredHandler(VideoPlayer source, double seconds) {
        Debug.Log($"OnClockResyncOccurredHandler:{seconds}");
    }

    private void OnErrorReceivedHandler(VideoPlayer source, string message) {
        Debug.LogError($"OnErrorReceivedHandler:{message}");
    }

    private void Awake() {
        m_rawImage = GetComponent<RawImage>();
        m_renderTextureGetter = GetComponent<RenderTextureGetter>();

        m_videoPlayer = GetComponent<VideoPlayer>();
        m_videoPlayer.skipOnDrop = false; // 不允许跳过帧以赶上当前时间
        m_videoPlayer.prepareCompleted += OnPrepareCompletedHandler;
        m_videoPlayer.seekCompleted += OnSeekCompletedHandler;
        // 这可能会占用 CPU 资源
        m_videoPlayer.sendFrameReadyEvents = true;
        m_videoPlayer.frameReady += OnFrameReadiedHandler;
        m_videoPlayer.clockResyncOccurred += OnClockResyncOccurredHandler;
        m_videoPlayer.errorReceived += OnErrorReceivedHandler;
    }

    private void OnEnable() {
        // 启动播放引擎准备。
        // 准备工作包括预留播放所需的资源，以及预加载要播放的部分或全部内容。完成这些工作后，可以立即接收帧并可以查询与源相关的所有属性。
        m_videoPlayer.Prepare();
    }

    private void Start() {
        var renderTexture = m_renderTextureGetter.renderTexture;
        m_rawImage.texture = renderTexture;
        m_videoPlayer.targetTexture = renderTexture;
    }

    private void FixedUpdate() {
        CheckGotoFrameCompleted();
        CheckComplete();
    }

    private void OnDestroy() {
        if (m_videoPlayer) {
            m_videoPlayer.prepareCompleted -= OnPrepareCompletedHandler;
            m_videoPlayer.seekCompleted -= OnSeekCompletedHandler;
            m_videoPlayer.frameReady -= OnFrameReadiedHandler;
            m_videoPlayer.clockResyncOccurred -= OnClockResyncOccurredHandler;
        }
    }

}
