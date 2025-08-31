using UnityEngine;
using System.Collections;
/// <summary>
/// 根据语言本地化对象激活器
/// </summary>
public class LocalizedObjectActivator : MonoBehaviour {
    
    [Tooltip("英文时, 需要激活的GameObject列表")]
    public GameObject[] enList;
    [Tooltip("中文时, 需要激活的GameObject列表")]
    public GameObject[] cnList;

    private void Awake() {
        if (App.instance) {
            ActiveWithLanguage(App.instance.language);
        }
    }

    private void Start() {
        ActiveWithLanguage(App.instance.language);
        App.instance.onChangedLanguageEvent += OnChangeLanguage;
    }

    private void ActiveWithLanguage(App.Language language) {
        if (language == App.Language.Auto) return;
        GameObject[] activeList = null;
        GameObject[] deactiveList = null;
        if (language == App.Language.En) {
            activeList = enList;
            deactiveList = cnList;
        } else if (language == App.Language.Cn) {
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

    private void OnChangeLanguage(App.Language language) {
        ActiveWithLanguage(language);
    }

    private void OnDestroy() {
        App.instance.onChangedLanguageEvent -= OnChangeLanguage;
    }
}