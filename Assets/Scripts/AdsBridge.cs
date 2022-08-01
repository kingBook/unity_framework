using UnityEngine;
using System.Collections;
using System.Threading.Tasks;

public static class AdsBridge {


    /// <summary>
    /// 所有使用到的打点信息的枚举
    /// </summary>
    public enum PointInfo {
        /// <summary> 开始关卡（id：关卡编号） </summary>
        LevelStart,
        /// <summary> 关卡胜利（id：关卡编号） </summary>
        LevelVictory,
        /// <summary> 关卡失败（id：关卡编号） </summary>
        LevelFailure,
        /// <summary> 点击进入小游戏按钮 </summary>
        OnClickButtonEnterMiniGame
    }

    /// <summary>
    /// 标记打点
    /// </summary>
    /// <param name="info"> 打点信息 </param>
    /// <param name="id"> 当多条相同的打点信息时用于区分的 Id，例如：MarkPoint(MarkInfo.LevelStart, 1/2/3...)，-1 时表示省略 </param>
    public static void MarkPoint(PointInfo info, int id = -1) {
        // 使用 info.ToString() 获取字符串 Key
        Debug.Log($"== 标记打点: info:{info.ToString()}, id:{id} ");
#if UNITY_IOS
        switch (info) {
            case PointInfo.LevelStart:
                //AnalyticsManager.LevelStart(id);
                break;
            case PointInfo.OnClickButtonEnterMiniGame:
                //AnalyticsManager.OnClickButtonEnterMiniGame();
                break;
        }
#elif UNITY_ANDROID

#else
        
#endif
    }


    /// <summary>
    /// 展示激励视频广告
    /// </summary>
    /// <param name="onUserEarnedReward"> 播放完成给予奖励时调用（不允许 null） </param>
    /// <param name="OnAdClosed"> 中途关闭时调用（不给予奖励，可能为 null） </param>
    /// <param name="OnAdOpening"> 成功加载并开始显示时调用（用于播放广告时需要暂停游戏，可能为 null） </param>
    public static void ShowRewardAd(System.Action onUserEarnedReward, System.Action OnAdClosed = null, System.Action OnAdOpening = null) {
        // TODO: 视频广告接口
        // onUserEarnedReward.Invoke(); // 注意:勿重复多次调用
        // OnAdClosed?.Invoke();        // 注意:勿重复多次调用，此回调可能为 null
        // OnAdOpening?.Invoke();       // 注意:勿重复多次调用，此回调可能为 null
#if UNITY_IOS
        //AdsManager.instance.ShowRewardAd(onUserEarnedReward, OnAdClosed, OnAdOpening);
#elif UNITY_ANDROID
        // 模拟测试代码
        OnAdOpening?.Invoke();
        Delay(onUserEarnedReward);
#else
        // 模拟测试代码
        OnAdOpening?.Invoke();
        Delay(onUserEarnedReward);
#endif
    }

    /// <summary> 此方法仅用于测试 </summary>
    private static async void Delay(System.Action callback) {
        await Task.Delay(1000);
        Debug.Log("== 模拟展示激励视频广告完成");
        callback();
    }


    /// <summary>
    /// 展示插页式广告
    /// </summary>
    /// <param name="msg"> 说明信息 </param>
    public static void ShowInterstitialAd(string msg) {
        Debug.Log("== 模拟展示插页式广告: " + msg);
#if UNITY_IOS
        //AdsManager.instance.ShowInterstitial();
#elif UNITY_ANDROID

#else
        
#endif
    }


    /// <summary>
    /// 链接到'其他游戏'
    /// </summary>
    /// <param name="id"> 0: xx游戏；1：xxx游戏 </param>
    public static void LinkToOtherGame(int id) {
        Debug.Log("== 点击'推广其他游戏'按钮:" + id);
#if UNITY_IOS
        //UnityStoreKitMgr.Instance.ShowAppDetail("1495959578");
#elif UNITY_ANDROID

#else
        
#endif
    }
}
