using UnityEngine;
/// <summary>
/// 静音按钮，在Start()中:
/// </summary>
/// <code>
/// if(AudioListener.volume<=0)SwapButtonImage.swapTo(1);//第二张图
/// if(AudioListener.volume>0) SwapButtonImage.swapTo(0);//第一张图
/// </code>
[RequireComponent(typeof(ButtonImageSwitcher))]
public class ButtonMute : MonoBehaviour {

    private void Start () {
        ButtonImageSwitcher buttonImageSwitcher = GetComponent<ButtonImageSwitcher>();
        buttonImageSwitcher.SwapTo(AudioListener.volume <= 0 ? 1 : 0);
    }

}
