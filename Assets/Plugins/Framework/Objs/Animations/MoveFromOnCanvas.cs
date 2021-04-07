using DG.Tweening;
using UnityEngine;

/// <summary>
/// 从指定的起始位置移动到当前位置
/// </summary>
/// 
[RequireComponent(typeof(RectTransform))]
public class MoveFromOnCanvas : MonoBehaviour {
    [Tooltip("运动的起始位置(Canvas设计分辨率下的AnchoredPosition)")]
    public Vector2 from;
    [Tooltip("运动持续时间"), Range(0,10)]
    public float duration=1.5f;

    /// <summary>
    /// void(MoveFromOnCanvas target)
    /// </summary>
    public event System.Action<MoveFromOnCanvas> onCompleteEvent;

    private RectTransform m_rectTransform;
    private Vector2 m_posRecord;

    private void Awake () {
        m_rectTransform = GetComponent<RectTransform>();
        m_posRecord = m_rectTransform.anchoredPosition;
    }

    private void OnEnable () {
        m_rectTransform.anchoredPosition = from;
        m_rectTransform.DOAnchorPos(m_posRecord, duration).onComplete = OnComplete;
    }

    private void OnComplete () {
        m_rectTransform.DOKill();
        onCompleteEvent?.Invoke(this);
    }

    private void OnDisable () {
        m_rectTransform.DOKill();
    }

}