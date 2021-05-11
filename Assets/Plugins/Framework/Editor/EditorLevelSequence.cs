using System;
using UnityEditor;
using UnityEngine;
using System.Collections;
using System.IO;


public static class EditorLevelSequence {

    /// <summary> 查找的最大关卡数 </summary>
    public const int MaxLevelCount = 500;

    /// <summary>
    /// 检查关卡序列中是否有某一关缺失，并在控制台输出结果
    /// </summary>
    /// <param name="levelFilesDirectory"> 包含所有关卡文件的目录，如：“Assets/Scenes” </param>
    /// <param name="levelFileNamesPrefix"> 关卡文件名称前缀，如：“Level_”。（一般都有 "Level_1"、"Level_2"... 一样的前缀加数字的命名） </param>
    /// <param name="levelFileNamesExtension"> 关卡名称后缀，如：“.jpg”、“.unity” </param>
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
    /// 交换如："Level_1"、"Level_2"... ’前缀+数字‘ 的文件
    /// </summary>
    /// <param name="levelFilesDirectory"> 包含所有关卡文件的目录，如：“Assets/Scenes” </param>
    /// <param name="levelFileNamesPrefix"> 关卡文件名称前缀，如：“Level_”。（一般都有 "Level_1"、"Level_2"... 一样的前缀加数字的命名） </param>
    /// <param name="levelFileNamesExtension"> 关卡名称后缀，如：“.jpg”、“.unity” </param>
    /// <param name="levelNumbers"> 多组需要交换的关卡数字，如：“1,2, 3,4, 5,6” </param>
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
    /// 交换如："Level_1"、"Level_2"... ’前缀+数字‘ 的文件
    /// </summary>
    /// <param name="levelFilesDirectory"> 包含所有关卡文件的目录，如：“Assets/Scenes” </param>
    /// <param name="levelFileNamesPrefix"> 关卡文件名称前缀，如：“Level_”。（一般都有 "Level_1"、"Level_2"... 一样的前缀加数字的命名） </param>
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
    /// 将所有如："Level_1"、"Level_2"... ’前缀+数字‘ 的文件插入到关卡文件序列，示例代码：InsertAllLevelFile("Assets/Scenes", "Level_", ".unity");
    /// </summary>
    /// <param name="levelFilesDirectory"> 包含所有关卡文件的目录，如：“Assets/Scenes” </param>
    /// <param name="levelFileNamesPrefix"> 关卡文件名称前缀，如：“Level_”。（一般都有 "Level_1"、"Level_2"... 一样的前缀加数字的命名） </param>
    /// <param name="levelFileNamesExtension"> 关卡名称后缀，如：“.jpg”、“.unity” </param>
    public static void InsertAllLevelFile (string levelFilesDirectory, string levelFileNamesPrefix, string levelFileNamesExtension) {
        for (int i = MaxLevelCount; i >= 1; i--) {
            for (int j = MaxLevelCount; j >= 1; j--) {
                string fileName = $"{levelFileNamesPrefix}{i}-{j}";
                RenameFileIntoSequence(levelFilesDirectory, levelFileNamesPrefix, levelFileNamesExtension, fileName, i + 1);
            }
        }
    }

    /// <summary>
    /// 重命名一个文件且插入到原有的关卡序列文件中，插入的文件和其它关卡序列文件目录必须一致（注意：关卡数量超过500需要调整最大关卡数（MaxLevelCount）常量），示例代码：RenameFileIntoSequence("Assets/Scenes", "Level_", ".unity", "Level_20-4", 21);
    /// </summary>
    /// <param name="levelFilesDirectory"> 包含所有关卡文件的目录，如：“Assets/Scenes” </param>
    /// <param name="levelFileNamesPrefix"> 关卡文件名称前缀，如：“Level_”。（一般都有 "Level_1"、"Level_2"... 一样的前缀加数字的命名） </param>
    /// <param name="levelFileNamesExtension"> 关卡名称后缀，如：“.jpg”、“.unity” </param>
    /// <param name="fileName"> 即将要插入到关卡序列场景名称不包含后缀，如：“Level_28-2” </param>
    /// <param name="insertLevelNumber"> 插入到的关卡数字（原有的关卡将后移） </param>
    public static void RenameFileIntoSequence (string levelFilesDirectory, string levelFileNamesPrefix, string levelFileNamesExtension, string fileName, int insertLevelNumber) {
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
        Debug.Log($"已将 {fileName}{levelFileNamesExtension} 重命名为：{levelFileNamesPrefix}{insertLevelNumber}{levelFileNamesExtension}");
    }
}
