using UnityEngine;
using System.Collections;

/// <summary>
/// RectTransform工具类
/// </summary>
public class RectTransformUtil {

    /// <summary>
    /// 移动一个 RectTransform 到指定的屏幕坐标位置（此方法需要 Anchor 在左下角 AnchorMin 和 AnchorMax 都为0）
    /// </summary>
    /// <param name="rectTransform">要移动的 RectTransform</param>
    /// <param name="screenPoint">屏幕坐标位置</param>
    /// <param name="canvas">Canvas</param>
    /// <param name="anchor">锚点，x,y范围区间[0,1]</param>
    public static void MoveToScreenPoint(RectTransform rectTransform, Vector2 screenPoint, Canvas canvas, Vector2 anchor) {
        float scaleFactor = canvas.scaleFactor;
        Vector2 screenSize = new Vector2(Screen.width, Screen.height);

        screenPoint /= scaleFactor; // 计算屏幕坐标在 Canvas 实际大小中的位置

        // 计算枢轴
        anchor.x = Mathf.Clamp01(anchor.x);
        anchor.y = Mathf.Clamp01(anchor.y);
        screenPoint -= rectTransform.rect.size * anchor;

        Vector2 realSize = screenSize / scaleFactor; // Canvas实际大小(像素为单位)

        Vector2 anchorMinPos = realSize * rectTransform.anchorMin; // anchorMin 在 Canvas 实际大小中的位置(像素为单位)
        //Vector2 anchorMaxPos = realSize * rectTransform.anchorMax; // anchorMax 在 Canvas 实际大小中的位置(像素为单位)

        Vector2 leftBottomPos = anchorMinPos + rectTransform.offsetMin; // RectTransform 框的左下角在 Canvas 实际大小中的位置(像素为单位)
        //Vector2 rightTopPos = anchorMaxPos + rectTransform.offsetMax; // RectTransform 框的右上角在 Canvas 实际大小中的位置(像素为单位)

        Vector2 offset = screenPoint - leftBottomPos;

        // offsetMin：表示 RectTransform 框的左下角减去 anchorMin 位置的值(像素为单位)。
        // offsetMax：表示 RectTransform 框的右上下角减去 anchorMax 位置的值(像素为单位)。
        rectTransform.offsetMin += offset;
        rectTransform.offsetMax += offset;
    }

    /// <summary>
    /// 根据屏幕速度向量移动 RectTransform
    /// </summary>
    /// <param name="rectTransform"> 要移动的 RectTransform </param>
    /// <param name="velocity"> 屏幕速度向量 </param>
    /// <param name="canvas"> Canvas </param>
    /// <param name="isLimitToParent"> 限制移动范围在父级的矩形内 </param>
    public static void Move(RectTransform rectTransform, Vector2 velocity, Canvas canvas, bool isLimitToParent) {
        velocity /= canvas.scaleFactor;
        rectTransform.anchoredPosition += velocity;

        if (isLimitToParent) {
            LimitMoveRangeToParent(rectTransform);
        }
    }

    /// <summary>
    /// 限制 rectTransform 的移动范围在父级的矩形内
    /// </summary>
    /// <param name="rectTransform"></param>
    public static void LimitMoveRangeToParent(RectTransform rectTransform) {
        Bounds rangeBounds = RectTransformUtility.CalculateRelativeRectTransformBounds(rectTransform.parent);
        Bounds bounds = RectTransformUtility.CalculateRelativeRectTransformBounds(rectTransform.parent, rectTransform);

        Vector2 anchoredPosition = rectTransform.anchoredPosition;

        float dxMin = bounds.min.x - rangeBounds.min.x;
        float dxMax = bounds.max.x - rangeBounds.max.x;
        if (dxMin > 0.0f) {
            anchoredPosition.x -= dxMin;
        } else if (dxMax < 0.0f) {
            anchoredPosition.x -= dxMax;
        }

        float dyMin = bounds.min.y - rangeBounds.min.y;
        float dyMax = bounds.max.y - rangeBounds.max.y;
        if (dyMin > 0.0f) {
            anchoredPosition.y -= dyMin;
        } else if (dyMax < 0.0f) {
            anchoredPosition.y -= dyMax;
        }
        rectTransform.anchoredPosition = anchoredPosition;
    }

    /// <summary>
    /// 围绕指定的屏幕点缩放
    /// </summary>
    /// <param name="rectTransform"></param>
    /// <param name="screenPoint"></param>
    /// <param name="scaleValue"></param>
    /// <param name="canvas"></param>
    /// <param name="isLimitToParent"> 限制移动范围在父级的矩形内 </param>
    public static void ScaleAroundPoint(RectTransform rectTransform, Vector2 screenPoint, float scaleValue, Canvas canvas, bool isLimitToParent) {
        // 缩放前，在屏幕坐标系的矩形
        Rect rect = GetScreenRect(rectTransform, canvas);
        // 缩放前，围绕缩放点在矩形中的单位化位置
        Vector2 pointPivot = Rect.PointToNormalized(rect, screenPoint);

        rectTransform.localScale = Vector3.one * scaleValue;

        // 缩放后，在屏幕坐标系的矩形
        Rect rect2 = GetScreenRect(rectTransform, canvas);
        // 缩放后，围绕缩放点在矩形2中的位置
        Vector2 pivotInRect2 = Rect.NormalizedToPoint(rect2, pointPivot);

        // 移动与 screenPoint 对齐
        Vector2 offset = screenPoint - pivotInRect2;
        Move(rectTransform, offset, canvas, isLimitToParent);
    }

    /// <summary>
    /// 获取 RectTransform 在屏幕坐标中的矩形(由4个蓝色圆形角点构成的矩形)
    /// </summary>
    /// <param name="rectTransform"></param>
    /// <param name="canvas"></param>
    /// <returns></returns>
    public static Rect GetScreenRect(RectTransform rectTransform, Canvas canvas, bool isScale = true) {
        Rect rect = rectTransform.rect;

        if (isScale) {
            Vector3 localScale = rectTransform.localScale;
            Vector3 canvasLocalScale = canvas.transform.localScale;

            // 根据 Canvas 和自身缩放
            rect.width *= canvasLocalScale.x * localScale.x;
            rect.height *= canvasLocalScale.y * localScale.y;

            rect.position *= canvasLocalScale;
            rect.position *= localScale;
        }

        // 计算矩形左下角
        Vector2 leftBottom = (Vector2)rectTransform.position + rect.position;
        rect.position = leftBottom;
        return rect;
    }

    /// <summary>
    /// Canvas.RenderMode 为 Screen Space-Overlay 时，rectTransform.position 就是屏幕坐标
    /// </summary>
    /// <param name="rectTransform"></param>
    public static Vector2 GetScreenPoint(RectTransform rectTransform) {
        return rectTransform.position;
    }

}