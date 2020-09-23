using UnityEngine;
using System.Collections;
/// <summary>
/// RectTransform工具类
/// </summary>
public class RectTransformUtil{
	
	/// <summary>
	/// 返回指定RectTransform屏幕坐标矩形
	/// </summary>
	/// <param name="rectTransform">指定的RectTransform</param>
	/// <param name="canvasLocalScale">Canvas的缩放量</param>
	/// <returns></returns>
	public static Rect GetScreenRect(RectTransform rectTransform,Vector3 canvasLocalScale){
		Rect rect=rectTransform.rect;
		//根据Canvas缩放
		rect.width*=canvasLocalScale.x;
		rect.height*=canvasLocalScale.y;
		rect.position*=canvasLocalScale;
		//计算矩形左下角
		Vector2 leftBottom=(Vector2)rectTransform.position+rect.position;
		rect.position=leftBottom;
		return rect;
	}

	/// <summary>
	/// 移动一个 RectTransform 到指定的屏幕坐标位置
	/// </summary>
	/// <param name="rectTransform">要移动的 RectTransform</param>
	/// <param name="screenPoint">屏幕坐标位置</param>
	/// <param name="canvas">Canvas</param>
	/// <param name="anchorX">锚点x，范围区间[0,1]</param>
	/// <param name="anchorY">锚点y，范围区间[0,1]</param>
	public static void MoveToScreenPoint(RectTransform rectTransform,Vector2 screenPoint,Canvas canvas,float anchorX=0.5f,float anchorY=0.5f){
		float scaleFactor=canvas.scaleFactor;
		Vector2 screenSize=new Vector2(Screen.width,Screen.height);

		screenPoint/=scaleFactor;//计算屏幕在Canvas实际大小中的位置

		//计算枢轴，默认以中心为枢轴
		anchorX=Mathf.Clamp01(anchorX);
		anchorY=Mathf.Clamp01(anchorY);
		screenPoint-=rectTransform.rect.size*new Vector2(anchorX,anchorY);
		
		Vector2 realSize=screenSize/scaleFactor;//Canvas实际大小(像素为单位)
		
		Vector2 anchorMinPos=realSize*rectTransform.anchorMin;//anchorMin在Canvas实际大小中的位置(像素为单位)
		Vector2 anchorMaxPos=realSize*rectTransform.anchorMax;//anchorMax在Canvas实际大小中的位置(像素为单位)

		Vector2 leftBottomPos=anchorMinPos+rectTransform.offsetMin;//RectTransform框的左下角在Canvas实际大小中的位置(像素为单位)
		Vector2 rightTopPos=anchorMaxPos+rectTransform.offsetMax;//RectTransform框的右上角在Canvas实际大小中的位置(像素为单位)

		Vector2 offset=screenPoint-leftBottomPos;
		
		//offsetMin：表示RectTransform框的左下角减去anchorMin位置的值(像素为单位)。
		//offsetMax：表示RectTransform框的右上下角减去anchorMax位置的值(像素为单位)。
		rectTransform.offsetMin+=offset;
		rectTransform.offsetMax+=offset;
	}
}
