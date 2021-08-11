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
        OnClickEnterMiniGameButton
    }

    /// <summary>
    /// 标记打点
    /// </summary>
    /// <param name="info"> 打点信息 </param>
    /// <param name="id"> 当多条相同的打点信息时用于区分的 Id，例如：MarkPoint(MarkInfo.LevelStart, 1/2/3...)，-1 时表示省略 </param>
    public static void MarkPoint (PointInfo info, int id = -1) {
        // 使用 info.ToString() 获取字符串 Key
        Debug.Log($"== 标记打点 {{ info:{info.ToString()}, id:{id} }}");
#if UNITY_IOS
        switch (info) {
            case PointInfo.LevelStart:
                //AnalyticsManager.LevelStart(id);
                break;
            case PointInfo.OnClickEnterMiniGameButton:
                //AnalyticsManager.OnClickEnterMiniGameButton();
                break;
        }
#elif UNITY_ANDROID

#else
        
#endif
    }


    /// <summary>
    /// 展示激励视频广告
    /// </summary>
    /// <param name="onComplete"> 播放完成时调用（给予奖励） </param>
    /// <param name="onClose"> 中途关闭时调用（不给予奖励，可能为 null） </param>
    public static void ShowRewardAd (System.Action onComplete, System.Action onClose = null) {
        // TODO: 视频广告接口
        // onComplete.Invoke(); // 播放完成时调用（给予奖励 注意:勿重复调用多次）
        // onClose?.Invoke();   // 中途关闭时调用（不给予奖励 注意:勿重复调用多次，此回调用可能为 null）
#if UNITY_IOS
        //AdsManager.instance.ShowRewardAd(onComplete, onClose);
#elif UNITY_ANDROID
        // 模拟测试代码
        DelayComplete(onComplete);
#else
        // 模拟测试代码
        DelayComplete(onComplete);
#endif
    }

    /// <summary> 此方法仅用于测试 </summary>
    private static async void DelayComplete (System.Action completeCallback) {
        await Task.Delay(1000);
        Debug.Log("== 模拟展示激励视频广告完成");
        completeCallback();
    }


    /// <summary>
    /// 展示插页式广告
    /// </summary>
    /// <param name="msg"> 说明信息 </param>
    public static void ShowInterstitialAd (string msg) {
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
    public static void LinkToOtherGame (int id) {
        Debug.Log("== 点击'推广其他游戏'按钮:" + id);
#if UNITY_IOS
        //UnityStoreKitMgr.Instance.ShowAppDetail("1495959578");
#elif UNITY_ANDROID

#else
        
#endif
    }
}
