using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 点击按钮时，切换按钮上的图片
/// </summary>
public class SwapButtonImage:BaseMonoBehaviour{
	[Tooltip("来回切换的两张图片")]
	public Sprite[] sprites;
	private Button _button;
	private Image _image;
	protected override void Awake() {
		base.Awake();
		_button=GetComponent<Button>();
		_button.onClick.AddListener(onClick);

		_image=GetComponent<Image>();
	}

	private void onClick(){
		if(_image.sprite==sprites[0]){
			_image.sprite=sprites[1];
		}else{
			_image.sprite=sprites[0];
		}
	}

	public void	swapTo(int spriteId){
		_image.sprite=sprites[spriteId];
	}

	protected override void OnDestroy() {
		_button.onClick.RemoveListener(onClick);
		base.OnDestroy();
	}
}
