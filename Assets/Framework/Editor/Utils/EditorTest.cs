#if UNITY_EDITOR

using System;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

public class EditorTest : Editor {

    [MenuItem("Tools/EditorTest", true)]
    private static bool ValidateMenuItem() {
        return !EditorApplication.isPlaying;
    }

    [MenuItem("Tools/EditorTest")]
    private static void Test() {
        if (EditorApplication.isPlaying) return;
        Debug.Log("== Tools/EditorTest ==");

        // 检查关卡序列是否有缺失
        //EditorLevelSequence.CheckLevelSequenceRight("Assets/Scenes", "Level_", ".unity",136);

        // 查找缺失的关卡，如果有缺失关则缺失关后的所有关向前移
        //EditorLevelSequence.MergeGap("Assets/Scenes","Level_",".unity");

        // 交换关卡
        //EditorLevelSequence.SwapLevelFile("Assets/Scenes", "Level_", ".unity", 19,27,  16,25);

        // 将所有 “Level_28-1”、“Level_28-2”... 命名的场景文件插入到关卡序列
        //EditorLevelSequence.InsertAllLevelFile("Assets/Scenes", "Level_", ".unity");

        // 将文件名指定的文件如：“Level_temp” 插入到指定的目标关卡，目标关卡及之后的关卡将后移
        //EditorLevelSequence.RenameAndInsertIntoTarget("Assets/Scenes", "Level_", ".unity", "Level_20-4", 21);

        // 在已排序好的关卡序列中，将某一关调整到指定的目标关，目标关及之后的关卡会后移，调整后会产生间隙
        //EditorLevelSequence.InsertIntoTarget("Assets/Scenes", "Level_", ".unity",136,135);

        // 重命名一个文件
        //EditorLevelSequence.RenameFile("Assets/Scenes",".unity", "Level_0", "Level_-");

        
    }
}

#endif