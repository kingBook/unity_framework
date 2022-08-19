using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Profiling;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class PanelDebugHelper : MonoBehaviour {

    /// <summary>
    /// 添加金钱事件，回调函数格式：<code> void OnAddMoneysHandler() </code>
    /// </summary>
    public event System.Action onAddMoneysEvent;

    /// <summary>
    /// 进入指定关卡事件，回调函数格式：<code> void OnGotoLevelHandler(int levelNumber) </code>
    /// </summary>
    public event System.Action<int> onGotoLevelEvent;

    /// <summary>
    /// 切换解锁状态事件，回调函数格式: <code> void OnToggleUnlockLevelHandler(bool isUnlockLevel) </code>
    /// </summary>
    public event System.Action<bool> onToggleUnlockLevelEvent;


    [SerializeField] GameObject[] m_groups;
    [SerializeField] private TMP_Text m_textInfo;
    [SerializeField] private TMP_Text m_textOutputOneLine;
    [SerializeField] private TMP_Text m_textPauseOrResume;
    [SerializeField] private TMP_InputField m_inputFieldOutput;
    [SerializeField] private TMP_InputField m_inputFieldLevelNumber;

    private int m_groupIndex;
    private float m_time;
    private bool m_isPause;
    private bool m_isStackTrace;
    private bool m_isUnlockLevel;
    private bool m_isSingleLine;

    public bool isUnlockLevel => m_isUnlockLevel;


    #region Editor Reference
    public void OnClickButtonExpand() {
        ActiveGroup(1);
    }

    public void OnClickButtonCollapse() {
        ActiveGroup(0);
    }

    public void OnClickButtonClose() {
        ActiveGroup(-1);
    }

    public void OnClickButtonClearLocalData() {
        // 清除本地数据
        PlayerPrefs.DeleteAll();
        PlayerPrefs.Save();
    }

    public void OnClickButtonAddMoneys() {
        onAddMoneysEvent?.Invoke();
    }

    public void OnClickButtonPauseOrResume() {
        m_isPause = !m_isPause;
        m_textPauseOrResume.text = m_isPause ? "Resume" : "Pause";
    }

    public void OnClickButtonClear() {
        m_inputFieldOutput.text = "";
    }

    public void OnClickButtonGo() {
        if (int.TryParse(m_inputFieldLevelNumber.text, out int levelNumber)) {
            onGotoLevelEvent?.Invoke(levelNumber);
        } else {
            m_inputFieldLevelNumber.text = "";
            Debug.Log("Please enter the level number");
        }

    }

    public void OnToggleUnlockLevel() {
        m_isUnlockLevel = !m_isUnlockLevel;
        onToggleUnlockLevelEvent?.Invoke(m_isUnlockLevel);
    }

    public void OnToggleStackTrace() {
        m_isStackTrace = !m_isStackTrace;
    }

    public void OnToggleSingleLine() {
        m_isSingleLine = !m_isSingleLine;
    }
    #endregion

    private void ActiveGroup(int index) {
        m_groupIndex = index;
        for (int i = 0; i < m_groups.Length; i++) {
            m_groups[i].SetActive(m_groupIndex == i);
        }
        m_textOutputOneLine.text = "";
    }

    private void UpdateInfoText() {
        m_time += (Time.unscaledDeltaTime - m_time) * 0.1f;

        float ms = m_time * 1000.0f;
        float fps = 1.0f / m_time;

        //ms 保留一位小数
        ms = Mathf.Ceil(ms * 10f) / 10f;
        //fps 只取整数
        fps = Mathf.Floor(fps);

        float mem = Mathf.CeilToInt(Profiler.GetTotalAllocatedMemoryLong() / 1024f / 1024f * 10f) / 10f;
        float memTotal = Mathf.CeilToInt(Profiler.GetTotalReservedMemoryLong() / 1024f / 1024f * 10f) / 10f;

#if UNITY_EDITOR
        int savedDynamicBatches = UnityStats.dynamicBatchedDrawCalls - UnityStats.dynamicBatches;
        int savedStaticBatches = UnityStats.staticBatchedDrawCalls - UnityStats.staticBatches;
        int savedInstancedBatches = UnityStats.instancedBatchedDrawCalls - UnityStats.instancedBatches;
        int savedByBatching = savedDynamicBatches + savedStaticBatches + savedInstancedBatches;

        m_textInfo.text = $"{fps} FPS ({ms}ms)  {mem}/{memTotal}MB\n" +
                          $"batches:{UnityStats.batches}({savedByBatching})  shadowCasters:{UnityStats.shadowCasters}";
#else
        m_textInfo.text = $"{fps} FPS ({ms}ms) {mem}/{memTotal}MB";
#endif
    }

    private void LogHandler(string logString, string stackTrace, LogType type) {
        if (m_isPause) return;

        m_textOutputOneLine.text = $"[{System.DateTime.Now.ToString("HH:mm:ss")}] {logString}";

        // 单行时，每次清空
        if (m_isSingleLine) {
            m_inputFieldOutput.text = "";
        }

        m_inputFieldOutput.text += $"[{System.DateTime.Now.ToString("HH:mm:ss")}] {logString}\n";
        if (m_isStackTrace) {
            m_inputFieldOutput.text += stackTrace + '\n';
        }
    }

    private void Awake() {
        ActiveGroup(0);
    }

    private void OnEnable() {
        Application.logMessageReceivedThreaded += LogHandler;
    }

    private void Update() {
        UpdateInfoText();
    }

    private void OnDisable() {
        Application.logMessageReceivedThreaded -= LogHandler;
    }
}
