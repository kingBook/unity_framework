﻿#if UNITY_EDITOR

using UnityEditor;
using UnityEngine;

/// <summary>
/// 在 Hierarchy 视图对象的快捷菜单中增加 Flip Mesh Normals（反转网格法线）项
/// </summary>
public class FlipMeshNormalsEditor : Editor {

    private static bool s_isExecute;
    
    /// <summary> 翻转网格的法线 </summary>
    private static void FlipMeshNormals(Mesh mesh) {
        int[] triangles = mesh.triangles;
        for (int i = 0, len = triangles.Length; i < len; i += 3) {
            // 交换三角形的首尾索引
            int t = triangles[i];
            triangles[i] = triangles[i + 2];
            triangles[i + 2] = t;
        }
        mesh.triangles = triangles;

    }

    /// <summary> 翻转多个游戏对象网格的法线 </summary>
    private static void FlipMeshNormals(GameObject[] gameObjects) {
        for (int i = 0, len = gameObjects.Length; i < len; i++) {
            GameObject go = gameObjects[i];

            Mesh mesh = null;
            SkinnedMeshRenderer skinnedMeshRenderer = go.GetComponent<SkinnedMeshRenderer>();
            if (skinnedMeshRenderer) {
                mesh = skinnedMeshRenderer.sharedMesh;
            } else {
                MeshFilter meshFilter = go.GetComponent<MeshFilter>();
                if (meshFilter) {
                    mesh = meshFilter.sharedMesh;
                }
            }

            if (mesh) {
                string path = AssetDatabase.GetAssetPath(mesh);
                // 是否为 Assets 文件夹下的资源（Assets 文件夹下的资源才能编辑，避免编辑到 Unity 的内置资源的网格）
                bool isAssetFolder = path.IndexOf("Assets") == 0;
                if (isAssetFolder) {
                    FlipMeshNormals(mesh);
                }
            }
        }
    }

    /// <summary> 验证所选择的游戏对象有网格时菜单才可用(不计算子级) </summary>
    [MenuItem("GameObject/Flip Mesh Normals", true)]
    private static bool ValidateFlipMeshNormalsOnGameObject() {
        bool isEnableMenuItem = false;
        GameObject[] gameObjects = Selection.gameObjects;
        for (int i = 0, len = gameObjects.Length; i < len; i++) {
            GameObject go = gameObjects[i];
            Mesh mesh = null;
            SkinnedMeshRenderer skinnedMeshRenderer = go.GetComponent<SkinnedMeshRenderer>();
            if (skinnedMeshRenderer) {
                mesh = skinnedMeshRenderer.sharedMesh;
            } else {
                MeshFilter meshFilter = go.GetComponent<MeshFilter>();
                if (meshFilter) {
                    mesh = meshFilter.sharedMesh;
                }
            }

            if (mesh) {
                string path = AssetDatabase.GetAssetPath(mesh);
                // 是否为 Assets 文件夹下的资源（Assets 文件夹下的资源才能编辑，避免编辑到 Unity 的内置资源的网格）
                bool isAssetFolder = path.IndexOf("Assets") == 0;
                if (isAssetFolder) {
                    isEnableMenuItem = true;
                    break;
                }
            }
        }
        return isEnableMenuItem;
    }

    [MenuItem("GameObject/Flip Mesh Normals", false, 11)]
    private static void FlipMeshNormalsOnGameObject() {
        if (s_isExecute) {
            s_isExecute = false;
            FlipMeshNormals(Selection.gameObjects);
        }
    }
    
    [InitializeOnLoadMethod]
    private static void StartInitializeOnLoadMethod() {
        EditorApplication.hierarchyWindowItemOnGUI += OnHierarchyGUI;
    }

    private static void OnHierarchyGUI(int instanceID, Rect selectionRect) {
        if (Event.current != null && Event.current.button == 1 && Event.current.type == EventType.MouseUp) {
            s_isExecute = true;
        }
    }

}
#endif