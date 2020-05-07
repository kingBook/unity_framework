using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;
/// <summary>
/// FBX动画分割
/// </summary>
public class FbxAnimationSpliter:Editor{
	
	/// <summary>当前项目的路径</summary>
	private static readonly string projectPath=Environment.CurrentDirectory.Replace("\\","/");
	
	[MenuItem("Assets/Split FBX Animation",true)]
	private static bool ValidateSplitFbxAnimation(){
		string[] guids=Selection.assetGUIDs;
		int i=guids.Length;
		while(--i>=0){
			string guid=guids[i];
			string assetPath=AssetDatabase.GUIDToAssetPath(guid);
			string extension=System.IO.Path.GetExtension(assetPath).ToLower();
			if(extension==".fbx"){
				return true;
			}
		}
		return false;
	}
	
	[MenuItem("Assets/Split FBX Animation")]
	private static void SplitFbxAnimation(){
		string[] guids=Selection.assetGUIDs;
		int i=guids.Length;
		while(--i>=0){
			string guid=guids[i];
			string assetPath=AssetDatabase.GUIDToAssetPath(guid);
			string extension=Path.GetExtension(assetPath).ToLower();
			if(extension!=".fbx")continue;
			
			//去除后缀的资源相对路径，如：Assets/Models/testFBX
			string assetNamePath=assetPath.Substring(0,assetPath.LastIndexOf('.'));
			//.txt绝对路径，如：D:/projects/unity_test/Assets/Models/testFBX.txt
			string txtPath=projectPath+'/'+assetNamePath+".txt";
			
			if(File.Exists(txtPath)){
				StreamReader streamReader =File.OpenText(txtPath);
				string sAnimList=streamReader.ReadToEnd();
				streamReader.Close();
				
				var clipAnimations=ParseAnimFile(sAnimList);
				
				ModelImporter modelImporter=(ModelImporter)AssetImporter.GetAtPath(assetPath);
				modelImporter.clipAnimations=clipAnimations;
				EditorUtility.SetDirty(modelImporter);
				modelImporter.SaveAndReimport();
			}
		}
	}
	
	/*
	 //AssetPostprocessor时，使用此方法
	 private void OnPreprocessAnimation(){
		//去除后缀的资源相对路径，如：Assets/Models/testFBX
		string assetNamePath=assetPath.Substring(0,assetPath.LastIndexOf('.'));
		//.txt绝对路径，如：D:/projects/unity_test/Assets/Models/testFBX.txt
		string txtPath=projectPath+'/'+assetNamePath+".txt";
		
		if(File.Exists(txtPath)){
			StreamReader streamReader =File.OpenText(txtPath);
			string sAnimList=streamReader.ReadToEnd();
			streamReader.Close();
			
			var clipAnimations=ParseAnimFile(sAnimList);
			ModelImporter modelImporter=assetImporter as ModelImporter;
			modelImporter.clipAnimations=clipAnimations;
		}
	}*/
	
	/// <summary>
	/// 解析动画
	/// .txt文件格式：
	/// "起始帧-结束帧 loop或noloop(可选的) 动画名称(可以包含空格)"
	/// 如：
	/// 0-10 loop idle
	/// 11-25 noloop jump up
	/// </summary>
	/// <param name="sAnimList"></param>
	/// <returns></returns>
	private static ModelImporterClipAnimation[] ParseAnimFile(string sAnimList){
		List<ModelImporterClipAnimation> list=new List<ModelImporterClipAnimation>();
		
		Regex regexString=new Regex(" *(?<firstFrame>[0-9]+) *- *(?<lastFrame>[0-9]+) *(?<loop>(loop|noloop| )) *(?<name>[^\r^\n]*[^\r^\n^ ])",RegexOptions.Compiled|RegexOptions.ExplicitCapture);

		Match match=regexString.Match(sAnimList,0);
		while(match.Success){
			ModelImporterClipAnimation clip=new ModelImporterClipAnimation();

			if(match.Groups["firstFrame"].Success){
				clip.firstFrame=System.Convert.ToInt32(match.Groups["firstFrame"].Value,10);
			}

			if(match.Groups["lastFrame"].Success){
				clip.lastFrame=System.Convert.ToInt32(match.Groups["lastFrame"].Value,10);
			}

			if(match.Groups["loop"].Success){
				clip.loopTime=match.Groups["loop"].Value=="loop";
			}

			if(match.Groups["name"].Success){
				clip.name=match.Groups["name"].Value;
			}
			
			list.Add(clip);
			
			match=regexString.Match(sAnimList,match.Index+match.Length);
		}
		return list.ToArray();
	}
}