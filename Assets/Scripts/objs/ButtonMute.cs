using UnityEngine;
using System.Collections;
using UnityEngine.UI;

[RequireComponent(typeof(SwapButtonImage))]
public class ButtonMute:BaseMonoBehaviour{
	
	protected override void Start() {
		base.Start();
		SwapButtonImage swapButtonImage=GetComponent<SwapButtonImage>();
		swapButtonImage.swapTo(AudioListener.pause?1:0);
	}

}
