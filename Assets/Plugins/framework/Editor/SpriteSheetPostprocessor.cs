using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Xml;
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;
using UnityEngine.UI;
using Object=UnityEngine.Object;

/// <summary>
/// 切片flash导入出来的位图表
/// </summary>
public class SpriteSheetPostprocessor:AssetPostprocessor{
	/// <summary>当前项目的路径</summary>
	private static readonly string projectPath=Environment.CurrentDirectory.Replace("\\","/");
	/// <summary>动画类型</summary>
	private enum AnimationType{ Sprite, Image }
	/// <summary>帧频</summary>
	private const int frameRate=24;
	
	private void OnPostprocessTexture(Texture2D texture){
		//去除后缀的资源相对路径，如：Assets/Textures/idle
		string assetNamePath=assetPath.Substring(0,assetPath.LastIndexOf('.'));
		//.xml绝对路径，如：D:/projects/unity_test/Assets/Textures/idle.xml
		string xmlPath=projectPath+'/'+assetNamePath+".xml";
		//该文件仅用于标记是否已经创建动画文件
		string createAnimationStartFlagPath=projectPath+'/'+assetNamePath+".createAnimatrionStart";
		
		if(File.Exists(xmlPath)){
			//切图
			SpriteMetaData[] spritesheet=GetSpritesheetData(texture,xmlPath);
			TextureImporter importer=assetImporter as TextureImporter;
			importer.textureType=TextureImporterType.Sprite;
			importer.spriteImportMode=spritesheet.Length>0?SpriteImportMode.Multiple:SpriteImportMode.Single;
			importer.spritesheet=spritesheet;
			//删除.xml，一定要删除，否则下面重新导入会造成死机
			File.Delete(xmlPath);
			if(spritesheet.Length>1){
				//"创建动画文件标记"
				FileStream createAnimFlagFile=File.Create(createAnimationStartFlagPath);
				createAnimFlagFile.Close();
			}
			//重新导入，更新切片数据（注意：请将.xml文件删除再重新导入，否则会反复执行此项操作造成unity卡死）
			AssetDatabase.ImportAsset(assetPath);
		}else if(File.Exists(createAnimationStartFlagPath)){
			//如果存在"创建动画文件标记"，则创建动画文件
			File.Delete(createAnimationStartFlagPath);
			DelayCreateAnimationFile(assetPath,assetNamePath);
		}
	}
	
	/// <summary>
	/// 获取Sprite表数据
	/// </summary>
	/// <param name="texture"></param>
	/// <param name="xmlPath"></param>
	/// <returns></returns>
	private SpriteMetaData[] GetSpritesheetData(Texture2D texture,string xmlPath){
		XmlDocument doc=new XmlDocument();
		doc.Load(xmlPath);

		XmlNodeList nodes=doc.DocumentElement.SelectNodes("SubTexture");
		int len=nodes.Count;
		SpriteMetaData[] spritesheet=new SpriteMetaData[len];
		float textureHeight=texture.height;

		Vector2 pivot=new Vector2();
		for(int i=0;i<len;i++){
			XmlElement ele=nodes[i] as XmlElement;
			if(i==0){
				pivot.x=float.Parse(ele.GetAttribute("pivotX"));
				pivot.y=float.Parse(ele.GetAttribute("pivotY"));
			}
			string name=ele.GetAttribute("name");
			float x=float.Parse(ele.GetAttribute("x"));
			float y=float.Parse(ele.GetAttribute("y"));
			float width=float.Parse(ele.GetAttribute("width"));
			float height=float.Parse(ele.GetAttribute("height"));
			
			string frameXStr=ele.GetAttribute("frameX");
			if(string.IsNullOrEmpty(frameXStr))frameXStr="0";
			float frameX=float.Parse(frameXStr);
			string frameYStr=ele.GetAttribute("frameY");
			if(string.IsNullOrEmpty(frameYStr))frameYStr="0";
			float frameY=float.Parse(frameYStr);

			/*string frameWidthStr=ele.GetAttribute("frameWidth");
			if(string.IsNullOrEmpty(frameWidthStr))frameWidthStr="0";
			float frameWidth=float.Parse(frameWidthStr);
			string frameHeightStr=ele.GetAttribute("frameHeight");
			if(string.IsNullOrEmpty(frameHeightStr))frameHeightStr="0";
			float frameHeight=float.Parse(frameHeightStr);

			if(frameWidth>0)width=frameWidth;
			if(frameHeight>0)height=frameHeight;*/

			float poX=(pivot.x+frameX)/width;
			float poY=(height-pivot.y-frameY)/height;
			
			var spriteMetaData=new SpriteMetaData();
			spriteMetaData.name=name;
			spriteMetaData.alignment=(int)SpriteAlignment.Custom;
			spriteMetaData.pivot=new Vector2(poX,poY);
			spriteMetaData.rect=new Rect(x,-y+textureHeight-height,width,height);
			spritesheet[i]=spriteMetaData;
		}
		return spritesheet;
	}
	/// <summary>
	/// 延时创建动画文件
	/// </summary>
	/// <param name="assetPath">Assets文件夹下有后缀的资源路径</param>
	/// <param name="assetNamePath">Assets文件夹下无后缀的资源路径</param>
	private async void DelayCreateAnimationFile(string assetPath,string assetNamePath){
		await Task.Delay(1);
		//获取切分后的图片
		Object[] objects=AssetDatabase.LoadAllAssetsAtPath(assetPath);
		int len=objects.Length;
		//objects列表中除了一个是Texture2D,其它都是Sprite（Texture2D在位置有可能在索引0或length-1）
		List<Sprite> spriteList=new List<Sprite>();
		for(int i=0;i<len;i++){
			Sprite sprite=objects[i] as Sprite;
			if(sprite!=null)spriteList.Add(sprite);
		}
		Sprite[] sprites=spriteList.ToArray();
		//创建动画文件
		CreateAnimationFile(AnimationType.Sprite,sprites,assetNamePath);
		CreateAnimationFile(AnimationType.Image,sprites,assetNamePath);
		//保存
		AssetDatabase.SaveAssets();
	}
	
