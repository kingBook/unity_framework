using DG.Tweening;
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class Flashing : MonoBehaviour {

    [Tooltip("闪烁间隔")] public float interval = 0.03f;

    private SpriteRenderer m_spriteRenderer;
    private float m_alphaRecord;
    private Sequence m_sequence;

    public bool isFlashing { get; private set; }

    public void StartFlashing(float timeSeconds = 3.0f) {
        StartFlashing(timeSeconds, null);
    }

    public void StartFlashing(float timeSeconds, System.Action onComplete) {
        if (isFlashing) return;
        isFlashing = true;

        m_alphaRecord = m_spriteRenderer.color.a;

        m_sequence = DOTween.Sequence();
        m_sequence.Append(m_spriteRenderer.DOFade(0.0f, interval));
        m_sequence.Append(m_spriteRenderer.DOFade(1.0f, interval));
        m_sequence.SetLoops((int)(timeSeconds / (2.0f * interval)));
        m_sequence.OnComplete(() => {
            isFlashing = false;

            var color = m_spriteRenderer.color;
            color.a = m_alphaRecord;
            m_spriteRenderer.color = color;

            m_sequence = null;

            onComplete?.Invoke();
        });
    }

    private void Awake() {
        m_spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void OnDestroy() {
        if (m_sequence != null) {
            m_sequence.Kill();
        }
    }

}
