using DG.Tweening;
using System.Collections;
using UnityEngine;

/// <summary>
/// 延迟激活激活所有子对象（挂载此脚本的父级对象必须一直处于激活）
/// </summary>
public class DelayActiveChildren : MonoBehaviour {

    public enum MethodType {
        OnEnable,
        Update,
        LateUpdate,
    }

    [Tooltip("时间（秒）")] public float time = 3;
    [Tooltip("在指定的方法执行 Disable 操作，对象必须处于激活状态")] public MethodType disableOnMethod = MethodType.OnEnable;

    private void DeactiveChildrenAndStartDelay() {
        if (IsInvoking(nameof(OnDelayed))) return;
        SetActiveChildren(false);
        Invoke(nameof(OnDelayed), time);
    }

    private void OnDelayed() {
        if (this == null) return;
        SetActiveChildren(true);
    }

    private void SetActiveChildren(bool value) {
        for (int i = 0, len = transform.childCount; i < len; i++) {
            GameObject child = transform.GetChild(i).gameObject;
            child.SetActive(value);
        }
    }

    private void OnEnable() {
        if (disableOnMethod == MethodType.OnEnable) {
            DeactiveChildrenAndStartDelay();
        }
    }

    private void Update() {
        if (disableOnMethod == MethodType.Update) {
            DeactiveChildrenAndStartDelay();
        }
    }

    private void LateUpdate() {
        if (disableOnMethod == MethodType.LateUpdate) {
            DeactiveChildrenAndStartDelay();
        }
    }

    private void OnDisable() {
        CancelInvoke(nameof(OnDelayed));
    }
}
