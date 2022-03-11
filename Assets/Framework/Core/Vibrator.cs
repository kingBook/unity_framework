using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 震动器
/// </summary>
public class Vibrator : MonoBehaviour {

    private void Awake () {
        Vibration.Init();
    }

    /// <summary> 手机弱震动 </summary>
    public void VibratePop () {
#if UNITY_IOS
        Vibration.VibratePop();
#elif UNITY_ANDROID
        Vibration.Vibrate(20);
#endif
    }
}
