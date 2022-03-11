using System.Collections;
using UnityEngine;

/// <summary>
/// 延迟禁用器
/// </summary>
public class DelayDisabler : MonoBehaviour {

    [Tooltip("时间（秒）")] public float time = 5;

    private void DisableSelf () {
        gameObject.SetActive(false);
    }

    private IEnumerator Start () {
        yield return null;
        Invoke(nameof(DisableSelf), time);
    }
}
