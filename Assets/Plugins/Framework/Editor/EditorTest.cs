using System;
using UnityEditor;
using UnityEngine;

public class EditorTest : Editor {

    [MenuItem("Tools/EditorTest", true)]
    private static bool ValidateMenuItem () {
        return !EditorApplication.isPlaying;
    }

    [MenuItem("Tools/EditorTest")]
    private static void Test () {
        if (EditorApplication.isPlaying) return;
        Debug.Log("== Tools/EditorTest ==");

        // 检查关卡序列是否有缺失
        //EditorLevelSequence.CheckLevelSequenceRight("Assets/Scenes", "Level_", ".unity",136);

        // 查找缺失的关卡数，如果缺失则缺失之后的关卡前移
        //EditorLevelSequence.MergeGap("Assets/Scenes","Level_",".unity");

        // 交换关卡
        //EditorLevelSequence.SwapLevelFile("Assets/Scenes", "Level_", ".unity", 19,27,  16,25);

        // 将所有 “Level_28-1”、“Level_28-2”... 命名的场景文件插入到关卡序列
        //EditorLevelSequence.InsertAllLevelFile("Assets/Scenes", "Level_", ".unity");

        // 将文件名指定的文件如：“Level_20-4” 插入到指定的关卡数
        //EditorLevelSequence.RenameFileIntoSequence("Assets/Scenes", "Level_", ".unity", "Level_20-4", 21);
    }



}