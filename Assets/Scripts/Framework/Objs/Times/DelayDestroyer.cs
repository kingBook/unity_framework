using System.Collections;
using UnityEngine;

/// <summary>
/// 延时销毁器
/// </summary>
public class DelayDestroyer : MonoBehaviour {

    [Tooltip("时间（秒）")] public float time = 5;

    /// <summary>
    /// 销毁事件，回调函数格式：<code> void OnDestroyHandler() </code>
    /// </summary>
    public event System.Action onDestroyEvent;

    private void DestroySelf() {
        onDestroyEvent?.Invoke();
        Destroy(gameObject);
    }

    private IEnumerator Start() {
        yield return null;
        Invoke(nameof(DestroySelf), time);
    }
}
