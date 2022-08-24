#if UNITY_EDITOR
using System.IO;
using System.Reflection;
using UnityEditor;
using UnityEngine;

public class TextureToNearestPowerOf2 : ScriptableObject {


    [MenuItem("Tools/Texture to nearest power of 2")]
    private static void ToNearestPowerOf2() {
        const string dot = ".";
        const string dotPNG = ".png";
        const string dotMeta = ".meta";

        Texture2D[] texture2Ds = Selection.GetFiltered<Texture2D>(SelectionMode.TopLevel);

        RenderTexture activeRecord = RenderTexture.active;

        for (int i = 0, len = texture2Ds.Length; i < len; i++) {
            Texture2D texture2D = texture2Ds[i];

            string path = AssetDatabase.GetAssetPath(texture2D);
            TextureImporter importer = (TextureImporter)AssetImporter.GetAtPath(path);

            // 只操作 TextureType：Sprite(2D and UI)，SpriteMode: Single/Multiple 的纹理
            bool is2dAndUI = importer.textureType == TextureImporterType.Sprite;
            bool isSingleOrMultipleSpriteMode = importer.spriteImportMode == SpriteImportMode.Single || importer.spriteImportMode == SpriteImportMode.Multiple;
            if (!is2dAndUI || !isSingleOrMultipleSpriteMode) {
                Debug.Log("只能转换 TextureType: Sprite(2D and UI)，SpriteMode: Single/Multiple 的纹理");
                continue;
            }

            if (Mathf.IsPowerOfTwo(texture2D.width) && Mathf.IsPowerOfTwo(texture2D.height)) {
                Debug.Log("已是2的次方，跳过");
                continue;
            }

            // 下一个二次方值
            int outputWidth = Mathf.NextPowerOfTwo(texture2D.width);
            int outputHeight = Mathf.NextPowerOfTwo(texture2D.height);

            Texture2D output = new Texture2D(outputWidth, outputHeight, TextureFormat.ARGB32, false);

            // 透明
            Color32[] color32s = new Color32[outputWidth * outputHeight];
            output.SetPixels32(color32s, 0);

            // 复制到 RenderTexture
            RenderTexture renderTexture = new RenderTexture(texture2D.width, texture2D.height, 16, RenderTextureFormat.ARGB32);
            Graphics.Blit(texture2D, renderTexture);
            RenderTexture.active = renderTexture;

            // 从 RenderTexture 中读出来
            output.ReadPixels(new Rect(0, 0, texture2D.width, texture2D.height), 0, 0);
            output.Apply();

            // 删除原文件，写入新文件
            File.Delete(path);
            string outputPath = path.Substring(0, path.LastIndexOf(dot)) + dotPNG; // 后缀改为.png
            //outputPath = path;
            //outputPath = path.Insert(path.LastIndexOf("."), "_");
            Debug.Log(outputPath);
            byte[] bytes = output.EncodeToPNG();
            File.WriteAllBytes(outputPath, bytes);

            File.Move(path + dotMeta, outputPath + dotMeta);// 修改.meta名为png.meta

            // 修改导入设置
            if (importer.spriteImportMode == SpriteImportMode.Single) {
                importer.spriteImportMode = SpriteImportMode.Multiple;
                importer.spritesheet = new SpriteMetaData[] {
                            new SpriteMetaData{
                                name=texture2D.name,
                                rect=new Rect(0,0,texture2D.width,texture2D.height),
                                alignment=0,
                                pivot=new Vector2(0.5f,0.5f),
                                border=importer.spriteBorder
                            }
                        };
                importer.spritesheet[0].rect = new Rect(0, 0, texture2D.width, texture2D.height);
            }
            importer.spriteBorder = Vector4.zero;
        }
        RenderTexture.active = activeRecord;

        AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport);
    }

    private static int GetLocalIdentfierInFile(UnityEngine.Object obj) {
        PropertyInfo info = typeof(SerializedObject).GetProperty("inspectorMode", BindingFlags.NonPublic | BindingFlags.Instance);
        SerializedObject sObj = new SerializedObject(obj);
        info.SetValue(sObj, InspectorMode.Debug, null);
        SerializedProperty localIdProp = sObj.FindProperty("m_LocalIdentfierInFile");
        return localIdProp.intValue;
    }
}
#endif
