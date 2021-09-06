#pragma warning disable 0649
using UnityEngine;
using UnityEngine.UI;


/// <summary>
/// 获得钱币动画
/// 
/// <code>
/// GameObject inst = Instantiate(m_prefab, m_prefab.transform.parent);
/// inst.SetActive(true);
///
/// GetCoinsEffect effect = inst.GetComponent<GetCoinsEffect>();
/// effect.allowRaycast = false;
/// effect.SetAddCount(100);
/// effect.onCompleteEvent += (GetCoinsEffect getCoinsEffect, float progress, int addCount) => {
///     m_game.goldCount += addCount;
/// };
/// </code>
/// 
/// </summary>
public class GetCoinsEffect : MonoBehaviour {

    /// <summary> 一个币创建完成（未开始收集动画之前），格式：<code> void (GetCoinsEffect getCoinsEffect, float progress) </code> </summary>
    public event System.Action<GetCoinsEffect, float> onCreatedEvent;

    /// <summary> 所有币都创建完成（未开始收集动画之前），格式：<code> void (GetCoinsEffect getCoinsEffect) </code> </summary>
    public event System.Action<GetCoinsEffect> onAllCreatedEvent;

    /// <summary> 一个币收集动画完成（飞到目标点），格式：<code> void (GetCoinsEffect getCoinsEffect, float progress, int addCount) </code> </summary>
    public event System.Action<GetCoinsEffect, float, int> onCompleteEvent;

    /// <summary> 所胡币收集动画完成（飞到目标点），格式：<code> void (GetCoinsEffect getCoinsEffect) </code> </summary>
    public event System.Action<GetCoinsEffect> onAllCompleteEvent;

    [Tooltip("创建币的数量")]
    public int coinImageCount = 20;

    [Tooltip("创建间隔时间 <秒>")]
    public float createIntervalTime = 0.1f;

    [Tooltip("创建出币后收集聚拢的目标点")]
    public RectTransform targetPointRectTransform;

    [Tooltip("币预制件")]
    public GameObject coinImagePrefab;

    [Tooltip("true 时将创建出多个币后停止动画，直到调用 StartMoveToTargetAnimation() 方法才继续动画")]
    public bool stopOnCreated;

    [Tooltip("允许射线投射计算（UI 按钮在动画结束前是否能被点击）")]
    public bool allowRaycast;

    [SerializeField]
    private AudioClip m_audioClip;

    private CoinImage[] m_coinImageInstances;

    private int m_tweenCompleteCount;
    private int m_createdCount;
    private bool m_isAllCoinsCreated;

    private int m_addCount;


    /// <summary> 是否所有币都创建完成（未开始收集动画之前） </summary>
    public bool isAllCoinsCreated => m_isAllCoinsCreated;

    /// <summary> 设置增加金币的数量 </summary>
    public void SetAddCount (int value) {
        m_addCount = value;
    }

    /// <summary>
    /// 开始移动到目标点动画（必须 <see cref="stopOnCreated"/> 和 <see cref="m_isAllCoinsCreated"/> 为 true 时才能调用此方法）
    /// </summary>
    public void StartMoveToTargetAnimation () {
        if (!stopOnCreated) return;
        if (!m_isAllCoinsCreated) {
            Debug.LogError("必须所有币创建完成才能调用此方法");
            return;
        }

        for (int i = 0; i < coinImageCount; i++) {
            CoinImage coinImage = m_coinImageInstances[i];
            coinImage.StartMoveToTargetAnimation();
        }

        if (m_audioClip) {
            App.instance.audioManager.PlayEffect(m_audioClip, Camera.main.transform);
        }
    }

    private void Awake () {
        m_tweenCompleteCount = 0;
        coinImagePrefab.SetActive(false);
    }

    private void Start () {
        if (!stopOnCreated) {
            if (m_audioClip) {
                App.instance.audioManager.PlayEffect(m_audioClip, Camera.main.transform);
            }
        }

        Image imageBackground = GetComponent<Image>();
        imageBackground.enabled = allowRaycast;

        CreateCoinImages();
    }

    private void CreateCoinImages () {
        m_coinImageInstances = new CoinImage[coinImageCount];
        Transform parent = coinImagePrefab.transform.parent;

        for (int i = 0; i < coinImageCount; i++) {
            GameObject inst = Instantiate(coinImagePrefab, parent);
            inst.SetActive(true);
            inst.transform.SetAsFirstSibling();

            CoinImage coinImage = inst.GetComponent<CoinImage>();
            coinImage.isStopOnCreated = stopOnCreated;
            coinImage.delayOnCreatedTime = createIntervalTime * i;
            coinImage.targetPointRectTransform = targetPointRectTransform;
            coinImage.onCompleteEvent += OnCoinCreated;
            coinImage.onCompleteEvent += OnCoinTweenComplete;

            m_coinImageInstances[i] = coinImage;
        }
    }

    /// <summary>
    /// 一个币创建完成（未开始收集动画之前）
    /// </summary>
    private void OnCoinCreated () {
        m_createdCount++;

        float progress = (float)m_createdCount / coinImageCount;

        onCreatedEvent?.Invoke(this, progress);

        // 所有币创建完成
        if (progress >= 1f) {
            m_isAllCoinsCreated = true;
            onAllCreatedEvent?.Invoke(this);
        }
    }

    /// <summary>
    /// 一个币收集动画完成
    /// </summary>
    private void OnCoinTweenComplete () {
        m_tweenCompleteCount++;

        float progress = (float)m_tweenCompleteCount / coinImageCount;

        // 计算一个金币收集完成时，实际所需要增加金币的数量
        int average = m_addCount / coinImageCount;
        int remainder = m_addCount % coinImageCount;
        int addCount = progress >= 1f ? average + remainder : average;

        onCompleteEvent?.Invoke(this, progress, addCount);

        // 所有币收集动画完成
        if (progress >= 1f) {
            onAllCompleteEvent?.Invoke(this);
            Destroy(gameObject);
        }
    }

    private void OnDestroy () {
        for (int i = 0; i < coinImageCount; i++) {
            CoinImage coinImage = m_coinImageInstances[i];
            if (!coinImage) continue;
            coinImage.onCompleteEvent -= OnCoinTweenComplete;
        }
    }

}