using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using System.Xml;
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 切片flash导入出来的位图表
/// </summary>
public class SpriteSheetPostprocessor : AssetPostprocessor {
    /// <summary>当前项目的路径</summary>
    private static readonly string s_projectPath = Environment.CurrentDirectory.Replace("\\", "/");
    /// <summary>是否创建AnimatorContrller文件</summary>
    private static bool s_isCreateAnimatorController = false;
    /// <summary>单帧是否创建动画</summary>
    private const bool IsSingleFrameCreateAnim = true;
    /// <summary>帧频</summary>
    private const int FrameRate = 24;
    /// <summary>动画类型</summary>
    private enum AnimationType { Sprite, Image }

    private void OnPreprocessTexture () {
        //去除后缀的资源相对路径，如：Assets/Textures/idle
        string assetNamePath = assetPath.Substring(0, assetPath.LastIndexOf('.'));
        //资源文件无后缀的名称
        string assetName = assetNamePath.Substring(assetNamePath.LastIndexOf('/') + 1);
        //.xml绝对路径，如：D:/projects/unity_test/Assets/Textures/idle.xml
        string xmlPath = s_projectPath + '/' + assetNamePath + ".xml";
        if (File.Exists(xmlPath)) {
            TextureImporter importer = (TextureImporter)assetImporter;
            //通过反射调用TextureImporter.GetWidthAndHeight私有方法获取纹理的高
            object[] parameters = new object[2] { 0, 0 };
            MethodInfo methodInfo = importer.GetType().GetMethod("GetWidthAndHeight", BindingFlags.NonPublic | BindingFlags.Instance);
            methodInfo.Invoke(importer, parameters);
            int textureHeight = (int)parameters[1];
            //切图
            SpriteMetaData[] spritesheet = GetSpritesheetData(textureHeight, assetName, xmlPath);
            //设置导入器属性
            importer.textureType = TextureImporterType.Sprite;
            importer.spriteImportMode = spritesheet.Length > 0 ? SpriteImportMode.Multiple : SpriteImportMode.Single;
            importer.spritesheet = spritesheet;
        }
    }

    private void OnPostprocessTexture (Texture2D texture) {
        //在此方法内创建动画，删除.xml会出现不能读取问题
    }

    private static void OnPostprocessAllAssets (string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths) {
        int len = importedAssets.Length;
        int i;
        //排序.multipleImageAnim路径放在前
        List<string> pathsList = new List<string>();
        for (i = 0; i < len; i++) {
            string path = importedAssets[i];
            string extension = Path.GetExtension(path);
            if (extension == ".multipleImageAnim") pathsList.Insert(0, path);
            else pathsList.Add(path);
        }
        importedAssets = pathsList.ToArray();
        i = len;
        while (--i >= 0) {
            string path = importedAssets[i];
            int dotIndex = path.LastIndexOf('.');
            if (dotIndex > -1) {
                string extension = Path.GetExtension(path);
                if (extension == ".png") {
                    //去除后缀的资源相对路径，如：Assets/Textures/idle
                    string assetNamePath = path.Substring(0, path.LastIndexOf('.'));
                    //.xml绝对路径，如：D:/projects/unity_test/Assets/Textures/idle.xml
                    string xmlPath = s_projectPath + '/' + assetNamePath + ".xml";
                    if (File.Exists(xmlPath)) {
                        //如：Assets/Textures/idle0000 去掉尾部的 0000
                        string assetNamePathRemovedFrameNO = assetNamePath.Substring(0, assetNamePath.Length - 4);
                        //如：D:/projects/unity_test/Assets/Textures/idle.multipleImageAnim
                        string testMultipleImageAnimPath = s_projectPath + '/' + assetNamePathRemovedFrameNO + ".multipleImageAnim";
                        //不存在.multipleImageAnim多图片动画配置文件才创建动画
                        if (!File.Exists(testMultipleImageAnimPath)) {
                            //延时创建动画文件
                            DelayCreateAnimationFile(1, path, assetNamePath);
                        }
                        //删除.xml文件
                        DelayDeleteAsset(1, assetNamePath + ".xml");
                    }
                } else if (extension == ".multipleImageAnim") {
                    //资源所在文件夹的相对路径，如：Assets/Textures
                    string assetFolderPath = path.Substring(0, path.LastIndexOf('/'));
                    //去除后缀的资源相对路径，如：Assets/Textures/idle
                    string assetNamePath = path.Substring(0, path.LastIndexOf('.'));
                    //.multipleImageAnim绝对路径，如：D:/projects/unity_test/Assets/Textures/idle.multipleImageAnim
                    string multipleImageAnimPath = s_projectPath + '/' + assetNamePath + ".multipleImageAnim";
                    if (File.Exists(multipleImageAnimPath)) {
                        //延时创建动画文件
                        DelayCreateAnimationFileWithMultipleImage(1, multipleImageAnimPath, assetFolderPath, assetNamePath);
                        //删除.multipleImageAnim文件
                        DelayDeleteAsset(1, assetNamePath + ".multipleImageAnim");
                    }
                }
            }
        }
    }

