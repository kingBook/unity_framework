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

	/// <summary>
	/// 打印一个数组的所有元素，如：
	/// <br>int[] list=new int[]{1,2,3};</br>
	/// <br>Debug2.LogArray(list);</br>
	/// <br>输出：1, 2, 3</br>
	/// </summary>
	/// <typeparam name="T"></typeparam>
	/// <param name="list">打印的数组</param>
	/// <param name="pre">前缀字符</param>
	/// <param name="end">后缀字符</param>
	public static void LogArray<T>(T[] list,object pre=null,object end=null){
		int len=list.Length;
		object obj;
		string str="";
		for(int i=0;i<len;i++){
			obj=list[i];
			str+=(obj==null)?"Null":obj.ToString();
			if(i<len-1)str+=", ";
		}
		string preStr=pre==null?"":pre.ToString();
		string endStr=end==null?"":end.ToString();
		str=preStr+" "+str+" "+endStr;
		UnityEngine.Debug.Log(str);
	}
}