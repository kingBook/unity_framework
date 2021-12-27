#if UNITY_EDITOR

using System;
using UnityEditor;
using UnityEngine;
using System.Collections;
using System.IO;
using System.Collections.Generic;

public static class EditorLevelSequence {

    /// <summary> 查找的最大关卡数 </summary>
    public const int MaxLevelCount = 500;

    /// <summary>
    /// 检查关卡序列中是否有某一关缺失，并在控制台输出结果
    /// </summary>
    /// <param name="levelFilesDirectory"> 包含所有关卡文件的目录，如：“Assets/Scenes” </param>
    /// <param name="levelFileNamesPrefix"> 关卡文件名称前缀，如：“Level_” </param>
    /// <param name="levelFileNamesExtension"> 关卡名称后缀，如：“.jpg”、“.unity” </param>
    /// <param name="maxLevelNumber"> 最大的关卡数 </param>
    public static void CheckLevelSequenceRight (string levelFilesDirectory, string levelFileNamesPrefix, string levelFileNamesExtension, int maxLevelNumber) {
        // 如果目录尾部有 "/" 则删除
        if (levelFilesDirectory[levelFilesDirectory.Length - 1] == '/') {
            levelFilesDirectory = levelFilesDirectory.Remove(levelFilesDirectory.Length - 1, 1);
        }

        for (int i = 1; i <= maxLevelNumber; i++) {
            string path = $"{levelFilesDirectory}/{levelFileNamesPrefix}{i}{levelFileNamesExtension}";
            if (!File.Exists(path)) {
                Debug.Log("缺失：" + path);
            } else {
                //查找到最后一个都没缺失，则输出‘无缺失’消息
                if (i >= maxLevelNumber) {
                    Debug.Log("检查完成：没有关卡缺失");
                }
            }
        }
    }

    /// <summary>
    /// 查找 [1, <see cref="MaxLevelCount"/>] 之间，如果有缺失关则缺失关后的所有关向前移
    /// </summary>
    /// <param name="levelFilesDirectory"> 包含所有关卡文件的目录，如：“Assets/Scenes” </param>
    /// <param name="levelFileNamesPrefix"> 关卡文件名称前缀，如：“Level_” </param>
    /// <param name="levelFileNamesExtension"> 关卡名称后缀，如：“.jpg”、“.unity” </param>
    public static void MergeGap (string levelFilesDirectory, string levelFileNamesPrefix, string levelFileNamesExtension) {
        // 如果目录尾部有 "/" 则删除
        if (levelFilesDirectory[levelFilesDirectory.Length - 1] == '/') {
            levelFilesDirectory = levelFilesDirectory.Remove(levelFilesDirectory.Length - 1, 1);
        }


        // 查找缺失的所有关卡
        List<int> missLevelNumbers = new List<int>(); // 缺失的关卡列表

        for (int i = 1; i <= MaxLevelCount; i++) {
            string path = $"{levelFilesDirectory}/{levelFileNamesPrefix}{i}{levelFileNamesExtension}";
            if (!File.Exists(path)) {
                missLevelNumbers.Add(i); // 添加缺失的关卡数
            }
        }

        if (missLevelNumbers.Count > 0) {
            while (true) {
                int levelNumber = missLevelNumbers[missLevelNumbers.Count - 1] + 1; // 缺失关的下一关
                for (int i = levelNumber; i <= MaxLevelCount; i++) {
                    string fileName = $"{levelFileNamesPrefix}{i}"; // 无后缀
                    int prevLevelNumber = i - 1;
                    RenameAndInsertIntoTarget(levelFilesDirectory, levelFileNamesPrefix, levelFileNamesExtension, fileName, prevLevelNumber);
                }

                missLevelNumbers.RemoveAt(missLevelNumbers.Count - 1);
                if (missLevelNumbers.Count <= 0) {
                    break;
                }
            }
        }
    }

    /// <summary>
    /// 交换关卡文件（一次对调多个关卡）
    /// </summary>
    /// <param name="levelFilesDirectory"> 包含所有关卡文件的目录，如：“Assets/Scenes” </param>
    /// <param name="levelFileNamesPrefix"> 关卡文件名称前缀，如：“Level_” </param>
    /// <param name="levelFileNamesExtension"> 关卡名称后缀，如：“.jpg”、“.unity” </param>
    /// <param name="levelNumbers"> 多组需要交换的关卡数字，每两个为一组，如：“1,2, 3,4, 5,6” </param>
    public static void SwapLevelFile (string levelFilesDirectory, string levelFileNamesPrefix, string levelFileNamesExtension, params int[] levelNumbers) {
        if (levelNumbers.Length < 2 || levelNumbers.Length % 2 > 0) {
            Debug.LogError("参数 levelNumbers 长度必须大于0，且是2的倍数");
            return;
        }

        for (int i = 0, len = levelNumbers.Length; i < len; i += 2) {
            SwapLevelFile(levelFilesDirectory, levelFileNamesPrefix, levelFileNamesExtension, levelNumbers[i], levelNumbers[i + 1]);
        }
    }

