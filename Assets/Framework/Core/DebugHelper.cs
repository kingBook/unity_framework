using UnityEngine;

/// <summary>
/// 调试助手，用于在屏幕显示 Log 信息等
/// </summary>
public class DebugHelper : MonoBehaviour {

    /// <summary>
    /// 添加金钱事件，函数格式：void OnAddMoneys()
    /// </summary>
    public event System.Action onAddMoneysEvent;

    [Tooltip("是否最小化")]
    [SerializeField] private bool m_isMinimized;

    private string m_output = "";
    private Vector2 m_scrollPos;
    private bool m_isPause;
    private bool m_isStackTrace;
    private bool m_isUnlockLevel;

    public bool isUnlockLevel => m_isUnlockLevel;

    private void OnEnable () {
        Application.logMessageReceivedThreaded += LogHandler;
    }

    private void OnGUI () {
        float buttonSize = Mathf.Min(Screen.width, Screen.height) * 0.1f;
        if (m_isMinimized) {
            if (GUILayout.Button(" > ", GUILayout.MinWidth(buttonSize), GUILayout.MinHeight(buttonSize))) {
                m_isMinimized = false;
            }
        } else {
            float width = Screen.width * 0.5f;
            float height = Screen.height;
            GUILayout.BeginVertical();

            // 滚动的文本
            m_scrollPos = GUILayout.BeginScrollView(m_scrollPos);
            GUILayout.TextArea(m_output, GUILayout.MaxWidth(width), GUILayout.ExpandHeight(true));
            GUILayout.EndScrollView();

            GUILayout.BeginHorizontal();
            {
                // 最小化按钮
                if (GUILayout.Button(" < ", GUILayout.MinHeight(buttonSize))) {
                    m_isMinimized = true;
                }

                // 暂停/恢复按钮
                string pauseResumeText = m_isPause ? "Resume" : "Pause";
                if (GUILayout.Button(pauseResumeText, GUILayout.MinHeight(buttonSize))) {
                    m_isPause = !m_isPause;
                }

                // 清除按钮
                if (GUILayout.Button("Clear", GUILayout.MinHeight(buttonSize))) {
                    m_output = "";
                }
            }
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            {
                // 清除本地数据按钮
                if (GUILayout.Button("ClearLocalData", GUILayout.MinHeight(buttonSize))) {
                    PlayerPrefs.DeleteAll();
                    PlayerPrefs.Save();
                }

                // 增加金钱按钮
                if (GUILayout.Button("AddMoneys", GUILayout.MinHeight(buttonSize))) {
                    onAddMoneysEvent?.Invoke();
                }
            }
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            {
                // 是否解锁关卡单选框
                m_isUnlockLevel = GUILayout.Toggle(m_isUnlockLevel, "UnlockLevel", GUILayout.MinHeight(buttonSize));
                // 跟踪 Log 栈
                m_isStackTrace = GUILayout.Toggle(m_isStackTrace, "StackTrace", GUILayout.MinHeight(buttonSize));
            }
            GUILayout.EndHorizontal();

            GUILayout.EndVertical();
        }
    }

    private void LogHandler (string logString, string stackTrace, LogType type) {
        if (m_isPause) return;
        m_output += $"[{System.DateTime.Now.ToString("HH:mm:ss")}] {logString}\n";
        if (m_isStackTrace) {
            m_output += stackTrace + '\n';
        }
    }

    private void OnDisable () {
        Application.logMessageReceivedThreaded -= LogHandler;
    }

}