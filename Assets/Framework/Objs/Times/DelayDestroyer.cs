using System.Collections;
using UnityEngine;

/// <summary>
/// 延时销毁器
/// </summary>
public class DelayDestroyer : MonoBehaviour {

    [Tooltip("时间（秒）")] public float time = 5;

    private void DestroySelf () {
        Destroy(gameObject);
    }

    private IEnumerator Start () {
        yield return null;
        Invoke(nameof(DestroySelf),time);
    }
}
