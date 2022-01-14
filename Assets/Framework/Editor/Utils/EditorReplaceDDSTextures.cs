#if UNITY_EDITOR
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

/// <summary>
/// 将材质引用的 .dds 纹理替换为.jpg或.png格式
/// * .jpg或.png需与.dds同名，同位置
/// * .fbx文件引用的.dds需将材质解压
/// </summary>
public class EditorReplaceDDSTextures : Editor {

    private static readonly string[] s_propertyNames = { "_MainTex", "_BumpMap" };
    private static readonly List<string> s_ddsTexturePaths = new List<string>();

    [MenuItem("Assets/Replace DDS Textures")]
    public static void ReplaceDDSTextures () {
        string[] paths = AssetDatabase.GetAllAssetPaths();
        for (int i = 0; i < paths.Length; i++) {
            string path = paths[i];
            if (string.IsNullOrEmpty(path)) continue;
            if (path.IndexOf("Assets") == 0) {
                string extension = Path.GetExtension(path).ToLower();

                if (extension == ".fbx") {
                    if (GetFBXReferenceDDS(path)) {
                        Debug.LogWarning($"{path} 引用了.dds纹理，需要将材质解压后才能替换");
                    }
                } else {
                    Material material = AssetDatabase.LoadAssetAtPath<Material>(path);
                    if (material) {
                        ReplaceMaterialDDS(material);
                    }
                }
            }
        }

        //保存并刷新资源
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        Debug.Log("Replace DDS textures complete");
    }

    [MenuItem("Assets/Delete All Replaced DDS")]
    public static void DeleteAllReplacedDDS () {
        for (int i = 0; i < s_ddsTexturePaths.Count; i++) {
            AssetDatabase.DeleteAsset(s_ddsTexturePaths[i]);
        }
        s_ddsTexturePaths.Clear();
        //保存并刷新资源
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        Debug.Log("Delete all replaced DDS textures complete");
    }

    private static bool GetFBXReferenceDDS (string fbxPath) {
        Object[] objects = AssetDatabase.LoadAllAssetRepresentationsAtPath(fbxPath);
        for (int j = 0; j < objects.Length; j++) {
            Object obj = objects[j];
            if (obj && obj is Material) {
                if (GetMaterialReferenceDDS((Material)obj)) {
                    return true;
                }
            }
        }
        return false;
    }

    private static bool GetMaterialReferenceDDS (Material material) {
        for (int i = 0; i < s_propertyNames.Length; i++) {
            string propertyName = s_propertyNames[i];
            if (material.HasProperty(propertyName)) {
                Texture texture = material.GetTexture(propertyName);
                if (texture) {
                    string texturePath = AssetDatabase.GetAssetPath(texture);
                    string texturePathExtension = Path.GetExtension(texturePath).ToLower();
                    if (texturePathExtension == ".dds") {
                        return true;
                    }
                }
            }
        }
        return false;
    }

    private static void ReplaceMaterialDDS (Material material) {
        for (int i = 0; i < s_propertyNames.Length; i++) {
            string propertyName = s_propertyNames[i];
            if (material.HasProperty(propertyName)) {
                Texture texture = material.GetTexture(propertyName);
                if (texture) {
                    string texturePath = AssetDatabase.GetAssetPath(texture);
                    string texturePathExtension = Path.GetExtension(texturePath).ToLower();
                    if (texturePathExtension == ".dds") {
                        string newTextruePath = texturePath.Substring(0, texturePath.Length - texturePathExtension.Length) + ".png";
                        Texture newTexture = AssetDatabase.LoadAssetAtPath<Texture>(newTextruePath);
                        if (!newTexture) {
                            newTextruePath = texturePath.Substring(0, texturePath.Length - texturePathExtension.Length) + ".jpg";
                            newTexture = AssetDatabase.LoadAssetAtPath<Texture>(newTextruePath);
                        }
                        if (newTexture) {
                            material.SetTexture(propertyName, newTexture);
                            // 成功替换的 .dds 添加到列表
                            if (!s_ddsTexturePaths.Contains(texturePath)) {
                                s_ddsTexturePaths.Add(texturePath);
                            }
                        } else {
                            Debug.Log($"替换 {texturePath} 失败，没有找到 {newTextruePath}.png或.jpg");
                        }
                    }
                }
            }
        }
    }


}
#endif