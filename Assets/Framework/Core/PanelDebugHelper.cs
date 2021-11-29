using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class PanelDebugHelper : MonoBehaviour {

    [SerializeField] GameObject[] m_groups;
    [SerializeField] private Text m_textFPS;
    //[SerializeField] private TMPro.TMP_InputField m_textOutput;

    private int m_groupIndex;
    private float m_time;

    #region Editor Reference
    public void OnClickButtonExpand () {
        ActiveGroup(1);
    }
    public void OnClickButtonCollapse () {
        ActiveGroup(0);
    }
    #endregion

    private void ActiveGroup (int index) {
        m_groupIndex = index;
        for (int i = 0; i < m_groups.Length; i++) {
            m_groups[i].SetActive(m_groupIndex == i);
        }
    }

    private void UpdateFPS () {
        m_time += (Time.unscaledDeltaTime - m_time) * 0.1f;

        float ms = m_time * 1000.0f;
        float fps = 1.0f / m_time;

        //ms 保留一位小数
        ms = Mathf.Ceil(ms * 10f) / 10f;
        //fps 只取整数
        fps = Mathf.Floor(fps);

        m_textFPS.text = $"{fps} FPS ({ms}ms)";
    }

    private void Start () {
        ActiveGroup(0);
    }

    private void Update () {
        UpdateFPS();
    }

}
