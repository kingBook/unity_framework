using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[ExecuteInEditMode]
[RequireComponent(typeof(GridLayoutGroup))]
public class GridLayoutCellSizeFitter : MonoBehaviour {

    public enum FitParentMode {
        None,
        WidthControlsHeight,
        HeightControlsWidth
    }

    public Vector2 designParentSize;
    public Vector2 designCellSize;
    public FitParentMode fitParentMode = FitParentMode.WidthControlsHeight;
    [Tooltip("仅在缩小时适配")] public bool zoomOutOnly = true;


    private GridLayoutGroup m_gridLayoutGroup;


    private void Awake() {
        m_gridLayoutGroup = GetComponent<GridLayoutGroup>();
    }

    private void Update() {
        Fit();
    }

    private void Fit() {
        Vector2 parentSize = ((RectTransform)transform.parent).rect.size;
        if (fitParentMode == FitParentMode.WidthControlsHeight) {
            float scale = parentSize.x / designParentSize.x;
            if (zoomOutOnly) {
                scale = Mathf.Min(scale, 1f);
            }
            m_gridLayoutGroup.cellSize = designCellSize * scale;
        } else if (fitParentMode == FitParentMode.HeightControlsWidth) {
            float scale = parentSize.y / designParentSize.y;
            if (zoomOutOnly) {
                scale = Mathf.Min(scale, 1f);
            }
            m_gridLayoutGroup.cellSize = designCellSize * scale;
        }
    }


#if UNITY_EDITOR
    private void Reset() {
        if (m_gridLayoutGroup == null) {
            m_gridLayoutGroup = GetComponent<GridLayoutGroup>();
        }
        designCellSize = m_gridLayoutGroup.cellSize;
        designParentSize = ((RectTransform)transform.parent).rect.size;
    }
#endif
}
