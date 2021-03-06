﻿#if UNITY_EDITOR

using System.Threading.Tasks;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

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



    // 加载 Unity 编辑器时运行
    [InitializeOnLoadMethod]
    private static void InitOnLoad () {
        Debug.Log("InitOnLoad");
        EditorApplication.playModeStateChanged -= OnPlayerModeStateChanged;
        EditorApplication.playModeStateChanged += OnPlayerModeStateChanged;
    }


    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void InitOnLoad2 () {
        Debug.Log("InitOnLoad2");
        //EditorSceneManager.sceneLoaded += OnSceneLoaded;
        EditorSceneManager.activeSceneChanged += OnActiveSceneChanged;
    }

    private static void OnPlayerModeStateChanged (PlayModeStateChange playModeState) {
        Debug.LogFormat("state:{0} will:{1} isPlaying:{2}", playModeState, EditorApplication.isPlayingOrWillChangePlaymode, EditorApplication.isPlaying);
        switch (playModeState) {
            case PlayModeStateChange.EnteredEditMode:
                break;
            case PlayModeStateChange.ExitingEditMode:
                /*Debug.Log("sceneCount:" + EditorSceneManager.sceneCount);
                for (int i = 0; i < EditorSceneManager.sceneCount; i++) {
                    Scene scene = EditorSceneManager.GetSceneAt(i);
                    if (scene.IsValid() && scene.isLoaded) {
                        Debug2.Log(scene.name);
                        Debug.Log(scene.GetRootGameObjects()[0].name);
                    }
                }*/

                

                break;
            case PlayModeStateChange.EnteredPlayMode:
                //EditorApplication.playModeStateChanged -= OnPlayerModeStateChanged;
                break;
            case PlayModeStateChange.ExitingPlayMode:
                //EditorSceneManager.sceneLoaded -= OnSceneLoaded;
                EditorSceneManager.activeSceneChanged -= OnActiveSceneChanged;
                break;
        }
    }

    private static void OnActiveSceneChanged (Scene current, Scene next) {
        Debug.Log("== OnActiveSceneChanged:");
        Debug.Log(next.GetRootGameObjects()[2].name);

        HierarchyUtil.SetExpandedRecursive(next.GetRootGameObjects()[2],true);
    }


    private static void OnSceneLoaded (Scene scene, LoadSceneMode mode) {
        /*Debug.Log("== OnSceneLoaded");
        if (scene.IsValid() && scene.isLoaded) {
            Debug2.Log(scene.name);
            Debug.Log(scene.GetRootGameObjects()[0].name);
        }*/

        
    }


}

#endif