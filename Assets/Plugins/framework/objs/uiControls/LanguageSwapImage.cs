using UnityEngine;
using System.Collections;
using UnityEngine.UI;
/// <summary>
/// 根据语言交换图片
/// </summary>
public class LanguageSwapImage:BaseMonoBehaviour{
	public Sprite spriteEN;
	public Sprite spriteCN;
	private Image m_image;

	protected override void Awake() {
		base.Awake();
		m_image=GetComponent<Image>();
		if(App.instance!=null){
			SwapImageToLanguage(App.instance.language);
		}
	}

	protected override void Start(){
		base.Start();
		SwapImageToLanguage(App.instance.language);
		App.instance.onChangeLanguage+=OnChangeLanguage;
	}

	private void OnChangeLanguage(Language language){
		SwapImageToLanguage(language);
	}

	private void SwapImageToLanguage(Language language){
		if(language==Language.EN){
			if(spriteEN!=null) m_image.sprite=spriteEN;
		}else if(language==Language.CN){
			if(spriteCN!=null)m_image.sprite=spriteCN;
		}
	}

	protected override void OnDestroy(){
		App.instance.onChangeLanguage-=OnChangeLanguage;
		base.OnDestroy();
	}
}
