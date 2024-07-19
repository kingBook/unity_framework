#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

public class TestHierarchyPopupMenuEditor : ScriptableObject {


    [MenuItem("GameObject/Create New Child", true)]
    private static bool ValidateCreateChild() {
        Debug.Log("ValidateCreateChild");
        GameObject[] gameObjects = Selection.gameObjects;
        return gameObjects.Length > 0;
    }


    [MenuItem("GameObject/Create New Child", false, 11)]
    private static void CreateChild() {
        if (Event.current!=null) {
            Debug.Log($"{Event.current.displayIndex}");
        }
        /*GameObject[] gameObjects = Selection.gameObjects;
        for (int i = 0; i < gameObjects.Length; i++) {
            var child = new GameObject("New Child");
            child.transform.parent = gameObjects[i].transform;
        }*/
    }




}
#endif
