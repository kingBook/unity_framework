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
        LevelFailure
    }

    #region Test
    /// <summary> 延时 (此方法仅用于测试) </summary>
    private static async void Delay(System.Action callback) {
        await Task.Delay(1000);
        Debug.Log("== Test display AD completed");
        callback();
    }
    #endregion

    /// <summary>
    /// 标记打点
    /// </summary>
    /// <param name="info"> 打点信息 </param>
    /// <param name="id"> 当多条相同的打点信息时用于区分的 Id，例如：MarkPoint(MarkInfo.LevelStart, 1/2/3...)，-1 时表示省略 </param>
    public static void MarkPoint(PointInfo info, int id = -1) {
        // 使用 info.ToString() 获取字符串 Key
        Debug.Log($"== MarkPoint: info:{info.ToString()}, id:{id} ");

        switch (info) {
            case PointInfo.LevelStart:
                break;
            case PointInfo.LevelVictory:
                break;
            case PointInfo.LevelFailure:
                break;
        }
    }

    /// <summary>
    /// 展示激励视频广告
    /// </summary>
    /// <param name="onUserEarnedReward"> 播放完成给予奖励时调用（不允许 null） </param>
    /// <param name="onAdClosed"> 中途关闭时调用（不给予奖励，可能为 null） </param>
    /// <param name="onAdOpening"> 成功加载并开始显示时调用（用于播放广告时需要暂停游戏，可能为 null） </param>
    public static void ShowRewardAd(System.Action onUserEarnedReward, System.Action onAdClosed = null, System.Action onAdOpening = null) {
        // TODO: 激励视频广告接口
        // onUserEarnedReward.Invoke(); // 注意:勿重复多次调用
        // onAdClosed?.Invoke();        // 注意:勿重复多次调用，此回调可能为 null
        // onAdOpening?.Invoke();       // 注意:勿重复多次调用，此回调可能为 null

        // 模拟测试代码
        onAdOpening?.Invoke();
        Delay(onUserEarnedReward);
    }

    /// <summary>
    /// 展示插屏广告
    /// </summary>
    /// <param name="onAdClosed"> 关闭时调用 (可能为 null) </param>
    /// <param name="onAdOpening"> 成功加载并开始显示时调用 (可能为 null) </param>
    public static void ShowInterstitialAd(System.Action onAdClosed = null, System.Action onAdOpening = null) {
        // TODO: 插屏广告接口
        // onAdClosed?.Invoke();        // 注意:勿重复多次调用，此回调可能为 null
        // onAdOpening?.Invoke();       // 注意:勿重复多次调用，此回调可能为 null

        // 模拟测试代码
        onAdOpening?.Invoke();
        if (onAdClosed != null) {
            Delay(onAdClosed);
        }
    }
	
	/// <summary>
    /// 播放横幅广告
    /// </summary>
    /// <param name="baseLine"> 0：顶部对齐;1：底部对齐 </param>
    /// <param name="offset"> 距离基准位置 px </param>
    public static void ShowBarnner(int baseLine, int offset = 0)  {
        
    }

    /// <summary>
    /// 链接到'其他游戏'
    /// </summary>
    /// <param name="id"> 0: xx游戏；1：xxx游戏 </param>
    public static void LinkToOtherGame(int id) {
        Debug.Log("== Click the link to other games button:" + id);

    }
}
