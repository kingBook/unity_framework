using UnityEngine;
using System.Collections;
using UnityEngine.UI;
/// <summary>
/// 设置图片碰撞测试的最小透明度
/// <br>注意：使用此脚本必须在图片导入设置中勾选Read/Write Enabled属性</br>
/// </summary>
[RequireComponent(typeof(Image))]
[DisallowMultipleComponent]
public class ImageHitTestMinAlpha:MonoBehaviour{
	[Tooltip("碰撞测试的最小透明度")]
	[Range(0,1)]
	public float alphaHitTestMinimumThreshold=0.01f;
	private Image m_image;

	private void Awake(){
		m_image=GetComponent<Image>();
		m_image.alphaHitTestMinimumThreshold=alphaHitTestMinimumThreshold;
	}
}
