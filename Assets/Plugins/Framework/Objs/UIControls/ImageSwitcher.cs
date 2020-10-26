using UnityEngine;
using System.Collections;
using UnityEngine.UI;
/// <summary>
/// 图片交换器
/// </summary>
public class ImageSwitcher:BaseMonoBehaviour{
	
	[Tooltip("切换的图片(未指定时自动从当前对象组件列表中获取)")]
	public Image image=null;

	[Tooltip("来回切换的两张图片")]
	public Sprite[] sprites;

#if UNITY_EDITOR
	protected override void Reset(){
		base.Reset();
		InitImageOnReset();
	}

	protected virtual void InitImageOnReset(){
		if(!image){
			image=GetComponent<Image>();
		}
	}
#endif

	protected override void Awake(){
		base.Awake();
		if(!image){
			image=GetComponent<Image>();
		}
	}

	public void AutoSwap(){
		if(image.sprite==sprites[0]){
			SwapTo(1);
		}else{
			SwapTo(0);
		}
	}

	public void	SwapTo(int spriteId){
		image.sprite=sprites[spriteId];
	}

}
