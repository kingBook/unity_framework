using UnityEngine;
using System.Collections;
using System.Threading.Tasks;

public static class AdsBridge {


    #region MarkPoint
    public enum PointInfo {
        /// <summary> 开始关卡（参数 id：表示第几关） </summary>
        StartLevel = 0,
        /// <summary> 链接到其它游戏（参数 id：表示链接到哪一个游戏） </summary>
        LinkToOtherGame = 1,
        /// <summary> 点击进入小游戏按钮 </summary>
        OnClickEnterMiniGameButton = 2
    }

    /// <summary>
    /// 标记打点
    /// </summary>
    /// <param name="info"> 打点信息 </param>
    /// <param name="id"> 当多条相同的打点信息时用于区分的 Id，例如：MarkPoint(MarkInfo.StartLevel, 1/2/3...)，-1 时表示省略 </param>
    public static void MarkPoint (PointInfo info, int id = -1) {
        Debug.Log($"== MarkPoint info:{info.ToString()} id:{id}");
#if UNITY_IOS
        switch (info) {
            case PointInfo.StartLevel:
                //AnalyticsManager.LevelStart(id);
                break;
            case PointInfo.LinkToOtherGame:
                //AnalyticsManager.LinkToOtherGame(id);
                break;
            case PointInfo.OnClickEnterMiniGameButton:
                //AnalyticsManager.OnClickEnterMiniGameButton();
                break;
        }
#elif UNITY_ANDROID

#else
    
#endif
    }
    #endregion MarkPoint

    /// <summary>
    /// 游戏内播放视频广告调用
    /// </summary>
    /// <param name="onComplete"> 播放完成时调用（给予奖励） </param>
    /// <param name="onClose"> 中途关闭时调用（不给予奖励，可能为 null） </param>
    public static void ShowVideo (System.Action onComplete, System.Action onClose = null) {
        // TODO: 视频广告接口
        // onComplete.Invoke(); // 播放完成时调用（给予奖励 注意:勿重复调用多次）
        // onClose?.Invoke();   // 中途关闭时调用（不给予奖励 注意:勿重复调用多次，此回调用可能为 null）
#if UNITY_IOS
        //AdsManager.instance.ShowRewardAd(onComplete, onClose);
#elif UNITY_ANDROID
        // 模拟测试代码
        Debug.Log("== 模拟显示视频广告");
        DelayComplete(onComplete);
#else
        // 模拟测试代码
        Debug.Log("== 模拟显示视频广告");
        DelayComplete(onComplete);
#endif

    }

    /// <summary> 此方法仅用于测试 </summary>
    private static async void DelayComplete (System.Action completeCallback) {
        await Task.Delay(1000);
        Debug.Log("== 模拟看广告视频完成");
        completeCallback();
    }

    /// <summary>
    /// 展示插页式广告
    /// </summary>
    /// <param name="msg"> 说明信息 </param>
    public static void ShowInterstitialAd (string msg) {
#if UNITY_IOS
        //AdsManager.instance.ShowInterstitial();
#elif UNITY_ANDROID
        Debug.Log("== 模拟展示插页式广告: " + msg);
#else
        Debug.Log("== 模拟展示插页式广告: " + msg);
#endif
    }

    /// <summary>
    /// 点击'其他游戏'按钮时调用
    /// </summary>
    /// <param name="id"> 0: xx游戏；1：xxx游戏 </param>
    public static void LinkToOtherGame (int id) {
#if UNITY_IOS
        //UnityStoreKitMgr.Instance.ShowAppDetail("1495959578");
#elif UNITY_ANDROID
        Debug.Log("== 点击'推广其他游戏'按钮:" + id);
#else
        Debug.Log("== 点击'推广其他游戏'按钮:" + id);
#endif
    }





}
