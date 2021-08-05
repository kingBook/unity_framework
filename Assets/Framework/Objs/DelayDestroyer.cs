using System.Collections;
using UnityEngine;

/// <summary>
/// 延时销毁器
/// </summary>
public class DelayDestroyer : MonoBehaviour {

    public float time = 5;

    private bool m_timerStarting;

    private void DestroySelf () {
        Destroy(gameObject);
    }

    private void Update () {
        if (m_timerStarting) return;

        m_timerStarting = true;
        Invoke(nameof(DestroySelf), time);
    }
}
