using DG.Tweening;
using UnityEngine;
/// <summary>
/// 从指定的起始位置移动到当前位置
/// </summary>
public class MoveFromOnCanvas:BaseMonoBehaviour{
	[Tooltip("运动的起始位置(Canvas设计分辨率下的AnchoredPosition)")]
	public Vector2 from;
	[Tooltip("运动持续时间")]
	[Range(0,10)]
	public float duration=1.5f;

	/// <summary>
	/// void(MoveFromOnCanvas target)
	/// </summary>
	public event System.Action<MoveFromOnCanvas> onCompleteEvent;

	private RectTransform m_rectTransform;
	private Vector2 m_posRecord;

	protected override void Awake() {
		base.Awake();
		m_rectTransform=(RectTransform)transform;
		m_posRecord=m_rectTransform.anchoredPosition;
	}

	protected override void OnEnable() {
		base.OnEnable();
		m_rectTransform.anchoredPosition=from;
		m_rectTransform.DOAnchorPos(m_posRecord,duration).onComplete=OnComplete;
	}

	private void OnComplete() {
		m_rectTransform.DOKill();
		onCompleteEvent?.Invoke(this);
	}

	protected override void OnDisable() {
		m_rectTransform.DOKill();
		base.OnDisable();
	}

}