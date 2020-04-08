using UnityEditor;
using UnityEditor.SceneManagement;

public class PlayModeStartSceneEditor:Editor{

	[MenuItem("Tools/PlayModeUseStartScene",true)]
	static bool ValidatePlayModeUseStartScene(){
		Menu.SetChecked("Tools/PlayModeUseStartScene",EditorSceneManager.playModeStartScene!=null);
		return !EditorApplication.isPlaying;
	}

	[MenuItem("Tools/PlayModeUseStartScene")]
	static void SetPlayModeStartScene(){
		if (Menu.GetChecked("Tools/PlayModeUseStartScene")){
			EditorSceneManager.playModeStartScene=null;
		}else{
			SceneAsset scene=AssetDatabase.LoadAssetAtPath<SceneAsset>(EditorBuildSettings.scenes[0].path);
			EditorSceneManager.playModeStartScene=scene;
		}
	}
}