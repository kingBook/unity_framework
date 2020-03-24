using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

public class EditorTest:Editor{
	[MenuItem("Tools/EditorTest")]
	public static void test(){
		if(EditorApplication.isPlaying)return;
		Debug.Log("test");
	}

}