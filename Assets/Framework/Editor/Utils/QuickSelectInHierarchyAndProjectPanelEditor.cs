#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.ShortcutManagement;
using UnityEngine;

public class QuickSelectInHierarchyAndProjectPanelEditor : ScriptableObject {

    // 第一次打开 Unity 编辑器运行一次，之后每次进入 Play 模式都运行一次
    [InitializeOnLoadMethod]
    private static void InitOnLoad() {
        //EditorApplication.update -= OnApplicationUpdate;
        //EditorApplication.update += OnApplicationUpdate;
    }


    private static void OnApplicationUpdate() {

    }


}
#endif