    /// <summary>
    /// 获取Sprite表数据
    /// </summary>
    /// <param name="textureHeight"></param>
    /// <param name="assetName"></param>
    /// <param name="xmlPath"></param>
    /// <returns></returns>
    private SpriteMetaData[] GetSpritesheetData (int textureHeight, string assetName, string xmlPath) {
        XmlDocument doc = new XmlDocument();
        doc.Load(xmlPath);

        XmlNodeList nodes = doc.DocumentElement.SelectNodes("SubTexture");
        int len = nodes.Count;
        SpriteMetaData[] spritesheet = new SpriteMetaData[len];

        Vector2 pivot = new Vector2();
        for (int i = 0; i < len; i++) {
            XmlElement ele = nodes[i] as XmlElement;
            if (i == 0) {
                pivot.x = float.Parse(ele.GetAttribute("pivotX"));
                pivot.y = float.Parse(ele.GetAttribute("pivotY"));
            }
            string name = ele.GetAttribute("name");
            float x = float.Parse(ele.GetAttribute("x"));
            float y = float.Parse(ele.GetAttribute("y"));
            float width = float.Parse(ele.GetAttribute("width"));
            float height = float.Parse(ele.GetAttribute("height"));

            string frameXStr = ele.GetAttribute("frameX");
            if (string.IsNullOrEmpty(frameXStr)) frameXStr = "0";
            float frameX = float.Parse(frameXStr);
            string frameYStr = ele.GetAttribute("frameY");
            if (string.IsNullOrEmpty(frameYStr)) frameYStr = "0";
            float frameY = float.Parse(frameYStr);

            /*string frameWidthStr=ele.GetAttribute("frameWidth");
			if(string.IsNullOrEmpty(frameWidthStr))frameWidthStr="0";
			float frameWidth=float.Parse(frameWidthStr);
			string frameHeightStr=ele.GetAttribute("frameHeight");
			if(string.IsNullOrEmpty(frameHeightStr))frameHeightStr="0";
			float frameHeight=float.Parse(frameHeightStr);

			if(frameWidth>0)width=frameWidth;
			if(frameHeight>0)height=frameHeight;*/

            float poX = (pivot.x + frameX) / width;
            float poY = (height - pivot.y - frameY) / height;

            var spriteMetaData = new SpriteMetaData();
            spriteMetaData.name = len > 1 ? assetName + name : assetName;
            spriteMetaData.alignment = (int)SpriteAlignment.Custom;
            spriteMetaData.pivot = new Vector2(poX, poY);
            spriteMetaData.rect = new Rect(x, -y + textureHeight - height, width, height);
            spritesheet[i] = spriteMetaData;
        }
        return spritesheet;
    }

    /// <summary>
    /// 延时删除资源文件
    /// </summary>
    /// <param name="delay">延时的毫秒数</param>
    /// <param name="path">资源文件相对于项目文件夹的路径</param>
    private static async void DelayDeleteAsset (int delay, string path) {
        await Task.Delay(delay);
        AssetDatabase.DeleteAsset(path);
    }

    /// <summary>
    /// 延时创建动画文件（多张png，每张png内一个Sprite）
    /// </summary>
    /// <param name="delay">延时的毫秒数</param>
    /// <param name="multipleImageAnimPath">如：D:/projects/unity_test/Assets/Textures/idle.multipleImageAnim</param>
    /// <param name="multipleImageFolderPath">如：Assets/Textures</param>
    /// <param name="multipleImageAnimNamePath">如：Assets/Textures/idle</param>

    private static async void DelayCreateAnimationFileWithMultipleImage (int delay, string multipleImageAnimPath, string multipleImageFolderPath, string multipleImageAnimNamePath) {
        await Task.Delay(delay);
        XmlDocument doc = new XmlDocument();
        doc.Load(multipleImageAnimPath);
        XmlNodeList nodes = doc.DocumentElement.SelectNodes("name");
        int len = nodes.Count;
        Sprite[] sprites = new Sprite[len];
        for (int i = 0; i < len; i++) {
            XmlElement ele = nodes[i] as XmlElement;
            // 如：Assets/Textures/idle0000.png
            string pngPath = multipleImageFolderPath + '/' + ele.InnerText + ".png";
            Sprite sprite = AssetDatabase.LoadAssetAtPath<Sprite>(pngPath);
            sprites[i] = sprite;
        }
        //创建动画文件
        CreateAnimationFile(AnimationType.Sprite, sprites, multipleImageAnimNamePath);
        CreateAnimationFile(AnimationType.Image, sprites, multipleImageAnimNamePath);
        //保存
        AssetDatabase.SaveAssets();
    }

