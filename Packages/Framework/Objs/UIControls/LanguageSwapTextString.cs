#pragma warning disable 0649

using System.Collections;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 根据语言交换文本框的文本
/// </summary>
public class LanguageSwapTextString : MonoBehaviour {

    public string stringEN;
    public string stringCN;
    public Text text;

    private void OnChangeLanguage(App.Language language) {
        SwapStringToLanguage(language);
    }

#if UNITY_EDITOR
    private void Reset() {
        if (!text) {
            text = GetComponent<Text>();
        }
    }
#endif

    private void Awake() {
        if (App.instance != null) {
            SwapStringToLanguage(App.instance.language);
        }
    }

    private void Start() {
        SwapStringToLanguage(App.instance.language);
        App.instance.onChangedLanguageEvent += OnChangeLanguage;
    }

    private void SwapStringToLanguage(App.Language language) {
        if (language == App.Language.EN) {
            if (text != null) text.text = stringEN;
        } else if (language == App.Language.CN) {
            if (text != null) text.text = stringCN;
        }
    }

    private void OnDestroy() {
        App.instance.onChangedLanguageEvent -= OnChangeLanguage;
    }
}
