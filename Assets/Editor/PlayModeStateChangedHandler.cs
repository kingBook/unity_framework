using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;

/// <summary>
/// 处理播放模式改变时，在不打开main场景时进入播放模式主动载入main场景
/// </summary>
public static class PlayModeStateChangedHandler{
	
	//记录播放前数据的.asset文件保存路径
	private static readonly string m_playBeforeDataPath="Assets/Editor/playBeforeData.asset";
	//main场景路径
	private static readonly string m_mainScenePath="Assets/Scenes/main.unity";
	
	//初始化类时,注册事件处理函数
	[InitializeOnLoadMethod]
	static void OnProjectLoadedInEditor(){
		EditorApplication.playModeStateChanged+=OnPlayModeStateChanged;
		EditorSceneManager.sceneLoaded+=OnSceneLoaded;
	}

	private static void OnPlayModeStateChanged(PlayModeStateChange playModeState){
		if(playModeState==PlayModeStateChange.ExitingEditMode){
			//====退出编辑模式时====
			if(EditorSceneManager.loadedSceneCount>0){
				//保存播放前Hierarchy中激活的场景到本地
				var playBeforeData=ScriptableObject.CreateInstance<PlayBeforeData>();
				playBeforeData.activeScenePath=EditorSceneManager.GetActiveScene().path;
				for(int i=0;i<EditorSceneManager.loadedSceneCount;i++){
					Scene scene=EditorSceneManager.GetSceneAt(i);
					if(scene.path==m_mainScenePath){
						playBeforeData.isOpenedMainScene=true;
						break;
					}
				}
				AssetDatabase.CreateAsset(playBeforeData,m_playBeforeDataPath);
				//打开main场景，并激活
				Scene mainScene=EditorSceneManager.OpenScene(m_mainScenePath,OpenSceneMode.Additive);
				EditorSceneManager.SetActiveScene(mainScene);
			}
		}else if(playModeState==PlayModeStateChange.EnteredEditMode){
			//=====进入编辑模式时====
			var playBeforeData=AssetDatabase.LoadAssetAtPath<PlayBeforeData>(m_playBeforeDataPath);
			for(int i=0;i<EditorSceneManager.loadedSceneCount;i++){
				Scene scene=EditorSceneManager.GetSceneAt(i);
				if(scene.path==m_mainScenePath){
					if(!playBeforeData.isOpenedMainScene){
						//如果main场景在播放前未打开，则关闭main场景
						EditorSceneManager.CloseScene(scene,true);
					}
				}else if(scene.path==playBeforeData.activeScenePath){
					//还原播放前Hierarchy中激活的场景
					EditorSceneManager.SetActiveScene(scene);
					AssetDatabase.DeleteAsset(m_playBeforeDataPath);
				}
			}
		}
	}
	
	private static void OnSceneLoaded(Scene scene,LoadSceneMode mode){
		if(EditorApplication.isPlaying){
			var playBeforeData=AssetDatabase.LoadAssetAtPath<PlayBeforeData>(m_playBeforeDataPath);
			if(scene.path==playBeforeData.activeScenePath){
				Scene mainScene=EditorSceneManager.GetSceneByPath(m_mainScenePath);
				if(mainScene.isLoaded){
					//激活播放前在Hierarchy中激活的场景
					EditorSceneManager.SetActiveScene(scene);
				}
			}
		}
	}

	
}