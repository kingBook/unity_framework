using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

public class EditorTest : Editor {
    [MenuItem("Tools/EditorTest")]
    public static void Test () {
        if (EditorApplication.isPlaying) return;
        Debug.Log("Test");
		
		// 检查关卡序列是否有缺失
        //EditorLevelSequence.CheckLevelSequenceRight("Assets/Scenes", "Level_", ".unity",136);

        // 交换关卡
        //EditorLevelSequence.SwapLevelFile("Assets/Scenes", "Level_", ".unity", 19,27,  16,25);

        // 将所有 “Level_28-1”、“Level_28-2”... 命名的场景文件插入到关卡序列
        //EditorLevelSequence.InsertAllLevelFile("Assets/Scenes", "Level_", ".unity");

        // 将文件名指定的文件如：“Level_20-4” 插入到指定的关卡数
        //EditorLevelSequence.RenameFileIntoSequence("Assets/Scenes", "Level_", ".unity", "Level_20-4", 21);
    }

}