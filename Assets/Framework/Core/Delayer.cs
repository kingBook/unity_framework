using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// 延迟器
/// </summary>
public class Delayer : MonoBehaviour {

    private struct DelayObject {
        public float timeOnInit;
        public float time;
        public System.Action onComplete;
        public MonoBehaviour monoBehaviour;

        public DelayObject (float timeOnInit, float time, MonoBehaviour monoBehaviour, System.Action onComplete) {
            this.timeOnInit = timeOnInit;
            this.time = time;
            this.monoBehaviour = monoBehaviour;
            this.onComplete = onComplete;
        }
    }

    private List<DelayObject> m_delayObjects = new List<DelayObject>();
    private float m_pauseStartTime;
    private float m_pauseTime;

    /// <summary>
    /// 延迟执行一个函数（只有 monoBehaviour 被销毁、<see cref="App"/>暂停时，才会中断执行, Disable 不会中断）
    /// </summary>
    /// <param name="time"> 延迟的时间 </param>
    /// <param name="monoBehaviour"> 用于检测销毁的 MonoBehaviour，一般为 this </param>
    /// <param name="onComplete"> 延迟完成时的回调 </param>
    public void Delay (float time, MonoBehaviour monoBehaviour, System.Action onComplete) {
        if (onComplete == null) {
            Debug.LogError("参数 onComplete 不能为 null");
        }

        DelayObject delayObject = new DelayObject(Time.time, time, monoBehaviour, onComplete);
        m_delayObjects.Add(delayObject);
    }

    private void OnPauseOrResume (bool isPause) {
        if (isPause) {
            m_pauseStartTime = Time.time;
        } else {
            m_pauseTime = Time.time - m_pauseStartTime;
        }
    }

    private void Start () {
        App.instance.onPauseOrResumeEvent += OnPauseOrResume;
    }

    private void Update () {
        if (App.instance.isPause) return;

        int i = m_delayObjects.Count;
        while (--i >= 0) {
            DelayObject delayObject = m_delayObjects[i];
            if (!delayObject.monoBehaviour) {
                m_delayObjects.RemoveAt(i);
            }else if (Time.time - delayObject.timeOnInit >= delayObject.time + m_pauseTime) {
                delayObject.onComplete.Invoke();
                m_delayObjects.RemoveAt(i);
            }
        }

        // 暂停经过的时间，每帧重置
        m_pauseTime = 0f;
    }

    private void OnDestroy () {
        if (App.instance) {
            App.instance.onPauseOrResumeEvent -= OnPauseOrResume;
        }
    }
}
