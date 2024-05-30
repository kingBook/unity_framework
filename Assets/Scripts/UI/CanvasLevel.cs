using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CanvasLevel : MonoBehaviour {

    public DirectionInput directionInput;
    public KeyInput keyInput;
    public PanelInfo panelInfo;
    public PanelFullScreenNotice panelFullScreenNotice;

    [Space]
    [SerializeField] private AudioClip m_clickAudioClip;
    [SerializeField, Tooltip("金币一个个飞向目标点的动画预制件")] private AddMoneysEffect m_addMoneysEffectPrefab;
    [SerializeField, Tooltip("多个金币飞向目标点的动画预制件")] private GetCoinsEffect m_getCoinsEffectPrefab;

    private Game m_game;

    /// <summary>
    /// 创建金币一个个飞向目标点的动画特效
    /// </summary>
    /// <param name="originPos"> 金币的起始位置 </param>
    /// <param name="targetPointRectTransform"> 金币飞向的目标点位置 </param>
    /// <param name="allowRaycast"> 允许射线投射计算（UI 按钮在动画结束前是否能被点击 </param>
    /// <param name="addCount"> 增加的金币数量 </param>
    /// <param name="onComplete"> 所有金币都飞向目标点后的回调 </param>
    public AddMoneysEffect CreateAddMoneysEffect(Vector3 originPos, RectTransform targetPointRectTransform, bool allowRaycast, int addCount, System.Action onComplete = null) {
        AddMoneysEffect effect = Instantiate(m_addMoneysEffectPrefab, m_addMoneysEffectPrefab.transform.parent);
        effect.gameObject.SetActive(true);
        effect.transform.SetSiblingIndex(m_addMoneysEffectPrefab.transform.GetSiblingIndex() + 1);
        effect.transform.position = originPos;

        effect.targetPointRectTransform = targetPointRectTransform;
        effect.allowRaycast = allowRaycast;
        effect.SetAddCount(addCount);
        effect.onCompleteEvent += (AddMoneysEffect addMoneysEffect, float progress, int addCount2) => {
            if (this && m_game) {
                LocalManager.SetMoneyCount(LocalManager.GetMoneyCount() + addCount2);
                if(progress >= 1f) {
                    PlayerPrefs.Save();
                }
            }

            if (progress >= 1f) {
                onComplete?.Invoke();
            }
        };
        return effect;
    }

    /// <summary>
    /// 创建多个金币飞向目标点的动画特效
    /// </summary>
    /// <param name="originPos"> 金币的起始位置 </param>
    /// <param name="targetPointRectTransform"> 金币飞向的目标点位置 </param>
    /// <param name="allowRaycast"> 允许射线投射计算（UI 按钮在动画结束前是否能被点击 </param>
    /// <param name="addCount"> 增加的金币数量 </param>
    /// <param name="onInitCoinImage"> 用于初始化 <see cref="CoinImage"/>的 radiusFirst(创建多个金币的初始半径) 和 radiusSecond(多个金币向外张开的半径) </param>
    /// <param name="onComplete"> 所有金币都飞向目标点后的回调 </param>
    /// <returns></returns>
    public GetCoinsEffect CreateGetCoinsEffect(Vector3 originPos, RectTransform targetPointRectTransform, bool allowRaycast, int addCount, System.Action<CoinImage> onInitCoinImage = null, System.Action onComplete = null) {
        GetCoinsEffect effect = Instantiate(m_getCoinsEffectPrefab, m_getCoinsEffectPrefab.transform.parent);
        effect.transform.SetSiblingIndex(m_getCoinsEffectPrefab.transform.GetSiblingIndex() + 1);
        effect.transform.position = originPos;

        CoinImage coinImage = effect.GetComponentInChildren<CoinImage>();
        onInitCoinImage?.Invoke(coinImage);

        effect.gameObject.SetActive(true);

        effect.targetPointRectTransform = targetPointRectTransform;
        effect.allowRaycast = allowRaycast;
        effect.SetAddCount(addCount);
        effect.onCompleteEvent += (GetCoinsEffect getCoinsEffect, float progress, int addCount2) => {
            if (this && m_game) {
                LocalManager.SetMoneyCount(LocalManager.GetMoneyCount() + addCount2);
                if (progress >= 1f) {
                    PlayerPrefs.Save();
                }
            }

            if (progress >= 1f) {
                onComplete?.Invoke();
            }
        };
        return effect;
    }

    public void OnClick(Vector2 screenPoint) {
        if (EventSystem.current && EventSystem.current.currentSelectedGameObject) {
            bool isButton = EventSystem.current.currentSelectedGameObject.GetComponent<Button>() != null;
            if (isButton && m_clickAudioClip) {
                App.instance.audioManager.PlayEffect(m_clickAudioClip, Camera.main.transform.position);
            }
        }
    }


    private void Start() {
        m_game = App.instance.GetGame<Game>();
    }

    private void Update() {
        if (InputUtil.GetTouchBeganScreenPoint(false, out Vector3 screenPoint, out int fingerId)) {
            OnClick(screenPoint);
        }
    }


}
