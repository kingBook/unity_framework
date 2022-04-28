using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

/// <summary>
/// 扩展 Edit/Duplicate Ctrl+Shift+D 向下复制对象菜单
/// </summary>
public class DuplicateInHierarchyEditor : ScriptableObject {

    [MenuItem("Edit/Duplicate Downward %#d", true)]
    private static bool ValidateDuplicateDownward() {
        return Selection.gameObjects.Length > 0;
    }

    //# 表示 shift
    //& 表示 alt
    //% 表示 Ctrl
    [MenuItem("Edit/Duplicate Downward %#d", false, 120)]
    private static void DuplicateDownward() {
        DuplicateDownward(Selection.gameObjects);
    }

    private static void DuplicateDownward(GameObject[] gameObjects) {
        // 去除是其它项的子级的项
        RemoveChildOfGameObjects(ref gameObjects);
        int len = gameObjects.Length;
        int[] instanceIDs = new int[len];

        // 新的实例放置在最大 SiblingIndex 的下方
        int maxSiblingIndex = 0;
        for (int i = 0; i < len; i++) {
            GameObject original = gameObjects[i];
            int siblingIndex = original.transform.GetSiblingIndex();
            maxSiblingIndex = Mathf.Max(siblingIndex, maxSiblingIndex);
        }

        for (int i = 0; i < len; i++) {
            GameObject original = gameObjects[i];
            GameObject inst = Instantiate(original, original.transform.parent, true);
            inst.name = GetNewInstanceName(original);
            Undo.RegisterCreatedObjectUndo(inst, $"Duplicate Downward {original.name}"); // 记录撤消
            maxSiblingIndex++;
            inst.transform.SetSiblingIndex(maxSiblingIndex);
            instanceIDs[i] = inst.GetInstanceID();

        }

        Selection.instanceIDs = instanceIDs;
    }

    private static string GetNewInstanceName(GameObject original) {
        string newName = $"{original.name} (1)";
        var regex = new Regex(@"\s\((?<number>\d+)\)", RegexOptions.RightToLeft);
        Match match = regex.Match(original.name);
        if (match.Success) {
            int n = int.Parse(match.Groups["number"].Value);
            // 命名
            while (true) {
                n++;
                newName = regex.Replace(original.name, $" ({n})", 1);
                if (original.transform.parent) {
                    if (!original.transform.parent.Find(newName)) {
                        break;
                    }
                } else {
                    GameObject hasMatchNameRootGameObject = GetMatchNameGameObject(original.scene.GetRootGameObjects(), newName);
                    if (!hasMatchNameRootGameObject) {
                        break;
                    }
                }
            }
        }
        return newName;
    }

    private static GameObject GetMatchNameGameObject(GameObject[] gameObjects, string name) {
        for (int i = 0, len = gameObjects.Length; i < len; i++) {
            GameObject gameObject = gameObjects[i];
            if (gameObject.name == name) {
                return gameObject;
            }
        }
        return null;
    }

    /// <summary>
    /// 去除是其它项的子级的项
    /// </summary>
    private static void RemoveChildOfGameObjects(ref GameObject[] gameObjects) {
        List<GameObject> tempList = new List<GameObject>(gameObjects);
        for (int i = tempList.Count - 1; i >= 0; i--) {
            GameObject a = tempList[i];
            bool isRemove = false;
            for (int j = 0; j < tempList.Count; j++) {
                GameObject b = tempList[j];
                if (a == b) continue;
                if (a.transform.IsChildOf(b.transform)) {
                    isRemove = true;
                    break;
                }
            }
            if (isRemove) {
                tempList.RemoveAt(i);
            }
        }
        gameObjects = tempList.ToArray();
    }

}
