using UnityEngine;
using System.Collections;
using UnityEngine.UI;

[RequireComponent(typeof(SwapButtonImage))]
public class ButtonMute:BaseMonoBehaviour{
	
	protected override void Awake() {
		base.Awake();
		SwapButtonImage swapButtonImage=GetComponent<SwapButtonImage>();
		swapButtonImage.swapTo(AudioListener.pause?1:0);
	}

}
