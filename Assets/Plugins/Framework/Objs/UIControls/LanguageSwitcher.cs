using UnityEngine;
using System.Collections;
/// <summary>
/// 根据当前应用程序的语言激活/吊销列表中的GameObject
/// </summary>
public class LanguageSwitcher : MonoBehaviour {
    [Tooltip("英文时，需要激活的GameObject列表")]
    public GameObject[] enList;
    [Tooltip("中文时，需要激活的GameObject列表")]
    public GameObject[] cnList;

    private void Awake () {
        if (App.instance) {
            ActiveWithLanguage(App.instance.language);
        }
    }

    private void Start () {
        ActiveWithLanguage(App.instance.language);
        App.instance.onChangeLanguageEvent += OnChangeLanguage;
    }

    private void ActiveWithLanguage (App.Language language) {
        if (language == App.Language.AUTO) return;
        GameObject[] activeList = null;
        GameObject[] deactiveList = null;
        if (language == App.Language.EN) {
            activeList = enList;
            deactiveList = cnList;
        } else if (language == App.Language.CN) {
            activeList = cnList;
            deactiveList = enList;
        }

        int i = activeList.Length;
        while (--i >= 0) {
            activeList[i].SetActive(true);
        }

        i = deactiveList.Length;
        while (--i >= 0) {
            deactiveList[i].SetActive(false);
        }
    }

    private void OnChangeLanguage (App.Language language) {
        ActiveWithLanguage(language);
    }

    private void OnDestroy () {
        App.instance.onChangeLanguageEvent -= OnChangeLanguage;
    }
}