using System.Collections;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 金币一个个飞向目标点动画
/// 
/// <code>
/// GameObject inst = Instantiate(m_prefab, m_prefab.transform.parent);
/// inst.SetActive(true);
///
/// AddMoneysEffect effect = inst.GetComponent<GetCoinsEffect>();
/// effect.allowRaycast = false;
/// effect.SetAddCount(100);
/// effect.onCompleteEvent += (AddMoneysEffect addMoneysEffect, float progress, int addCount) => {
///     m_game.goldCount += addCount;
/// };
/// </code>
/// 
/// </summary>
public class AddMoneysEffect : MonoBehaviour {

    /// <summary> 一个币收集动画完成（飞到目标点），格式：<code> void (AddMoneysEffect addMoneysEffect, float progress, int addCount) </code> </summary>
    public event System.Action<AddMoneysEffect, float, int> onCompleteEvent;

    /// <summary> 所胡币收集动画完成（飞到目标点），格式：<code> void (AddMoneysEffect addMoneysEffect) </code> </summary>
    public event System.Action<AddMoneysEffect> onAllCompleteEvent;

    [Tooltip("创建币的数量")]
    public int coinImageCount = 20;

    [Tooltip("创建间隔时间 <秒>")]
    public float createIntervalTime = 0.1f;

    [Tooltip("创建出币后收集聚拢的目标点")]
    public RectTransform targetPointRectTransform;

    [Tooltip("币预制件")]
    public GameObject coinImagePrefab;

    [Tooltip("允许射线投射计算（UI 按钮在动画结束前是否能被点击）")]
    public bool allowRaycast;

    [SerializeField, Tooltip("创建出一个币的音效")]
    private AudioClip m_audioClip;

    private MoneyImage[] m_coinImageInstances;

    private int m_tweenCompleteCount;
    private int m_createdCount;

    private int m_addCount;

    /// <summary> 设置增加金币的数量 </summary>
    public void SetAddCount(int value) {
        m_addCount = value;

        // 实际增加数不能小于实例数，否则会出现一个金币飞到目标点后增加的加+0的情况
        if (m_addCount < coinImageCount) {
            coinImageCount = m_addCount;
        }
    }

    private void OnCreateTimer() {
        GameObject inst = Instantiate(coinImagePrefab, coinImagePrefab.transform.parent);
        MoneyImage moneyImage = inst.GetComponent<MoneyImage>();
        moneyImage.targetPointRectTransform = targetPointRectTransform;
        moneyImage.onCompleteEvent += OnCoinTweenComplete;
        inst.SetActive(true);

        if (m_audioClip) {
            App.instance.audioManager.PlayEffect(m_audioClip, Camera.main.transform);
        }

        App.instance.vibrator.VibratePop();

        m_createdCount++;
        if (m_createdCount >= coinImageCount) {
            CancelInvoke(nameof(OnCreateTimer));
        }
    }

    /// <summary>
    /// 一个币收集动画完成
    /// </summary>
    private void OnCoinTweenComplete() {
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

    private void Awake() {
        m_tweenCompleteCount = 0;
        coinImagePrefab.SetActive(false);
    }

    private void Start() {
        Image imageBackground = GetComponent<Image>();
        imageBackground.enabled = !allowRaycast;

        if (m_addCount > 0) {
            InvokeRepeating(nameof(OnCreateTimer), createIntervalTime, createIntervalTime);
        } else {
            // <=0时，直接完成
            onCompleteEvent?.Invoke(this, 1f, m_addCount);
            onAllCompleteEvent?.Invoke(this);
            Destroy(gameObject);
        }
    }
}
