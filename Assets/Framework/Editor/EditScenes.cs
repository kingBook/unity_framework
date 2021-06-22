#if UNITY_EDITOR

using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EditScenes : Editor {

    private static int s_currentIndex;
    private static string[] s_scenePaths;
    private static System.Action<Scene> s_onOpendCallback;

    [MenuItem("Tools/EditScenes", true)]
    private static bool ValidateMenuItem () {
        return !EditorApplication.isPlaying;
    }

    [MenuItem("Tools/EditScenes")]
    public static void Main () {
        if (EditorApplication.isPlaying) return;
        Debug.Log("== Tools/EditScenes ==");

        List<string> scenePaths = new List<string>();
        for (int i = 1; i <= 80; i++) {
            scenePaths.Add("Assets/Scenes/Level_" + i + ".unity");
        }

        OpenScenesOneByOne(scenePaths.ToArray(), (Scene scene) => {
            Debug2.Log(scene.name);

            //GameObject gameObj=GameObject.Find("CanvasLevel/PanelLevel/SafeAreaGroup/ButtonShop");
            //获取 Transform 序列化对象
            //RectTransform rt=(RectTransform)gameObj.transform;
            //SerializedObject so=new SerializedObject(rt);

            //遍历查看属性
            /*var prop=so.GetIterator();
			while(prop.Next(true)){
				Debug.Log(prop.name);
			}*/

            //Revert GameObject or Component
            //PrefabUtility.RevertObjectOverride(rt,InteractionMode.AutomatedAction);

            //Revert a property
            /*SerializedProperty propAnchoredPos=so.FindProperty("m_AnchoredPosition");
			PrefabUtility.RevertPropertyOverride(propAnchoredPos,InteractionMode.AutomatedAction);*/

            //修改后，必须标记'已编辑'，否则保存场景无效
            //EditorSceneManager.MarkSceneDirty(scene);
        });
    }

    private static void OpenScenesOneByOne (string[] scenePaths, System.Action<Scene> onOpend) {
        s_scenePaths = scenePaths;
        s_currentIndex = 0;
        s_onOpendCallback = onOpend;
        EditorSceneManager.sceneOpened += OnSceneOpened;
        EditorSceneManager.OpenScene(s_scenePaths[s_currentIndex]);


    }

    private static void OnSceneOpened (Scene scene, OpenSceneMode mode) {

        s_onOpendCallback.Invoke(scene);

        //保存前需要对已编辑的场景，使用 EditorSceneManager.MarkSceneDirty(scene) 标记
        EditorSceneManager.SaveOpenScenes();

        int maxIndex = s_scenePaths.Length - 1;
        if (s_currentIndex < maxIndex) {
            s_currentIndex++;
            EditorSceneManager.OpenScene(s_scenePaths[s_currentIndex]);
        } else {
            EditorSceneManager.sceneOpened -= OnSceneOpened;
            s_scenePaths = null;
            s_onOpendCallback = null;
            s_currentIndex = 0;
        }
    }
}

#endif