    /// <summary>
    /// 交换关卡文件（一次只能对调两个关卡）
    /// </summary>
    /// <param name="levelFilesDirectory"> 包含所有关卡文件的目录，如：“Assets/Scenes” </param>
    /// <param name="levelFileNamesPrefix"> 关卡文件名称前缀，如：“Level_” </param>
    /// <param name="levelFileNamesExtension"> 关卡名称后缀，如：“.jpg”、“.unity” </param>
    /// <param name="levelNumberA"> 关卡数字 A </param>
    /// <param name="levelNumberB"> 关卡数字 B </param>
    public static void SwapLevelFile (string levelFilesDirectory, string levelFileNamesPrefix, string levelFileNamesExtension, int levelNumberA, int levelNumberB) {
        // 如果目录尾部有 "/" 则删除
        if (levelFilesDirectory[levelFilesDirectory.Length - 1] == '/') {
            levelFilesDirectory = levelFilesDirectory.Remove(levelFilesDirectory.Length - 1, 1);
        }

        string filePathA = $"{levelFilesDirectory}/{levelFileNamesPrefix}{levelNumberA}{levelFileNamesExtension}";
        string filePathB = $"{levelFilesDirectory}/{levelFileNamesPrefix}{levelNumberB}{levelFileNamesExtension}";

        if (File.Exists(filePathA) && File.Exists(filePathB)) {
            string nameA = $"{levelFileNamesPrefix}{levelNumberA}{levelFileNamesExtension}";
            string nameB = $"{levelFileNamesPrefix}{levelNumberB}{levelFileNamesExtension}";

            // 将 A 临时换一个其它名称
            string tempNameA = $"temp_{nameA}";
            string tempFilePathA = $"{levelFilesDirectory}/{tempNameA}";
            AssetDatabase.RenameAsset(filePathA, tempNameA);

            // 把 B 命名为 A 原本的名称
            AssetDatabase.RenameAsset(filePathB, nameA);

            // 把临时的 A 命名为 B 原本的名称
            AssetDatabase.RenameAsset(tempFilePathA, nameB);
        }
    }

    /// <summary>
    /// 将所有如：“Level_28-1”、“Level_28-2”... ’{前缀}{数字}-{数字}‘ 的文件插入到关卡文件序列，示例代码：InsertAllLevelFile("Assets/Scenes", "Level_", ".unity");
    /// </summary>
    /// <param name="levelFilesDirectory"> 包含所有关卡文件的目录，如：“Assets/Scenes” </param>
    /// <param name="levelFileNamesPrefix"> 关卡文件名称前缀，如：“Level_” </param>
    /// <param name="levelFileNamesExtension"> 关卡名称后缀，如：“.jpg”、“.unity” </param>
    public static void InsertAllLevelFile (string levelFilesDirectory, string levelFileNamesPrefix, string levelFileNamesExtension) {
        for (int i = MaxLevelCount; i >= 1; i--) {
            for (int j = MaxLevelCount; j >= 1; j--) {
                string fileName = $"{levelFileNamesPrefix}{i}-{j}";
                RenameAndInsertIntoTarget(levelFilesDirectory, levelFileNamesPrefix, levelFileNamesExtension, fileName, i + 1);
            }
        }
    }

