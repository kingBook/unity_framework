#pragma warning disable 0649

using System.Collections;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 根据语言交换文本框的文本
/// <para> 注意： </para>
/// <para> 当在 Unity 编辑器中输入字符串时，如果存在转义字符如"\r, \n, \r\n, \t, \b"，请勾选<see cref="isUnescapeOnAwake"/>，</para>
/// <para> 或不要输入转义字符直接在字符串中按回车、tab等键代替转义字符. </para>
/// </summary>
public class LanguageSwapTextString : MonoBehaviour {

    [Multiline] public string stringEN;
    [Multiline] public string stringCN;
    public Text text;
    [Tooltip("在 Awake 函数中，是否使用 Regex.Unescape(string) 转换输入字符串中的任何转义字符")] public bool isUnescapeOnAwake;

    private void OnChangedLanguage(App.Language language) {
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
        if (isUnescapeOnAwake) {
            stringEN = Regex.Unescape(stringEN);
            stringCN = Regex.Unescape(stringCN);
        }
        if (App.instance != null) {
            SwapStringToLanguage(App.instance.language);
        }
    }

    private void Start() {
        SwapStringToLanguage(App.instance.language);
        App.instance.onChangedLanguageEvent += OnChangedLanguage;
    }

    private void SwapStringToLanguage(App.Language language) {
        if (text == null) return;
        if (language == App.Language.EN) {
            text.text = stringEN;
        } else if (language == App.Language.CN) {
            text.text = stringCN;
        }
    }

    private void OnDestroy() {
        App.instance.onChangedLanguageEvent -= OnChangedLanguage;
    }
}
