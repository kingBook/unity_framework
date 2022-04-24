#if UNITY_EDITOR
using System.Threading;
using UnityEditor;
using UnityEditor.ShortcutManagement;
using UnityEngine;

public class QuickSelectInHierarchyAndProjectPanelEditor : ScriptableObject {

    private static double s_lastKeytypedTime;
    private static string s_input="";

    // 第一次打开 Unity 编辑器运行一次，之后每次进入 Play 模式都运行一次
    [InitializeOnLoadMethod]
    private static void InitOnLoad() {
        //EditorApplication.update -= OnApplicationUpdate;
        //EditorApplication.update += OnApplicationUpdate;
        EditorApplication.projectWindowItemOnGUI -= OnProjectGUI;
        EditorApplication.projectWindowItemOnGUI += OnProjectGUI;
        s_lastKeytypedTime = EditorApplication.timeSinceStartup;
    }


    private static void OnApplicationUpdate() {

    }

    private static void OnProjectGUI(string guid, Rect selectionRect) {
        //Debug.Log($"guid:{guid}, selectionRect:{selectionRect}");
        if (Event.current != null) {
            if (Event.current.isKey) {
                if (EditorApplication.timeSinceStartup - s_lastKeytypedTime < 1f) {
                    s_input += Event.current.character.ToString();
                } else {
                    s_input = Event.current.character.ToString();
                }
                s_lastKeytypedTime = EditorApplication.timeSinceStartup;

                string[] assetGUIDs = Selection.assetGUIDs;
                if (assetGUIDs.Length > 0) {
                    string firstAssetPath = AssetDatabase.GUIDToAssetPath(assetGUIDs[0]);
                    //Debug.Log($"firstAssetPath:{firstAssetPath}");
                    
                    /*string[] GUIDs=AssetDatabase.FindAssets(s_input, new string[] { firstAssetPath });
                    if(GUIDs.Length>0){
                        string GUID = GUIDs[0];
                        string assetPath = AssetDatabase.GUIDToAssetPath(GUID);
                        Debug.Log($"assetPath:{assetPath}");
                        Object obj = AssetDatabase.LoadAssetAtPath<Object>(assetPath);
                        Selection.instanceIDs = new int[] { obj.GetInstanceID() };
                    }*/
                    
                }
            }
        }
    }


}
#endif
