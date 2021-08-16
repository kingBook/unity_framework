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
        EditorSceneManager.sceneLoaded -= OnSceneLoaded;
        EditorSceneManager.sceneLoaded += OnSceneLoaded;

        EditorSceneManager.activeSceneChanged -= OnActiveSceneChanged;
        EditorSceneManager.activeSceneChanged += OnActiveSceneChanged;
    }

    private static void OnPlayerModeStateChanged (PlayModeStateChange playModeState) {
        Debug.LogFormat("state:{0} will:{1} isPlaying:{2}", playModeState, EditorApplication.isPlayingOrWillChangePlaymode, EditorApplication.isPlaying);
        switch (playModeState) {
            case PlayModeStateChange.EnteredEditMode:
                ResumeEditModeSelections();
                break;
            case PlayModeStateChange.ExitingEditMode:
                Scene editorActiveScene = EditorSceneManager.GetActiveScene();
                int instanceID = editorActiveScene.GetRootGameObjects()[0].GetInstanceID();
                Debug2.Log("ExitingEditMode", instanceID, HierarchyUtil.IsExpanded(editorActiveScene.GetRootGameObjects()[0]));
                RecordSceneToLocal(editorActiveScene);
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
        //Debug.Log(next.GetRootGameObjects()[2].name);

        //HierarchyUtil.SetExpandedRecursive(next.GetRootGameObjects()[2],true);

        Scene runtimeActiveScene = UnityEngine.SceneManagement.SceneManager.GetActiveScene();
        //Debug2.Log("OnActiveSceneChanged", runtimeActiveScene.GetRootGameObjects()[3].name, runtimeActiveScene.GetRootGameObjects()[3].GetInstanceID());

        //Debug2.Log(runtimeActiveScene.name, current.name, next.name);


        List<Object> selections = new List<Object>();

        string path = System.Environment.CurrentDirectory + $"/Temp/{runtimeActiveScene.name + runtimeActiveScene.buildIndex}.sceneData";
        if (File.Exists(path)) {
            byte[] bytes = File.ReadAllBytes(path);
            MemoryStream memoryStream = new MemoryStream(bytes);
            BinaryFormatter binaryFormatter = new BinaryFormatter();

            SceneData sceneData = (SceneData)binaryFormatter.Deserialize(memoryStream);
            memoryStream.Close();

            HierarchyUtil.SetExpanded(runtimeActiveScene.GetHashCode(), true); // 展开激活的场景
            SetSceneWithExpandedData(runtimeActiveScene, sceneData, ref selections);
        }

        // 设置选中的 GameObject
        Selection.objects = selections.ToArray();
    }


    private static void OnSceneLoaded (Scene scene, LoadSceneMode mode) {
        //Debug2.Log("OnSceneLoaded", scene.GetRootGameObjects()[3].name, scene.GetRootGameObjects()[3].GetInstanceID());
        /*Debug.Log("== OnSceneLoaded");
        if (scene.IsValid() && scene.isLoaded) {
            Debug2.Log(scene.name);
            Debug.Log(scene.GetRootGameObjects()[0].name);

        }
        */
    }

    private static void RecordSceneToLocal (Scene scene) {
        Debug.Log("=== RecordSceneToLocal");

        SceneData sceneData = GetSceneExpandedData(scene);

        string path = System.Environment.CurrentDirectory + $"/Temp/{sceneData.name + sceneData.buildIndex}.sceneData";
        FileStream fileStream = new FileStream(path, FileMode.Create, FileAccess.Write, FileShare.ReadWrite);
        BinaryFormatter binaryFormatter = new BinaryFormatter();
        binaryFormatter.Serialize(fileStream, sceneData);
        fileStream.Close();
    }

    private static void SetSceneWithExpandedData (Scene scene, SceneData sceneData, ref List<Object> selections) {
        GameObject[] rootGameObjects = scene.GetRootGameObjects();
        int rootCount = rootGameObjects.Length;
        for (int i = 0, len = sceneData.rootGameObjectDatas.Count; i < len; i++) {
            if (i >= rootCount) break;
            GameObject rootGameObject = rootGameObjects[i];
            GameObjectData rootGameObjectData = sceneData.rootGameObjectDatas[i];
            SetGameObjectExpandedWithData(rootGameObject, rootGameObjectData, ref selections);
        }
    }

    private static void SetGameObjectExpandedWithData (GameObject gameObject, GameObjectData gameObjectData, ref List<Object> selections) {
        if (gameObject.name != gameObjectData.name) return;

        HierarchyUtil.SetExpanded(gameObject, gameObjectData.expanded);

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
    private static SceneData GetSceneExpandedData (Scene scene) {
        SceneData sceneData = new SceneData();
        sceneData.name = scene.name;
        sceneData.buildIndex = scene.buildIndex;
        GameObject[] rootGameObjects = scene.GetRootGameObjects();
        for (int i = 0, len = rootGameObjects.Length; i < len; i++) {
            GameObjectData rootGameOjbectData = GetGameObjectExpandedData(rootGameObjects[i]);
            sceneData.rootGameObjectDatas.Add(rootGameOjbectData);
        }
        return sceneData;
    }

    /// <summary> 返回一个 GameObject 的节点展开数据，不会返回 null </summary>
    private static GameObjectData GetGameObjectExpandedData (GameObject gameObject) {
        GameObjectData data = new GameObjectData();
        data.name = gameObject.name;
        data.expanded = HierarchyUtil.IsExpanded(gameObject);
        data.selection = System.Array.IndexOf(Selection.gameObjects, gameObject) > -1;
        data.instanceID = gameObject.GetInstanceID();
        for (int i = 0, len = gameObject.transform.childCount; i < len; i++) {
            GameObject child = gameObject.transform.GetChild(i).gameObject;
            GameObjectData childData = GetGameObjectExpandedData(child);
            data.children.Add(childData);
        }
        return data;
    }

    private static void ResumeEditModeSelections () {
        Debug.Log("=== ResumeEditModeSelections");

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

    private static void GetSelections (Scene scene, SceneData sceneData, ref List<Object> selections) {
        GameObject[] rootGameObjects = scene.GetRootGameObjects();
        int rootCount = rootGameObjects.Length;
        for (int i = 0, len = sceneData.rootGameObjectDatas.Count; i < len; i++) {
            if (i >= rootCount) break;
            GameObject rootGameObject = rootGameObjects[i];
            GameObjectData rootGameObjectData = sceneData.rootGameObjectDatas[i];
            GetSelections(rootGameObject, rootGameObjectData, ref selections);
        }
    }

    private static void GetSelections (GameObject gameObject, GameObjectData gameObjectData, ref List<Object> selections) {
        if (gameObject.name != gameObjectData.name) return;

        if (gameObjectData.selection) {
            Debug.Log(gameObject.name);
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