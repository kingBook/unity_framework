using System.Collections;
using TMPro;
using UnityEngine;

public class PanelInfo : MonoBehaviour {

    [SerializeField] private TMP_Text m_textLevelNumber;
    [SerializeField] private GameObject m_prefabFullScreenMessage;
    [SerializeField] private AudioClip m_audioClipMessage;

    /// <summary>  普通的文字全屏消息 </summary>
    public void CreateFullScreenMessage(string textStrings) {
        GameObject inst = Instantiate(m_prefabFullScreenMessage, m_prefabFullScreenMessage.transform.parent);
        TMP_Text txt = inst.GetComponentInChildren<TMP_Text>();
        txt.text = textStrings;
        inst.SetActive(true);
        App.instance.audioManager.PlayEffect(m_audioClipMessage, transform.position);
    }

    private void OnEnable() {
        m_prefabFullScreenMessage.SetActive(false);
    }
}
