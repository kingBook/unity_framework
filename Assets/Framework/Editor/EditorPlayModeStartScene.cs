using UnityEditor;
using UnityEditor.SceneManagement;

public class EditorPlayModeStartScene : Editor {

    [MenuItem("Tools/PlayModeUseStartScene", true)]
    private static bool ValidateMenuItem () {
        Menu.SetChecked("Tools/PlayModeUseStartScene", EditorSceneManager.playModeStartScene != null);
        return !EditorApplication.isPlaying;
    }

    [MenuItem("Tools/PlayModeUseStartScene")]
    private static void SetPlayModeStartScene () {
        if (Menu.GetChecked("Tools/PlayModeUseStartScene")) {
            EditorSceneManager.playModeStartScene = null;
        } else {
            SceneAsset scene = AssetDatabase.LoadAssetAtPath<SceneAsset>(EditorBuildSettings.scenes[0].path);
            EditorSceneManager.playModeStartScene = scene;
        }
    }
}