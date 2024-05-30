using System.Collections;
using UnityEngine;

public static class LocalManager {

    /// <summary> 返回关卡数字 </summary>
    public static int GetLevelNumber() {
        return PlayerPrefs.GetInt("LevelNumber", 0);
    }

    /// <summary> 设置关上写数字 </summary>
    public static void SetLevelNumber(int value) {
        PlayerPrefs.SetInt("LevelNumber", value);
    }

    /// <summary> 获取金钱数量 </summary>/
    public static int GetMoneyCount() {
        return PlayerPrefs.GetInt("MoneyCount", 0);
    }

    /// <summary> 设置金钱的数量 </summary>/
    public static void SetMoneyCount(int value) {
        PlayerPrefs.SetInt("MoneyCount", value);
    }
}
