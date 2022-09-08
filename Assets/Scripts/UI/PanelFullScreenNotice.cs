using System.Collections;
using TMPro;
using UnityEngine;

public class PanelFullScreenNotice : MonoBehaviour {

    [SerializeField] private GameObject m_prefabFullScreenNotice;
    [SerializeField] private AudioClip m_audioClipNotice;

    /// <summary>  普通的文字全屏通知 </summary>
    public void CreateFullScreenNotice(string textStrings) {
        GameObject inst = Instantiate(m_prefabFullScreenNotice, m_prefabFullScreenNotice.transform.parent);
        TMP_Text txt = inst.GetComponentInChildren<TMP_Text>();
        txt.text = textStrings;
        inst.SetActive(true);
        App.instance.audioManager.PlayEffect(m_audioClipNotice, transform.position);
    }

    private void OnEnable() {
        m_prefabFullScreenNotice.SetActive(false);
    }

}
