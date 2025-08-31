using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 振动器
/// </summary>
public class Vibrator : MonoBehaviour {

    private void Awake() {
        Vibration.Init();
    }

    /// <summary>
    /// 默认振动 ~400 毫秒
    /// </summary>
    public void Vibrate() {
        Vibration.Vibrate();
    }

    /// <summary>
    /// 手机弱振动（iOS：仅在配备Haptic引擎的iOS上可用。最低iPohne 6s或Android）
    /// </summary>
    public void VibratePop() {
#if UNITY_IOS
        Vibration.VibratePop();
#elif UNITY_ANDROID
        Vibration.Vibrate(20);
#endif
    }

    /// <summary>
    /// 强振动（iOS：仅在配备Haptic引擎的iOS上可用。最低iPohne 6s或Android）
    /// </summary>
    public void VibratePeek() {
        Vibration.VibratePeek();
    }

    /// <summary>
    /// 连续三次弱振动（iOS：仅在配备Haptic引擎的iOS上可用。最低iPohne 6s或Android）
    /// </summary>
    public void VibrateNope() {
        Vibration.VibrateNope();
    }

    /// <summary>
    /// 自定义持续时间（毫秒）振动（不支持iOS, 仅支持Android）
    /// </summary>
    /// <param name="milliseconds"></param>
    public void Vibrate(long milliseconds) {
        Vibration.Vibrate();
    }

    /// <summary>
    /// 自定义模式振动（不支持iOS, 仅支持Android）
    /// <code>
    /// long[] pattern = { 0, 1000, 1000, 1000, 1000 };
    /// Vibrator.Vibrate ( pattern, -1 );
    /// </code>
    /// </summary>
    /// <param name="pattern"> 模式数组 </param>
    /// <param name="repeat"> 重复次数 </param>
    public void Vibrate(long[] pattern, int repeat) {
        Vibration.Vibrate(pattern, repeat);
    }

}
