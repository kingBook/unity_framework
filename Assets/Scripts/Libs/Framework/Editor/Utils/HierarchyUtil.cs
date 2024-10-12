#if UNITY_EDITOR

using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.SceneManagement;

public class HierarchyUtil {

    /// <summary>
    /// Check if the target GameObject is expanded (aka unfolded) in the Hierarchy view.
    /// </summary>
    public static bool IsExpanded(GameObject go) {
        return GetExpandedGameObjects().Contains(go);
    }

    /// <summary>
    /// Get a list of all GameObjects which are expanded (aka unfolded) in the Hierarchy view.
    /// </summary>
    public static List<GameObject> GetExpandedGameObjects() {
        object sceneHierarchy = GetSceneHierarchy();

        MethodInfo methodInfo = sceneHierarchy
            .GetType()
            .GetMethod("GetExpandedGameObjects");

        object result = methodInfo.Invoke(sceneHierarchy, new object[0]);
        return (List<GameObject>)result;
    }

    /// <summary>
    /// Set the target GameObject as expanded (aka unfolded) in the Hierarchy view.
    /// </summary>
    public static void SetExpanded(GameObject go, bool expand) {
        object sceneHierarchy = GetSceneHierarchy();

        MethodInfo methodInfo = sceneHierarchy
            .GetType()
            .GetMethod("ExpandTreeViewItem", BindingFlags.NonPublic | BindingFlags.Instance);

        methodInfo.Invoke(sceneHierarchy, new object[] { go.GetInstanceID(), expand });
    }

    public static void SetExpanded(int instanceID, bool expand) {
        object sceneHierarchy = GetSceneHierarchy();

        MethodInfo methodInfo = sceneHierarchy
            .GetType()
            .GetMethod("ExpandTreeViewItem", BindingFlags.NonPublic | BindingFlags.Instance);

        methodInfo.Invoke(sceneHierarchy, new object[] { instanceID, expand });
    }

    /// <summary>
    /// Set the target GameObject and all children as expanded (aka unfolded) in the Hierarchy view.
    /// </summary>
    public static void SetExpandedRecursive(GameObject go, bool expand) {
        object sceneHierarchy = GetSceneHierarchy();

        MethodInfo methodInfo = sceneHierarchy
            .GetType()
            .GetMethod("SetExpandedRecursive", BindingFlags.Public | BindingFlags.Instance);

        methodInfo.Invoke(sceneHierarchy, new object[] { go.GetInstanceID(), expand });
    }

    public static void SetExpandedRecursive(int instanceID, bool expand) {
        object sceneHierarchy = GetSceneHierarchy();

        MethodInfo methodInfo = sceneHierarchy
            .GetType()
            .GetMethod("SetExpandedRecursive", BindingFlags.Public | BindingFlags.Instance);

        methodInfo.Invoke(sceneHierarchy, new object[] { instanceID, expand });
    }

    private static object GetSceneHierarchy() {
        EditorWindow window = GetHierarchyWindow();

        object sceneHierarchy = typeof(EditorWindow).Assembly
            .GetType("UnityEditor.SceneHierarchyWindow")
            .GetProperty("sceneHierarchy")
            .GetValue(window);

        return sceneHierarchy;
    }

    private static EditorWindow GetHierarchyWindow() {
        // old
        // For it to open, so that it the current focused window.
        // EditorApplication.ExecuteMenuItem("Window/General/Hierarchy");
        // return EditorWindow.focusedWindow;

        // new
        Assembly editorAssembly = typeof(EditorWindow).Assembly;
        System.Type HierarchyWindowType = editorAssembly.GetType("UnityEditor.SceneHierarchyWindow");
        EditorWindow hierarchyWindow = EditorWindow.GetWindow(HierarchyWindowType);
        return hierarchyWindow;
    }
}

#endif