using System.Collections;
using UnityEngine;

/// <summary>
/// 倒计时器
/// </summary>
public class CountDownTimer : MonoBehaviour {

    [Tooltip("倒计时的秒数")] public int timeSecondsTotal = 4 * 60;

    private int m_timeSeconds;
    private bool m_isPaused;

    /// <summary>
    /// 计时事件，回调函数格式：<code> void OnTimerHandler(int timeSeconds) </code>
    /// </summary>
    public event System.Action<int> onTimerEvent;

    /// <summary>
    /// 时间到事件，回调函数格式：<code> void OnTimeUpHandler() </code>
    /// </summary>
    public event System.Action onTimeUpEvent;

    /// <summary>
    /// 当前倒计时的秒数
    /// </summary>
    public int timeSecounds => m_timeSeconds;

    /// <summary>
    /// 在<see cref="StartTimer"/>后为 true, 当<see cref="StopTimer"/>后为false
    /// </summary>
    public bool isStarted { get; private set; }

    /// <summary>
    /// 是否已暂停
    /// </summary>
    public bool isPaused => m_isPaused;

    /// <summary>
    /// 开始计时（设置<see cref="timeSecounds"/>为<see cref="timeSecondsTotal"/>，<see cref="isStarted"/>为 true）
    /// </summary>
    public void StartTimer() {
        ResetTime();
        InvokeRepeating(nameof(OnTimer), 1f, 1f);
        isStarted = true;
    }

    /// <summary>
    /// 停止计时（设置<see cref="isStarted"/>设置为 false）
    /// </summary>
    public void StopTimer() {
        CancelInvoke(nameof(OnTimer));
        isStarted = false;
    }

    /// <summary>
    /// 暂停计时
    /// </summary>
    /// <param name="value"></param>
    public void SetPause(bool value) {
        m_isPaused = value;
    }

    /// <summary>
    /// 重置 <see cref="timeSecounds"/>等于<see cref="timeSecondsTotal"/>
    /// </summary>
    public void ResetTime() {
        m_timeSeconds = timeSecondsTotal;
    }

    private void OnTimer() {
        if (m_isPaused) return;
        m_timeSeconds--;

        onTimerEvent?.Invoke(m_timeSeconds);

        if (m_timeSeconds <= 0) {
            StopTimer();
            onTimeUpEvent?.Invoke();
        }
    }

    private void Start() {
        StartTimer();
    }
}
