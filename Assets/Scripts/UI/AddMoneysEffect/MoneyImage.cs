using DG.Tweening;
using System.Collections;
using UnityEngine;

public class MoneyImage : MonoBehaviour {

    /// <summary> 一个金币收集动画完成（飞到目标点）调用一次 </summary>
    public event System.Action onCompleteEvent;

    public float duration = 1f;

    [System.NonSerialized] public RectTransform targetPointRectTransform;

    private void OnTweenComplete () {
        onCompleteEvent?.Invoke();
        Destroy(gameObject);
    }

    private void Start () {
        transform.DOMove(targetPointRectTransform.position, duration).OnComplete(OnTweenComplete);
    }

    private void OnDestroy () {
        DOTween.Kill(transform);
    }
}
