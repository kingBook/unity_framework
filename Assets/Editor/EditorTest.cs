using UnityEditor;
using UnityEngine;

public class EditorTest:Editor{
	[MenuItem("Tools/EditorTest")]
	public static void test(){
		Debug.Log("test");
	}
}