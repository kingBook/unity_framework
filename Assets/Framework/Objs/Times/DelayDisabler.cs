using System.Collections;
using UnityEngine;

/// <summary>
/// 延迟禁用器
/// </summary>
public class DelayDisabler : MonoBehaviour {

    [Tooltip("时间（秒）")] public float time = 5;

    private bool m_isDelaying;

    private void DisableSelf () {
        gameObject.SetActive(false);
    }

    private void Update () {
        if (m_isDelaying) return;

        m_isDelaying = true;
        Invoke(nameof(DisableSelf), time);
    }
}
