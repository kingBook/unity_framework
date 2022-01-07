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
    public static void MoveToScreenPoint (RectTransform rectTransform, Vector2 screenPoint, Canvas canvas, Vector2 anchor) {
        float scaleFactor = canvas.scaleFactor;
        Vector2 screenSize = new Vector2(Screen.width, Screen.height);

        screenPoint /= scaleFactor;//计算屏幕坐标在Canvas实际大小中的位置

        //计算枢轴
        anchor.x = Mathf.Clamp01(anchor.x);
        anchor.y = Mathf.Clamp01(anchor.y);
        screenPoint -= rectTransform.rect.size * anchor;

        Vector2 realSize = screenSize / scaleFactor;//Canvas实际大小(像素为单位)

        Vector2 anchorMinPos = realSize * rectTransform.anchorMin;//anchorMin在Canvas实际大小中的位置(像素为单位)
        Vector2 anchorMaxPos = realSize * rectTransform.anchorMax;//anchorMax在Canvas实际大小中的位置(像素为单位)

        Vector2 leftBottomPos = anchorMinPos + rectTransform.offsetMin;//RectTransform框的左下角在Canvas实际大小中的位置(像素为单位)
        Vector2 rightTopPos = anchorMaxPos + rectTransform.offsetMax;//RectTransform框的右上角在Canvas实际大小中的位置(像素为单位)

        Vector2 offset = screenPoint - leftBottomPos;

        //offsetMin：表示RectTransform框的左下角减去anchorMin位置的值(像素为单位)。
        //offsetMax：表示RectTransform框的右上下角减去anchorMax位置的值(像素为单位)。
        rectTransform.offsetMin += offset;
        rectTransform.offsetMax += offset;
    }

    /// <summary>
    /// 根据屏幕速度向量移动 RectTransform
    /// </summary>
    /// <param name="rectTransform"> 要移动的 RectTransform </param>
    /// <param name="velocity"> 屏幕速度向量 </param>
    /// <param name="canvas"> Canvas </param>
    public static void Move (RectTransform rectTransform, Vector2 velocity, Canvas canvas) {
        velocity /= canvas.scaleFactor;

        rectTransform.offsetMin += velocity;
        rectTransform.offsetMax += velocity;
    }

}