#if UNITY_EDITOR
using System.IO;
using UnityEditor;
using UnityEditor.Build;
using UnityEngine;
using UnityEngine.Experimental.Rendering;

/// <summary>
/// 将图标转换为多种尺寸
/// 使用方法：右键单击.png -> Convert Icon To Multiple Sizes，将在 .png 当前目录下生成 Icons 文件夹及各尺寸的图标
/// </summary>
public class ConvertIconToMultipleSizes : Editor {

    private static readonly int[] s_sizes = { 432, 324, 216, 162, 108, 81, 192, 144, 96, 72, 48, 36 };

    [MenuItem("Assets/Convert Icon To Multiple Sizes", true)]
    private static bool ValidateSplitFbxAnimation() {
        if (Selection.count == 1) {
            if (Selection.activeObject is Texture2D) {
                return true;
            }
        }
        return false;
    }

    [MenuItem("Assets/Convert Icon To Multiple Sizes")]
    private static void SplitFbxAnimation() {
        if (Selection.count == 1) {
            if (Selection.activeObject is Texture2D) {
                Texture2D source = Selection.activeObject as Texture2D;
                string sourcePath = AssetDatabase.GUIDToAssetPath(Selection.assetGUIDs[0]);

                string sourceFolderPath = Path.GetDirectoryName(sourcePath).Replace("\\", "/");
                string newFolderPath = sourceFolderPath + "/Icons";

                if (!File.Exists(newFolderPath)) {
                    Directory.CreateDirectory(newFolderPath);
                }

                foreach (int size in s_sizes) {
                    Texture2D result = ScaleTexture(source, size, size);
                    byte[] bytes = result.EncodeToPNG();
                    
                    string outputPath = $"{newFolderPath}/icon{size}x{size}.png";
                    File.WriteAllBytes(outputPath, bytes);
                }
                AssetDatabase.Refresh();
            }
        }
    }


    /// <summary>
    /// 缩放纹理
    /// <para>
    /// 注意：
    /// source 源纹理的 Read/Write 开关需要在 Inspector 中打开,
    /// 不能勾选 Use Crunch Compression
    /// </para>
    /// </summary>
    public static Texture2D ScaleTexture(Texture2D source, int targetWidth, int targetHeight) {
        Texture2D result = new(targetWidth, targetHeight);
        Color[] rpixels = new Color[targetWidth * targetHeight];
        float incX = ((float)1 / source.width) * ((float)source.width / targetWidth);
        float incY = ((float)1 / source.height) * ((float)source.height / targetHeight);
        for (int px = 0; px < rpixels.Length; px++) {
            rpixels[px] = source.GetPixelBilinear(incX * ((float)px % targetWidth), incY * (Mathf.Floor((float)px / targetWidth)));
        }
        result.SetPixels(rpixels, 0);
        result.Apply();
        return result;
    }
}
#endif