    /// <summary>
    /// 重命名一个文件并插入到指定的目标关卡，目标关卡及之后的关卡将会后移，插入的文件和其它关卡序列文件目录必须一致（注意：关卡数量超过500需要调整最大关卡数（<see cref="MaxLevelCount"/>）常量），示例代码：RenameFileIntoSequence("Assets/Scenes", "Level_", ".unity", "Level_20-4", 21);
    /// </summary>
    /// <param name="levelFilesDirectory"> 包含所有关卡文件的目录，如：“Assets/Scenes” </param>
    /// <param name="levelFileNamesPrefix"> 关卡文件名称前缀，如：“Level_” </param>
    /// <param name="levelFileNamesExtension"> 关卡名称后缀，如：“.jpg”、“.unity” </param>
    /// <param name="fileName"> 即将要插入到关卡序列场景名称不包含后缀，如：“Level_temp” </param>
    /// <param name="insertLevelNumber"> 插入到的关卡数字（原有的关卡将后移） </param>
    public static void RenameAndInsertIntoTarget (string levelFilesDirectory, string levelFileNamesPrefix, string levelFileNamesExtension, string fileName, int insertLevelNumber) {
        //检查要重命名的文件是否存在
        string filePath = $"{levelFilesDirectory}/{fileName}{levelFileNamesExtension}";
        if (!File.Exists(filePath)) {
            //Debug.LogError(filePath + " 不存在");
            return;
        }

        // 如果目录尾部有 "/" 则删除
        if (levelFilesDirectory[levelFilesDirectory.Length - 1] == '/') {
            levelFilesDirectory = levelFilesDirectory.Remove(levelFilesDirectory.Length - 1, 1);
        }

        string targetPath = $"{levelFilesDirectory}/{levelFileNamesPrefix}{insertLevelNumber}{levelFileNamesExtension}";

        // 如果目标路径存在，插入数字及之后都后移
        if (File.Exists(targetPath)) {
            for (int i = MaxLevelCount; i >= insertLevelNumber; i--) {
                string tempPath = $"{levelFilesDirectory}/{levelFileNamesPrefix}{i}{levelFileNamesExtension}";
                if (File.Exists(tempPath)) {
                    string newName = $"{levelFileNamesPrefix}{i + 1}{levelFileNamesExtension}";
                    AssetDatabase.RenameAsset(tempPath, newName);
                }
            }
        }

        // 重命名
        string oldFilePath = $"{levelFilesDirectory}/{fileName}{levelFileNamesExtension}";
        string newFileName = $"{levelFileNamesPrefix}{insertLevelNumber}{levelFileNamesExtension}";

        AssetDatabase.RenameAsset(oldFilePath, newFileName);
        //Debug.Log($"已将 {fileName}{levelFileNamesExtension} 重命名为：{levelFileNamesPrefix}{insertLevelNumber}{levelFileNamesExtension}");
    }

    /// <summary>
    /// 将某一关插入到目标关，目标关卡及之后的关卡会后移（注意：1.调整后可能会产生间隙。2.关卡数量超过500需要调整最大关卡数（<see cref="MaxLevelCount"/>）常量。）
    /// </summary>
    /// <param name="levelFilesDirectory"> 包含所有关卡文件的目录，如：“Assets/Scenes” </param>
    /// <param name="levelFileNamesPrefix"> 关卡文件名称前缀，如：“Level_” </param>
    /// <param name="levelFileNamesExtension"> 关卡名称后缀，如：“.jpg”、“.unity” </param>
    /// <param name="currentLevelNumber"> 当前关卡 </param>
    /// <param name="targetLevelNumber"> 目标关卡 </param>
    public static void InsertIntoTarget (string levelFilesDirectory, string levelFileNamesPrefix, string levelFileNamesExtension, int currentLevelNumber, int targetLevelNumber) {
        if (currentLevelNumber == targetLevelNumber) {
            return;
        }

        // 先当前文件换一个临时的名称
        string currentFileName = $"{levelFileNamesPrefix}{currentLevelNumber}";
        string tempFileName = $"{currentFileName}_temp";
        RenameFile(levelFilesDirectory, levelFileNamesExtension, currentFileName, tempFileName);

        // 将临时命名的文件插入到指定的目标关卡，目标关卡及之后的关卡将后移
        RenameAndInsertIntoTarget(levelFilesDirectory, levelFileNamesPrefix, levelFileNamesExtension, tempFileName, targetLevelNumber);
    }

    /// <summary>
    /// 重命名一个文件
    /// </summary>
    /// <param name="directory"> 包含文件的目录，如：“Assets/Scenes” </param>
    /// <param name="fileExtension"> 文件的后缀，如：“.unity” </param>
    /// <param name="fileName"> 原来的文件名（不包含后缀），如：“Level_1”</param>
    /// <param name="newFileName"> 新的文件名（不包含后缀），如：“Level_2” </param>
    public static void RenameFile (string directory, string fileExtension, string fileName, string newFileName) {
        // 如果目录尾部有 "/" 则删除
        if (directory[directory.Length - 1] == '/') {
            directory = directory.Remove(directory.Length - 1, 1);
        }

        string newFilePath = $"{directory}/{newFileName}{fileExtension}";
        if (File.Exists(newFilePath)) {
            Debug.LogError(newFilePath + " 已存在，请指定新的名称");
            return;
        }
        string oldFilePath = $"{directory}/{fileName}{fileExtension}";
        string newName = $"{newFileName}{fileExtension}";

        AssetDatabase.RenameAsset(oldFilePath, newName);
    }
}

#endif