using UnityEngine;
using System.Collections;
using UnityEngine.UI;

/// <summary>
/// 根据语言交换图片
/// </summary>
public class LanguageSwapImage : MonoBehaviour {

    public Sprite spriteEN;
    public Sprite spriteCN;

    [SerializeField, SetProperty(nameof(useFitSwitcher))] // 此处使用SetProperty序列化setter方法，用法： https://github.com/LMNRY/SetProperty
    private bool m_useFitSwitcher;

    private Image m_image;
    private ImageSpriteFitSwitcher m_imageSpriteFitSwitcher;

    public bool useFitSwitcher {
        get => m_useFitSwitcher;
        set {
            m_useFitSwitcher = value;
#if UNITY_EDITOR
            if (!GetComponent<ImageSpriteFitSwitcher>()) {
                gameObject.AddComponent<ImageSpriteFitSwitcher>();
            }
#endif
        }
    }

    private void Awake() {
        m_image = GetComponent<Image>();
        if (App.instance != null) {
            if (useFitSwitcher) {
                m_imageSpriteFitSwitcher = GetComponent<ImageSpriteFitSwitcher>();
                if (!m_imageSpriteFitSwitcher) {
                    Debug.LogError("useFitSwitcher 为 true 时，必须给游戏对象添加 ImageSpriteFitSwitcher 组件");
                    return;
                }
            }

            SwapImageToLanguage(App.instance.language);
        }
    }

    private void Start() {
        SwapImageToLanguage(App.instance.language);
        App.instance.onChangedLanguageEvent += OnChangeLanguage;
    }

    private void OnChangeLanguage(App.Language language) {
        SwapImageToLanguage(language);
    }

    private void SwapImageToLanguage(App.Language language) {
        Sprite targetSprite = spriteEN;
        if (language == App.Language.Cn) {
            targetSprite = spriteCN;
        }

        if (targetSprite != null) {
            if (useFitSwitcher) {
                m_imageSpriteFitSwitcher.SwapTo(targetSprite);
            } else {
                m_image.sprite = targetSprite;
            }
        }
    }

    private void OnDestroy() {
        App.instance.onChangedLanguageEvent -= OnChangeLanguage;
    }
}