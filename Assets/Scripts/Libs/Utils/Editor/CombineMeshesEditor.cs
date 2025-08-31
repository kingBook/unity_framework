#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class CombineMeshesEditor : ScriptableObject {

    private static bool s_isExecute;

    [MenuItem("GameObject/CombineMeshes", true)]
    private static bool ValidateCombineMeshes() {
        GameObject[] gameObjects = Selection.gameObjects;
        return gameObjects.Length > 0;
    }

    [MenuItem("GameObject/CombineMeshes", false, 11)]
    private static void CombineMeshes() {
        if (s_isExecute) {
            s_isExecute = false;
            Combine();
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


    private static void Combine() {
        List<Material> materials = new List<Material>();
        List<CombineInstance> combineInstances = new List<CombineInstance>();

        for (int i = 0; i < Selection.objects.Length; i++) {
            GameObject gameObject = Selection.objects[i] as GameObject;
            if (gameObject == null) continue;
            MeshFilter filter = gameObject.GetComponent<MeshFilter>();
            if (filter == null || filter.sharedMesh == null) continue;
            EditorUtility.DisplayProgressBar("Combine Meshes", "Combine " + filter.sharedMesh.name, (float)i + 1 / Selection.objects.Length);

            combineInstances.Add(new CombineInstance() {
                mesh = filter.sharedMesh,
                transform = filter.transform.localToWorldMatrix
            });

            MeshRenderer renderer = gameObject.GetComponent<MeshRenderer>();
            materials.AddRange(renderer.sharedMaterials);
        }
        EditorUtility.ClearProgressBar();
        GameObject combine = new GameObject("Combined Mesh");
        MeshFilter meshFilter = combine.AddComponent<MeshFilter>();
        meshFilter.mesh = new Mesh { name = $"Combined Mesh {Guid.NewGuid()}" };
        meshFilter.sharedMesh.CombineMeshes(combineInstances.ToArray(), true);
        MeshRenderer meshRenderer = combine.AddComponent<MeshRenderer>();
        meshRenderer.materials = materials.ToArray();
    }
}
#endif
