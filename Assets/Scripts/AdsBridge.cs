using UnityEngine;
using System.Collections;
using System.Threading.Tasks;

public static class AdsBridge {


    /// <summary>
    /// 游戏内播放视频广告调用
    /// </summary>
    /// <param name="completeCallback"> 播放完成时调用（给予奖励） </param>
    /// <param name="closeCallback"> 中途关闭调用（不给予奖励，可能为 null） </param>
    public static void ShowVideo (System.Action completeCallback, System.Action closeCallback = null) {
        //TODO: 视频广告接口
        //completeCallback.Invoke(); // 播放完成时调用（给予奖励 注意:勿重复调用多次）
        //closeCallback?.Invoke();   // 中途关闭调用（不给予奖励 注意:勿重复调用多次，此回调用可能为 null）
#if UNITY_IOS
        //AdsManager.instance.ShowRewardAd(completeCallback, closeCallback);
#else
        // 模拟测试代码
        Debug.Log("== 模拟显示视频广告");
        DelayComplete(completeCallback);
#endif

    }

    /// <summary>
    /// 展示插页式广告
    /// </summary>
    /// <param name="msg"> 说明信息 </param>
    public static void ShowInterstitialAd (string msg) {
#if UNITY_IOS
        //AdsManager.instance.ShowInterstitial();
#else
        Debug.Log("== 模拟展示插页式广告: " + msg);
#endif
    }

    /// <summary>
    /// 点击'其他游戏'按钮时调用
    /// </summary>
    /// <param name="id"> 0: 最强特工；1：机械龙</param>
    public static void LinkToOtherGame (int id) {
#if UNITY_IOS
        //UnityStoreKitMgr.Instance.ShowAppDetail("1495959578");
#else
        Debug.Log("== 点击'推广其他游戏'按钮:" + id);
#endif
    }

    /// <summary>
    /// 标记打点
    /// </summary>
    /// <param name="msg">打点信息</param>
    public static void MarkPoint (string msg, string name, int arg0, int arg1) {
        //Debug.Log("MarkPoint: "+msg);
#if UNITY_IOS
        /*switch (name)
        {
            case "levelStart":
                AnalyticsManager.LevelStart(arg0);
                break;
            case "levelComplete":
                AnalyticsManager.LevelEnd();
                if (arg0 == 3 || arg0==6 || arg0==12)
                {
                    UnityStoreKitMgr.Instance.GoToCommnet();
                }
                break;
            case "levelFailed":
                AnalyticsManager.LevelEnd();
                break;
            case "getRole"://点击每过2关 角色界面领取角色按钮
                //AnalyticsManager.GetRole();
                break;
            case "getEndRole"://点击过关失败界面的领取角色按钮

                break;
            case "getCoin"://点击直接领取金币

                break;
            case "getCoinBag"://点击钱袋
                AnalyticsManager.GetCoinBag();
                break;
            case "linkGame"://点击打开推荐游戏
                AnalyticsManager.LinkGame();
                break;
            case "openBox"://点击右上角宝箱
                AnalyticsManager.OpenBox();
                break;
            case "ltyBox"://点击'宝箱抽奖'的'领取奖励'按钮

                break;
            case "getBoxKey"://点击'宝箱抽奖'的'钥匙+1'按钮

                break;
            case "skipLevel"://点击跳过关卡按钮
                AnalyticsManager.SkipLevel();
                break;
            case "clickItem"://点击商店物品
                AnalyticsManager.ClickItem(arg0,arg1);
                break;
            case "getShopCoin":

                break;
        }*/
#else

#endif
    }


    /// <summary> 此方法仅用于测试 </summary>
    private static async void DelayComplete (System.Action completeCallback) {
        await Task.Delay(1000);
        Debug.Log("== 模拟看广告视频完成");
        completeCallback();
    }
}
