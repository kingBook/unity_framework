using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[DisallowMultipleComponent]
public abstract class LoopHorizontalOrVerticalLayout : MonoBehaviour {

    protected class ScaleValueComparer : IComparer {
        int IComparer.Compare(object x, object y) {
            var a = ((RectTransform rectTransform, float scaleValue))x;
            var b = ((RectTransform rectTransform, float scaleValue))y;
            return (int)Mathf.Sign(a.scaleValue - b.scaleValue);
        }
    }

    [Tooltip("如果长度大于0，那么布局列表中的子对象，更改子对象的层级不影响布局顺序，想改变布局顺序只能更改此列表。此列表长度为0时，则根据子对象的层级顺序进行布局")]
    public List<RectTransform> childrenOrder = new List<RectTransform>();
    [Tooltip("初始时聚焦的索引，范围区间: [0, 子对象的总数-1]")]
    public int initFocusIndex;

    protected readonly ScaleValueComparer m_scaleValueComparer = new ScaleValueComparer();
    protected RectTransform[] m_childrenOrderRecord;


    protected void InitOrderListWithChildren() {
        if (childrenOrder.Count <= 0) {
            for (int i = 0, len = transform.childCount; i < len; i++) {
                var child = transform.GetChild(i);
                childrenOrder.Add((RectTransform)child);
            }
        }

        if (m_childrenOrderRecord == null) {
            m_childrenOrderRecord = childrenOrder.ToArray();
        }

    }

    protected virtual void Awake() {

    }

    protected virtual void OnEnable() {

    }

    protected virtual void Update() {

    }

    protected virtual void OnDisable() {

    }

    protected virtual void OnTransformChildrenChanged() {

    }

#if UNITY_EDITOR
    protected virtual void Reset() {

    }

    protected virtual void OnValidate() {

    }
#endif
}
