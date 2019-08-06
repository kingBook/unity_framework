using UnityEngine;
/// <summary>
/// 静音按钮，在Start()中:
/// <br>AudioListener.pause==true，SwapButtonImage.swapTo(1)，第二张图</br>
/// <br>AudioListener.pause==false，SwapButtonImage.swapTo(0)，第一张图</br>
/// </summary>
[RequireComponent(typeof(SwapButtonImage))]
public class ButtonMute:BaseMonoBehaviour{
	
	protected override void Start() {
		base.Start();
		SwapButtonImage swapButtonImage=GetComponent<SwapButtonImage>();
		swapButtonImage.swapTo(AudioListener.pause?1:0);
	}

}
