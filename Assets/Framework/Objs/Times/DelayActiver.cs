using System.Collections;
using UnityEngine;

/// <summary>
/// 延迟激活器
/// </summary>
public class DelayActiver : MonoBehaviour {

    [Tooltip("时间（秒）")] public float time = 5;

    private bool m_isDelaying;

    private void ActiveSelf () {
        gameObject.SetActive(true);
    }

    private void Update () {
        if (m_isDelaying) return;

        m_isDelaying = true;
        Invoke(nameof(ActiveSelf), time);
    }
}
