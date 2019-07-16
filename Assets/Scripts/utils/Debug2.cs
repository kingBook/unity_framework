using System.Diagnostics;
/// <summary>
/// 自定义的Log类，解决Debug.Log()/Debug.LogFormat()多参数不方便和在发布版本Log不剔除问题
/// <br>具体描述：https://docs.unity3d.com/Manual/BestPracticeUnderstandingPerformanceInUnity7.html (Debug code & the [conditional] attribute部分)</br>
/// </summary>
public static class Debug2 {
	//如果传递给 Conditional 属性的任何定义均未被定义，则会被修饰的方法以及对被修饰方法的所有调用都会在编译中剔除
	[Conditional("UNITY_EDITOR")]
	public static void Log(params object[] args) {
		int len=args.Length;
		object obj;
		string str="";
		for(int i=0;i<len;i++){
			obj=args[i];
			str+=(obj==null)?"Null":obj.ToString();
			if(i<len-1)str+=' ';
		}
		UnityEngine.Debug.Log(str);
	}
}
