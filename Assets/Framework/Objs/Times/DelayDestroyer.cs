using System.Collections;
using UnityEngine;

/// <summary>
/// 延时销毁器
/// </summary>
public class DelayDestroyer : MonoBehaviour {

    [Tooltip("时间（秒）")] public float time = 5;

    private bool m_isDelaying;

    private void DestroySelf () {
        Destroy(gameObject);
    }

    private void Update () {
        if (m_isDelaying) return;

        m_isDelaying = true;
        Invoke(nameof(DestroySelf), time);
    }
}
