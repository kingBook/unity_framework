#if UNITY_EDITOR

using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;


[System.Serializable]
public class SceneData {
    public string name;
    public int buildIndex;
    public List<GameObjectData> rootGameObjectDatas = new List<GameObjectData>();
}

[System.Serializable]
public class GameObjectData {
    public string name;
    public bool expanded;
    public bool selection;
    /// <summary> 仅限编辑器模式下 </summary>
    public int instanceID;
    public List<GameObjectData> children = new List<GameObjectData>();
}


/// <summary>
/// 设置进入播放模式时默认启动的场景(选择菜单 Tools/PlayModeUseStartScene 更改，将启动 BuildSettings 界面中的 buildIndex 为 [0] 的场景)
/// </summary>
public class PlayModeStartSceneEditor : Editor {

    [MenuItem("Tools/PlayModeUseStartScene", true)]
    private static bool ValidateMenuItem() {
        Menu.SetChecked("Tools/PlayModeUseStartScene", EditorSceneManager.playModeStartScene != null);
        return !EditorApplication.isPlaying;
    }

    [MenuItem("Tools/PlayModeUseStartScene")]
    private static void SetPlayModeStartScene() {
        if (Menu.GetChecked("Tools/PlayModeUseStartScene")) {
            EditorSceneManager.playModeStartScene = null;
        } else {
            SceneAsset scene = AssetDatabase.LoadAssetAtPath<SceneAsset>(EditorBuildSettings.scenes[0].path);
            EditorSceneManager.playModeStartScene = scene;
        }
    }


    // 第一次打开 Unity 编辑器运行一次，之后每次进入 Play 模式都运行一次
    [InitializeOnLoadMethod]
    private static void InitOnLoad() {
        // 侦听播放模式改变事件
        EditorApplication.playModeStateChanged -= OnPlayerModeStateChanged;
        EditorApplication.playModeStateChanged += OnPlayerModeStateChanged;

        // 侦听激活的场景改变事件
        EditorSceneManager.activeSceneChanged -= OnActiveSceneChanged;
        EditorSceneManager.activeSceneChanged += OnActiveSceneChanged;
    }

    private static void OnPlayerModeStateChanged(PlayModeStateChange playModeState) {
        switch (playModeState) {
            case PlayModeStateChange.EnteredEditMode:
                // 恢复编辑器模式下已选中的对象
                ResumeEditModeSelections();
                break;
            case PlayModeStateChange.ExitingEditMode:
                Scene editorActiveScene = EditorSceneManager.GetActiveScene();
                // 保存编辑器模式下，展开节点的数据
                RecordSceneToLocal(editorActiveScene);
                break;
            case PlayModeStateChange.EnteredPlayMode:

                break;
            case PlayModeStateChange.ExitingPlayMode:

                break;
        }
    }

    private static void OnActiveSceneChanged(Scene current, Scene next) {
        Scene runtimeActiveScene = UnityEngine.SceneManagement.SceneManager.GetActiveScene();

        string path = System.Environment.CurrentDirectory + $"/Temp/{runtimeActiveScene.name + runtimeActiveScene.buildIndex}.sceneData";
        if (File.Exists(path)) {
            List<Object> selections = new List<Object>();

            byte[] bytes = File.ReadAllBytes(path);
            MemoryStream memoryStream = new MemoryStream(bytes);
            BinaryFormatter binaryFormatter = new BinaryFormatter();

            SceneData sceneData = (SceneData)binaryFormatter.Deserialize(memoryStream);
            memoryStream.Close();


            HierarchyUtil.SetExpanded(runtimeActiveScene.GetHashCode(), true); // 展开激活的场景
            SetSceneWithExpandedData(runtimeActiveScene, sceneData, ref selections);

            // 设置选中的 GameObject
            Selection.objects = selections.ToArray();

        }
    }

    private static void RecordSceneToLocal(Scene scene) {
        SceneData sceneData = GetSceneExpandedData(scene);

        string path = System.Environment.CurrentDirectory + $"/Temp/{sceneData.name + sceneData.buildIndex}.sceneData";
        FileStream fileStream = new FileStream(path, FileMode.Create, FileAccess.Write, FileShare.ReadWrite);
        BinaryFormatter binaryFormatter = new BinaryFormatter();
        binaryFormatter.Serialize(fileStream, sceneData);
        fileStream.Close();
    }

