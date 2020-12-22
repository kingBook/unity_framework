#pragma warning disable 0649
using UnityEngine;
using UnityEngine.UI;
/// <summary>
/// Image.overrideSprite适配切换器。
/// 需要所有Anchors角点设置为一个点。
/// 将以(PosX,PosY)为中心，正比缩放Sprite在FitSize的矩形框范围内。
/// </summary>
public class ImageSpriteFitSwitcher:MonoBehaviour{
	[SerializeField]
	private Image m_image;
	[SerializeField]
	private Vector2 m_fitSize;
	
#if UNITY_EDITOR 
	private void Reset(){
		m_image=gameObject.GetComponent<Image>();
		m_fitSize=m_image.rectTransform.sizeDelta;
	}
#endif
	
	/// <summary>
	/// 交换Sprite
	/// </summary>
	public void SwapTo(Sprite sprite){
		m_image.overrideSprite=sprite;
		Fit(sprite);
	}
	
	/// <summary>
	/// 从2D纹理交换Sprite
	/// </summary>
	public void SwapTo(Texture2D texture){
		Sprite sprite=Sprite.Create(texture,new Rect(0,0,texture.width,texture.height),new Vector2(0.5f,0.5f));
		SwapTo(sprite);
	}
	
	/// <summary>
	/// 从Resources文件夹加载2D纹理交换Sprite
	/// </summary>
	/// <param name="resourcesTexture2DPath">Resources文件夹下2D纹理文件路径</param>
	public void SwapTo(string resourcesTexture2DPath){
		Sprite sprite=Resources.Load<Sprite>(resourcesTexture2DPath);
		SwapTo(sprite);
	}
	
	/// <summary>
	/// 正比缩放适应m_fitSize指定的矩形框
	/// </summary>
	private void Fit(Sprite sprite){
		Vector2 size=sprite.rect.size;
		float sx=m_fitSize.x/size.x;
		float sy=m_fitSize.y/size.y;
		float scale=Mathf.Min(sx,sy);
		size.x*=scale;
		size.y*=scale;
		m_image.rectTransform.sizeDelta=size;
	}
	
}