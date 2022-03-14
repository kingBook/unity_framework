﻿#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

// TreeView https://docs.unity.cn/cn/2019.4/Manual/TreeViewAPI.html
public class ExportPackgePlusEditorWindow : EditorWindow {

    // SerializeField 用于确保将视图状态写入窗口
    // 布局文件。这意味着只要窗口未关闭，即使重新启动 Unity，也会保持
    // 状态。如果省略该属性，仍然会序列化/反序列化状态。
    [SerializeField] private TreeViewState m_treeViewState;

    //TreeView 不可序列化，因此应该通过树数据对其进行重建。
    private SimpleTreeView m_simpleTreeView;

    private void OnEnable () {
        //检查是否已存在序列化视图状态（在程序集重新加载后
        // 仍然存在的状态）
        if (m_treeViewState == null) {
            m_treeViewState = new TreeViewState();
        }
        m_simpleTreeView = new SimpleTreeView(m_treeViewState);
    }

    private void OnGUI () {
        EditorGUILayout.BeginVertical();
        {
            Rect contentRect = new Rect(0, 0, position.width, position.height);
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("sdfsf");
            EditorGUILayout.EndHorizontal();

            Rect treeViewRect = GUILayoutUtility.GetRect(0f, 1e5f, 0f, 1e5f);
            m_simpleTreeView.OnGUI(treeViewRect);

            EditorGUILayout.BeginHorizontal();
            GUILayout.Button("All");
            GUILayout.Button("None");
            GUILayout.Toggle(true, "Include dependencies");
            GUILayout.FlexibleSpace();
            GUILayout.Button("Export...");
            EditorGUILayout.EndHorizontal();
        }
        EditorGUILayout.EndVertical();
    }


    [MenuItem("Assets/Export Package Plus...", false, 20)]
    private static void DisplayWindow () {
        var window = GetWindow<ExportPackgePlusEditorWindow>(true);
        window.titleContent = new GUIContent("Exporting Package");
        window.Show();
    }


    /*[MenuItem("Assets/Export Package (Assets And ProjectSettings)", false, 20)]
    private static void Execute () {
        Debug.Log("Export Package (Assets And ProjectSettings)");

        List<string> assetPathNames = new List<string>();
        string[] paths = AssetDatabase.GetAllAssetPaths();
        for (int i = 0, length = paths.Length; i < length; i++) {
            string path = paths[i];
            if (path.IndexOf("ProjectSettings") == 0) {
                assetPathNames.Add(path);
            } else if (path.IndexOf("Assets") == 0) {
                assetPathNames.Add(path);
            } else if (path.IndexOf("Packages") == 0) {

            } else if (path.IndexOf("Library") == 0) {

            } else {

            }
        }
        AssetDatabase.ExportPackage(assetPathNames.ToArray(), "Assets.unitypackage", ExportPackageOptions.Interactive);
    }*/
}

internal class SimpleTreeView : TreeView {
    public SimpleTreeView (TreeViewState treeViewState) : base(treeViewState) {
        Reload();
    }

    protected override TreeViewItem BuildRoot () {
        List<string> assetPathNames = new List<string>();
        string[] paths = AssetDatabase.GetAllAssetPaths();
        for (int i = 0, length = paths.Length; i < length; i++) {
            string path = paths[i];
            if (path.IndexOf("ProjectSettings") == 0) {
                assetPathNames.Add(path);
            } else if (path.IndexOf("Assets") == 0) {
                assetPathNames.Add(path);
                Debug.Log(path);
            } else if (path.IndexOf("Packages") == 0) {

            } else if (path.IndexOf("Library") == 0) {

            } else {

            }
        }


        var root = new TreeViewItem { id = 0, depth = -1, displayName = "Root" };
        var allItems = new List<TreeViewItem> {
                new TreeViewItem {id = 1, depth = 0, displayName = "Animals"},
                new TreeViewItem {id = 2, depth = 1, displayName = "Mammals"},
                new TreeViewItem {id = 3, depth = 2, displayName = "Tiger"},
                new TreeViewItem {id = 4, depth = 2, displayName = "Elephant"},
                new TreeViewItem {id = 5, depth = 2, displayName = "Okapi"},
                new TreeViewItem {id = 6, depth = 2, displayName = "Armadillo"},
                new TreeViewItem {id = 7, depth = 1, displayName = "Reptiles"},
                new TreeViewItem {id = 8, depth = 2, displayName = "Crocodile"},
                new TreeViewItem {id = 9, depth = 2, displayName = "Lizard"},
        };
        SetupParentsAndChildrenFromDepths(root, allItems);
        return root;
    }

    protected override void RowGUI (RowGUIArgs args) {
        var item = args.item;

        Rect toggleRect = args.rowRect;
        toggleRect.x += GetContentIndent(item);
        toggleRect.width = 18f;
        EditorGUI.Toggle(toggleRect, true);

        args.rowRect.x = toggleRect.width;
        base.RowGUI(args);
    }

}
#endif