    /// <summary>
    /// 延时创建动画文件（一张png内多帧Sprite）
    /// </summary>
    /// <param name="delay">延时的毫秒数</param>
    /// <param name="textureAssetPath">如：Assets/Textures/idle.png</param>
    /// <param name="textureAssetNamePath">如：Assets/Textures/idle</param>
    private static async void DelayCreateAnimationFile (int delay, string textureAssetPath, string textureAssetNamePath) {
        await Task.Delay(delay);
        //获取切分后的图片
        UnityEngine.Object[] objects = AssetDatabase.LoadAllAssetsAtPath(textureAssetPath);
        int len = objects.Length;
        //objects列表中除了一个是Texture2D,其它都是Sprite（Texture2D的索引位置有可能在0或length-1）
        List<Sprite> spriteList = new List<Sprite>();
        for (int i = 0; i < len; i++) {
            Sprite sprite = objects[i] as Sprite;
            if (sprite != null) spriteList.Add(sprite);
        }
        Sprite[] sprites = spriteList.ToArray();

        if (!IsSingleFrameCreateAnim && sprites.Length <= 1) return;//单帧不创建动画

        //创建动画文件
        CreateAnimationFile(AnimationType.Sprite, sprites, textureAssetNamePath);
        CreateAnimationFile(AnimationType.Image, sprites, textureAssetNamePath);
        //保存
        AssetDatabase.SaveAssets();
    }

    /// <summary>
    /// 创建动画文件
    /// </summary>
    /// <param name="type">动画的类型，用于标记是Sprite或Image</param>
    /// <param name="sprites">Sprite帧列表</param>
    /// <param name="textureAssetNamePath">如：Assets/Textures/idle</param>
    private static void CreateAnimationFile (AnimationType type, Sprite[] sprites, string textureAssetNamePath) {
        int len = sprites.Length;
        AnimationClip animationClip = new AnimationClip();
        //帧频
        animationClip.frameRate = FrameRate;
        //设置循环
        SerializedObject serializedClip = new SerializedObject(animationClip);
        SerializedProperty clipSettings = serializedClip.FindProperty("m_AnimationClipSettings");
        SerializedProperty loopTimeSet = clipSettings.FindPropertyRelative("m_LoopTime");
        loopTimeSet.boolValue = true;
        serializedClip.ApplyModifiedProperties();
        //动画曲线
        EditorCurveBinding curveBinding = new EditorCurveBinding();
        if (type == AnimationType.Sprite) {
            curveBinding.type = typeof(SpriteRenderer);
        } else {
            curveBinding.type = typeof(Image);
        }
        curveBinding.path = "";
        curveBinding.propertyName = "m_Sprite";
        //添加帧
        ObjectReferenceKeyframe[] keyframes = new ObjectReferenceKeyframe[len];
        const float frameTime = 1f / FrameRate;
        for (int i = 0; i < len; i++) {
            Sprite sprite = sprites[i];
            ObjectReferenceKeyframe keyframe = new ObjectReferenceKeyframe();
            keyframe.time = frameTime * i;
            keyframe.value = sprite;
            keyframes[i] = keyframe;
        }
        AnimationUtility.SetObjectReferenceCurve(animationClip, curveBinding, keyframes);
        //创建.anim文件
        string animFilePath;
        if (type == AnimationType.Sprite) {
            animFilePath = textureAssetNamePath + "_sprite.anim";
        } else {
            animFilePath = textureAssetNamePath + "_image.anim";
        }
        AssetDatabase.CreateAsset(animationClip, animFilePath);
        //创建.controller文件
        if (s_isCreateAnimatorController) {
            string controllerFilePath;
            if (type == AnimationType.Sprite) {
                controllerFilePath = textureAssetNamePath + "_sprite.controller";
            } else {
                controllerFilePath = textureAssetNamePath + "_image.controller";
            }
            AnimatorController animatorController = AnimatorController.CreateAnimatorControllerAtPath(controllerFilePath);
            AnimatorControllerLayer layer = animatorController.layers[0];
            AnimatorStateMachine stateMachine = layer.stateMachine;
            AnimatorState state = stateMachine.AddState(animationClip.name);
            state.motion = animationClip;
            stateMachine.AddEntryTransition(state);
        }
        //AssetDatabase.SaveAssets();
    }
}
