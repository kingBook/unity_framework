using System.Collections;
using UnityEngine;
/// <summary>
/// 游戏对象工具类
/// </summary>
public static class GameObjectUtil{
	/// <summary>
	/// 返回游戏对象及子级所有的 Renderer 组件构成的包围盒
	/// </summary>
	/// <param name="gameObject"></param>
	/// <returns></returns>
	public static Bounds GetRenderersBounds(GameObject gameObject){
		Bounds bounds=new Bounds();
		bounds.SetMinMax(Vector3.one*float.MaxValue,Vector3.one*float.MinValue);

		Renderer[] renderers=gameObject.GetComponentsInChildren<Renderer>();
		for(int i=0,len=renderers.Length;i<len;i++){
			Bounds rendererBounds=renderers[i].bounds;
			bounds.SetMinMax(Vector3.Min(bounds.min,rendererBounds.min),
							 Vector3.Max(bounds.max,rendererBounds.max));
		}
		return bounds;
	}
}