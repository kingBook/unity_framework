using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using DG.Tweening;

public class CoinImage : MonoBehaviour {

    /// <summary> 一个金币创建完成（未开始收集动画之前）调用一次 </summary>
    public event System.Action onCreatedEvent;

    /// <summary> 一个金币收集动画完成（飞到目标点）调用一次 </summary>
    public event System.Action onCompleteEvent;

    [Tooltip("创建币后显示所在的半径范围")]
    public RangeFloat radiusFirst = new RangeFloat(20f, 70f);

    [Tooltip("创建币后向外扩张的半径范围")]
    public RangeFloat radiusSecond = new RangeFloat(90f, 120f);

    [Tooltip("向外扩张后停留的时间 <秒>")]
    public float stayTime = 0.5f;

    [Tooltip("收集动画持续时间 <秒>")]
    public float tweenDuration = 1f;

    [System.NonSerialized] public bool isStopOnCreated;
    [System.NonSerialized] public float delayOnCreatedTime;
    [System.NonSerialized] public RectTransform targetPointRectTransform;

    private RectTransform m_rectTransform;
    private Image m_image;
    private Sequence m_sequence;
    private Sequence m_sequence2;

    public void StartMoveToTargetAnimation() {
        Sequence sequence = DOTween.Sequence();

        // 3.移动到结束点
        sequence.AppendInterval(delayOnCreatedTime);
        sequence.AppendCallback(() => {
            m_rectTransform.DOMove(targetPointRectTransform.position, tweenDuration);
            m_rectTransform.DOScale(0.4f, tweenDuration);

        });

        sequence.AppendInterval(tweenDuration);

        // 4.完成
        sequence.OnComplete(() => {
            onCompleteEvent?.Invoke();
            if (isActiveAndEnabled) {
                gameObject.SetActive(false);
            }
        });

        m_sequence2 = sequence;
    }


    private void Awake() {
        m_rectTransform = (RectTransform)transform;
        m_image = GetComponent<Image>();
    }

    private void Start() {
        // 初始位置
        Vector2 randomDirection = new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f));
        float randomDistanceFirst = Random.Range(radiusFirst.min, radiusFirst.max);
        Vector2 anchoredPosition = m_rectTransform.anchoredPosition;
        anchoredPosition = randomDirection * randomDistanceFirst;
        m_rectTransform.anchoredPosition = anchoredPosition;

        // 计算向外展开的位置
        float randomDistanceSecond = Random.Range(radiusSecond.min, radiusSecond.max);
        Vector2 targetAnchoredPos = randomDirection * randomDistanceSecond;

        // 初始透明
        Color color = m_image.color;
        color.a = 0f;
        m_image.color = color;

        // 动画序列
        Sequence sequence = DOTween.Sequence();
        // 1.淡入显示
        sequence.Append(m_image.DOFade(1f, 0.2f));
        // 2.向外展开
        sequence.Append(m_rectTransform.DOAnchorPos(targetAnchoredPos, 0.3f));

        sequence.AppendInterval(stayTime);

        sequence.AppendCallback(() => {
            onCreatedEvent?.Invoke();
        });

        if (!isStopOnCreated) {
            sequence.AppendCallback(() => {
                StartMoveToTargetAnimation();
            });
        }

        m_sequence = sequence;
    }

    private void OnDisable() {
        DOTween.Kill(m_sequence);
        DOTween.Kill(m_sequence2);
        DOTween.Kill(m_image);
        DOTween.Kill(m_rectTransform);
    }


}
