using DG.Tweening;
using System.Collections;
using UnityEngine;

/// <summary>
/// 延迟激活器（应用的对象必须处于激活状态）
/// </summary>
public class DelayActiver : MonoBehaviour {

    public enum MethodType {
        Start,
        Update
    }

    [Tooltip("时间（秒）")] public float time = 5;
    [Tooltip("在指定的方法执行 Disable 操作，对象必须处于激活状态")] public MethodType disableOnMethod = MethodType.Start;

    private void DisableAndStartDelay () {
        App.instance.Delay(time, this, ActiveSelf);

        gameObject.SetActive(false);
    }

    private void ActiveSelf () {
        if (this == null) return;
        gameObject.SetActive(true);
    }

    private void Start () {
        if (disableOnMethod == MethodType.Start) {
            DisableAndStartDelay();
        }
    }

    private void Update () {
        if (disableOnMethod == MethodType.Update) {
            DisableAndStartDelay();
        }
    }
}
