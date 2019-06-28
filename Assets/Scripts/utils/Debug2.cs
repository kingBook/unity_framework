using System.Diagnostics;
/// <summary>
/// 自定义的Log类，解决Debug.Log()不能多参数和在发布版本Log不剔除问题
/// <br>具体描述：https://docs.unity3d.com/Manual/BestPracticeUnderstandingPerformanceInUnity7.html (Debug code & the [conditional] attribute部分)</br>
/// </summary>
public static class Debug2 {
	//如果传递给 Conditional 属性的任何定义均未被定义，则会被修饰的方法以及对被修饰方法的所有调用都会在编译中剔除
	[Conditional("UNITY_EDITOR")]
	public static void Log(params object[] args) {
		string format="";
		for(int i=0;i<args.Length;i++){
			format+="{"+i+"}"+(i<args.Length-1?",":"");
		}
		UnityEngine.Debug.LogFormat(format,args);
	}
}