    private static void SetSceneWithExpandedData(Scene scene, SceneData sceneData, ref List<Object> selections) {
        GameObject[] rootGameObjects = scene.GetRootGameObjects();
        int rootCount = rootGameObjects.Length;
        for (int i = 0, len = sceneData.rootGameObjectDatas.Count; i < len; i++) {
            if (i >= rootCount) break;
            GameObject rootGameObject = rootGameObjects[i];
            GameObjectData rootGameObjectData = sceneData.rootGameObjectDatas[i];
            SetGameObjectExpandedWithData(rootGameObject, rootGameObjectData, ref selections);
        }
    }

    private static void SetGameObjectExpandedWithData(GameObject gameObject, GameObjectData gameObjectData, ref List<Object> selections) {
        if (gameObject.name != gameObjectData.name) return;

        if (gameObjectData.expanded) {
            HierarchyUtil.SetExpanded(gameObject, true);
        }

        if (gameObjectData.selection) {
            selections.Add(gameObject);
        }

        int childCount = gameObject.transform.childCount;
        for (int i = 0, len = gameObjectData.children.Count; i < len; i++) {
            if (i >= childCount) break;
            GameObject child = gameObject.transform.GetChild(i).gameObject;
            GameObjectData childData = gameObjectData.children[i];
            SetGameObjectExpandedWithData(child, childData, ref selections);
        }
    }

    /// <summary>  返回一个场景展开节点的数据，不会返回 null </summary>
    private static SceneData GetSceneExpandedData(Scene scene) {
        GameObject[] expandedGameObjects = HierarchyUtil.GetExpandedGameObjects().ToArray();

        SceneData sceneData = new SceneData();
        sceneData.name = scene.name;
        sceneData.buildIndex = scene.buildIndex;
        GameObject[] rootGameObjects = scene.GetRootGameObjects();
        for (int i = 0, len = rootGameObjects.Length; i < len; i++) {
            GameObjectData rootGameOjbectData = GetGameObjectExpandedData(rootGameObjects[i], expandedGameObjects);
            sceneData.rootGameObjectDatas.Add(rootGameOjbectData);
        }
        return sceneData;
    }

    /// <summary> 返回一个 GameObject 的节点展开数据，不会返回 null </summary>
    private static GameObjectData GetGameObjectExpandedData(GameObject gameObject, GameObject[] expandedGameObjects) {
        GameObjectData data = new GameObjectData();
        data.name = gameObject.name;
        data.expanded = System.Array.IndexOf(expandedGameObjects, gameObject) > -1;
        data.selection = System.Array.IndexOf(Selection.gameObjects, gameObject) > -1;
        data.instanceID = gameObject.GetInstanceID();
        for (int i = 0, len = gameObject.transform.childCount; i < len; i++) {
            GameObject child = gameObject.transform.GetChild(i).gameObject;
            GameObjectData childData = GetGameObjectExpandedData(child, expandedGameObjects);
            data.children.Add(childData);
        }
        return data;
    }

    private static void ResumeEditModeSelections() {
        Scene activeScene = EditorSceneManager.GetActiveScene();

        List<Object> selections = new List<Object>();

        string path = System.Environment.CurrentDirectory + $"/Temp/{activeScene.name + activeScene.buildIndex}.sceneData";
        if (File.Exists(path)) {
            byte[] bytes = File.ReadAllBytes(path);
            MemoryStream memoryStream = new MemoryStream(bytes);
            BinaryFormatter binaryFormatter = new BinaryFormatter();

            SceneData sceneData = (SceneData)binaryFormatter.Deserialize(memoryStream);
            memoryStream.Close();

            GetSelections(activeScene, sceneData, ref selections);
        }

        // 设置选中的 GameObject
        Selection.objects = selections.ToArray();
    }

    private static void GetSelections(Scene scene, SceneData sceneData, ref List<Object> selections) {
        GameObject[] rootGameObjects = scene.GetRootGameObjects();
        int rootCount = rootGameObjects.Length;
        for (int i = 0, len = sceneData.rootGameObjectDatas.Count; i < len; i++) {
            if (i >= rootCount) break;
            GameObject rootGameObject = rootGameObjects[i];
            GameObjectData rootGameObjectData = sceneData.rootGameObjectDatas[i];
            GetSelections(rootGameObject, rootGameObjectData, ref selections);
        }
    }

    private static void GetSelections(GameObject gameObject, GameObjectData gameObjectData, ref List<Object> selections) {
        if (gameObject.name != gameObjectData.name) return;

        if (gameObjectData.selection) {
            selections.Add(gameObject);
        }

        int childCount = gameObject.transform.childCount;
        for (int i = 0, len = gameObjectData.children.Count; i < len; i++) {
            if (i >= childCount) break;
            GameObject child = gameObject.transform.GetChild(i).gameObject;
            GameObjectData childData = gameObjectData.children[i];
            GetSelections(child, childData, ref selections);
        }
    }
}

#endif