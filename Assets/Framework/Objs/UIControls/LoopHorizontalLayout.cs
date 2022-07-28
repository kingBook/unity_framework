
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

[ExecuteInEditMode]
public class LoopHorizontalLayout : LoopHorizontalOrVerticalLayout {

    [Space]
    public float spacing;
    public bool isScaleChildren = true;
    [Range(0.1f, 1.0f)] public float minScale = 0.5f;
    [Range(1.0f, 5.0f)] public float maxScale = 1.0f;

    [Tooltip("当 child 超出视口范围，移动 child 到左边缘或右边缘，此值调整需要超出多少时才移动 (局部坐标)")]
    [SerializeField] private float m_offsetBeyondViewport;
    [SerializeField] private ScrollRect m_scrollRect;

    private float m_startX;
    private (RectTransform rectTransform, float scaleValue)[] m_siblingList;

    public float offsetBeyondViewport {
        get {
            return m_offsetBeyondViewport;
        }
        set {
            if (value < 0) {
                Debug.LogWarning(nameof(m_offsetBeyondViewport) + " 不能小于0");
            }
            m_offsetBeyondViewport = Mathf.Max(0f, value);
        }
    }


    private void UpdateLayout() {
        RectTransform content = (RectTransform)transform;
        Rect contentRect = content.rect;

        float x = m_startX;
        float y = contentRect.height * 0.5f;

        if (childrenOrder.Count > 0) {
            // 根据 childrenOrder 顺序布局
            for (int i = 0, len = childrenOrder.Count; i < len; i++) {
                RectTransform child = childrenOrder[i];
                if (!child || !child.gameObject.activeSelf) continue;

                float halfWidth = child.sizeDelta.x * 0.5f;
                if (i == 0) x += spacing;
                x += halfWidth;
                child.anchoredPosition = new Vector2(x, y);
                x += halfWidth + spacing;
            }
        } else {
            // 根据子象层级顺序布局
            for (int i = 0, len = transform.childCount; i < len; i++) {
                RectTransform child = transform.GetChild(i) as RectTransform;
                if (!child || !child.gameObject.activeSelf) continue;

                float halfWidth = child.sizeDelta.x * 0.5f;
                if (i == 0) x += spacing;
                x += halfWidth;
                child.anchoredPosition = new Vector2(x, y);
                x += halfWidth + spacing;
            }
        }

        Vector2 contentSizeDelta = content.sizeDelta;
        contentSizeDelta.x = x - m_startX;
        content.sizeDelta = contentSizeDelta;
    }

    private void MoveEndChildToStart(int count) {
        if (childrenOrder.Count > 0) {
            RectTransform lastChild = childrenOrder[childrenOrder.Count - 1];
            childrenOrder.RemoveAt(childrenOrder.Count - 1);
            childrenOrder.Insert(0, lastChild);

            RectTransform content = (RectTransform)transform;
            Vector2 pivot = content.pivot;
            pivot.x += (spacing + lastChild.sizeDelta.x) / content.sizeDelta.x;
            content.pivot = pivot;

            UpdateLayout();
        }
    }

    private void MoveStartChildToEnd(int count) {
        if (childrenOrder.Count > 0) {
            RectTransform firstChild = childrenOrder[0];
            childrenOrder.RemoveAt(0);
            childrenOrder.Add(firstChild);

            RectTransform content = (RectTransform)transform;
            Vector2 pivot = content.pivot;
            pivot.x -= (spacing + firstChild.sizeDelta.x) / content.sizeDelta.x;
            content.pivot = pivot;

            UpdateLayout();
        }
    }

    private void UpdateScaleAndSibling() {
        if (!m_scrollRect || !m_scrollRect.viewport || !m_scrollRect.content) return;

        Rect viewportRect = m_scrollRect.viewport.rect;
        if (viewportRect.width == 0f || viewportRect.height == 0f) return;

        Vector2 viewPortCenter = viewportRect.center;
        // 计算缩放
        int len = m_scrollRect.content.childCount;
        if (m_siblingList == null || len != m_siblingList.Length) {
            m_siblingList = new (RectTransform rectTransform, float scale)[len];
        }
        for (int i = 0; i < len; i++) {
            RectTransform child = (RectTransform)m_scrollRect.content.GetChild(i);
            RectTransformUtility.ScreenPointToLocalPointInRectangle(m_scrollRect.viewport, child.position, null, out Vector2 localPoint);
            float distanceCenter = Vector2.Distance(viewPortCenter, localPoint);
            float t = 1f - Mathf.Clamp01(distanceCenter / viewportRect.height);
            float scaleValue = Mathf.Lerp(minScale, maxScale, t);
            m_siblingList[i] = (child, scaleValue);
            if (isScaleChildren) {
                child.localScale = Vector3.one * scaleValue;
            }
        }
        // 更改层级顺序
        System.Array.Sort(m_siblingList, m_scaleValueComparer);
        for (int i = 0; i < len; i++) {
            RectTransform child = m_siblingList[i].rectTransform;
            child.SetSiblingIndex(i);
        }
    }

