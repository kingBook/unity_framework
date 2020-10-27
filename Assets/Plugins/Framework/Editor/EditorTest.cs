using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

public class EditorTest:Editor{
	[MenuItem("Tools/EditorTest")]
	public static void Test(){
		if(EditorApplication.isPlaying)return;
		Debug.Log("Test");
	}

}