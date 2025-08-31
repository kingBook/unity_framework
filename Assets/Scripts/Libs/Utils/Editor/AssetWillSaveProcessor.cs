#if UNITY_EDITOR
using UnityEditor;
using UnityEngine.SceneManagement;
using UnityEngine;

public class AssetWillSaveProcessor : UnityEditor.AssetModificationProcessor {

    private const string SCENE_EXTENSION = ".unity";

    /// <summary>
    /// 将要保存某个场景前事件，回调函数格式：<code> void OnWillSaveSceneHandler(Scene scene) </code>
    /// </summary>
    public static event System.Action<Scene> onWillSaveSceneEvent;

    public static void OnWillSaveAssets(string[] names) {
        for (int i = 0, len = names.Length; i < len; i++) {
            string name = names[i];
            if(StringUtil.EndsWith(name, SCENE_EXTENSION)) {
                Scene scene = SceneManager.GetSceneByPath(name);
                onWillSaveSceneEvent?.Invoke(scene);
            }
        }
    }
}
#endif