    private void OnScrollRectValueChangedHandler(Vector2 scrollValue) {
        if (!m_scrollRect || !m_scrollRect.viewport) return;
        if (childrenOrder.Count <= 0) return;

        Rect viewportRect = m_scrollRect.viewport.rect;
        if (viewportRect.width == 0f || viewportRect.height == 0f) return;

        CheckAndMoveChildren(m_scrollRect.velocity, viewportRect);

    }

    private void CheckAndMoveChildren(Vector2 velocity, Rect viewportRect) {
        int len = childrenOrder.Count;
        if (velocity.x > 0f) {
            // Content 右移
            int i = len;
            int needMoveCount = 0;
            while (--i >= 0) {
                RectTransform child = childrenOrder[i];
                Bounds bounds = RectTransformUtility.CalculateRelativeRectTransformBounds(m_scrollRect.viewport, child); // 计算出 child 在 viewport 的局部坐标系的包围盒
                if (bounds.min.x > viewportRect.xMax + offsetBeyondViewport) {
                    needMoveCount++;
                } else {
                    break;
                }
                if (needMoveCount > 0) {
                    MoveEndChildToStart(needMoveCount);
                }
            }
        } else if (velocity.x < 0f) {
            // Content 左移
            int needMoveCount = 0;
            for (int i = 0; i < len; i++) {
                RectTransform child = childrenOrder[i];
                Bounds bounds = RectTransformUtility.CalculateRelativeRectTransformBounds(m_scrollRect.viewport, child); // 计算出 child 在 viewport 的局部坐标系的包围盒
                if (bounds.max.x < viewportRect.xMin - offsetBeyondViewport) {
                    needMoveCount++;
                } else {
                    break;
                }
            }
            if (needMoveCount > 0) {
                MoveStartChildToEnd(needMoveCount);
            }

        }
    }

    private void FocusOn(RectTransform child) {
        RectTransform contentRect = m_scrollRect.content;

        Vector2 offset = m_scrollRect.transform.position - child.position;

        Vector2 pos = contentRect.position;
        pos.x += offset.x;
        contentRect.position = pos;

        Rect viewportRect = m_scrollRect.viewport.rect;
        if (viewportRect.width > 0f && viewportRect.height > 0f) {
            CheckAndMoveChildren(offset, viewportRect);
        }
    }

    private IEnumerator OnFirstFrameEnd() {
        yield return null;
        FocusOn(m_childrenOrderRecord[initFocusIndex]);
    }


    protected override void OnEnable() {
        if (Application.isPlaying) {
            if (m_scrollRect) {
                m_scrollRect.onValueChanged.AddListener(OnScrollRectValueChangedHandler);
            }
        }

        InitOrderListWithChildren();

        StartCoroutine(OnFirstFrameEnd());
    }

    protected override void Update() {
        if (Application.isPlaying) {
            UpdateScaleAndSibling();
        } else {
            UpdateLayout();
        }
    }

    protected override void OnTransformChildrenChanged() {
        for (int i = 0, len = transform.childCount; i < len; i++) {
            RectTransform child = transform.GetChild(i) as RectTransform;
            if (!child) continue;

            child.anchorMin = new Vector2(0f, 0f);
            child.anchorMax = new Vector2(0f, 0f);
        }
    }

    protected override void OnDisable() {
        if (Application.isPlaying) {
            if (m_scrollRect) {
                m_scrollRect.onValueChanged.RemoveListener(OnScrollRectValueChangedHandler);
            }
        }
    }

#if UNITY_EDITOR
    protected override void Reset() {
        if (!m_scrollRect) {
            m_scrollRect = GetComponentInParent<ScrollRect>();
        }
        InitOrderListWithChildren();
    }

    protected override void OnValidate() {
        m_offsetBeyondViewport = Mathf.Max(0f, m_offsetBeyondViewport);
    }
#endif

}
