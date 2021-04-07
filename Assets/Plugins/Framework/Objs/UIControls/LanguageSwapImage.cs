using UnityEngine;
using System.Collections;
using UnityEngine.UI;
/// <summary>
/// 根据语言交换图片
/// </summary>
public class LanguageSwapImage : MonoBehaviour {
    public Sprite spriteEN;
    public Sprite spriteCN;
    private Image m_image;

    private void Awake () {
        m_image = GetComponent<Image>();
        if (App.instance != null) {
            SwapImageToLanguage(App.instance.language);
        }
    }

    private void Start () {
        SwapImageToLanguage(App.instance.language);
        App.instance.onChangeLanguageEvent += OnChangeLanguage;
    }

    private void OnChangeLanguage (App.Language language) {
        SwapImageToLanguage(language);
    }

    private void SwapImageToLanguage (App.Language language) {
        if (language == App.Language.EN) {
            if (spriteEN != null) m_image.sprite = spriteEN;
        } else if (language == App.Language.CN) {
            if (spriteCN != null) m_image.sprite = spriteCN;
        }
    }

    private void OnDestroy () {
        App.instance.onChangeLanguageEvent -= OnChangeLanguage;
    }
}