	/// <summary>
	/// 创建动画文件
	/// </summary>
	/// <param name="type">动画的类型，用于标记是Sprite或Image</param>
	/// <param name="sprites">Sprite帧列表</param>
	/// <param name="assetNamePath">Assets文件夹下无后缀的资源路径</param>
	private void CreateAnimationFile(AnimationType type,Sprite[] sprites,string assetNamePath){
		int len=sprites.Length;
		AnimationClip animationClip=new AnimationClip();
		//帧频
		animationClip.frameRate=frameRate;
		//设置循环
		SerializedObject serializedClip=new SerializedObject(animationClip);
		SerializedProperty clipSettings=serializedClip.FindProperty("m_AnimationClipSettings");
		SerializedProperty loopTimeSet=clipSettings.FindPropertyRelative("m_LoopTime");
		loopTimeSet.boolValue=true;
		serializedClip.ApplyModifiedProperties();
		//动画曲线
		EditorCurveBinding curveBinding=new EditorCurveBinding();
		if(type==AnimationType.Sprite){
			curveBinding.type=typeof(SpriteRenderer);
		}else{
			curveBinding.type=typeof(Image);
		}
		curveBinding.path="";
		curveBinding.propertyName="m_Sprite";
		//添加帧
		ObjectReferenceKeyframe[] keyframes=new ObjectReferenceKeyframe[len];
		const float frameTime=1f/frameRate;
		for(int i=0;i<len;i++){
			Sprite sprite=sprites[i];
			ObjectReferenceKeyframe keyframe=new ObjectReferenceKeyframe();
			keyframe.time=frameTime*i;
			keyframe.value=sprite;
			keyframes[i]=keyframe;
		}
		AnimationUtility.SetObjectReferenceCurve(animationClip,curveBinding,keyframes);
		//创建.anim文件
		string animFilePath;
		if(type==AnimationType.Sprite){
			animFilePath=assetNamePath+"_sprite.anim";
		}else{
			animFilePath=assetNamePath+"_image.anim";
		}
		AssetDatabase.CreateAsset(animationClip,animFilePath);
		//创建.controller文件
		string controllerFilePath;
		if(type==AnimationType.Sprite){
			controllerFilePath=assetNamePath+"_sprite.controller";
		}else{
			controllerFilePath=assetNamePath+"_image.controller";
		}
		AnimatorController animatorController=AnimatorController.CreateAnimatorControllerAtPath(controllerFilePath);
		AnimatorControllerLayer layer=animatorController.layers[0];
		AnimatorStateMachine stateMachine=layer.stateMachine;
		AnimatorState state=stateMachine.AddState(animationClip.name);
		state.motion=animationClip;
		stateMachine.AddEntryTransition(state);
		AssetDatabase.SaveAssets();
	}
}
