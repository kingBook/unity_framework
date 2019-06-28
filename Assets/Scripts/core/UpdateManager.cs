using System.Collections.Generic;
/// <summary>
/// 更新管理器
/// <br>统一调用FixedUpdate、Update、LateUpdate、OnGUI、OnRenderObject</br>
/// <br>解决在压力状态下引起的效率低下问题，</br>
/// <br>具体描述:https://docs.unity3d.com/Manual/BestPracticeUnderstandingPerformanceInUnity8.html （Update managers部分）</br>
/// </summary>
public class UpdateManager{
	private List<IUpdate> _list=new List<IUpdate>();
	public void fixedUpdate(){
		int i=_list.Count;
		while(--i>=0){
			_list[i].FixedUpdate();
		}
	}
	public void update(){
		int i=_list.Count;
		while(--i>=0){
			_list[i].Update();
		}
	}
	public void lateUpdate(){
		int i=_list.Count;
		while(--i>=0){
			_list[i].LateUpdate();
		}
	}
	public void onGUI(){
		int i=_list.Count;
		while(--i>=0){
			_list[i].OnGUI();
		}
	}
	public void onRenderObject(){
		int i=_list.Count;
		while(--i>=0){
			_list[i].OnRenderObject();
		}
	}
	
	public void add(IUpdate item){
		//if(_list.Contains(item))return;//这个方法非常慢
		_list.Insert(0,item);
	}

	public void remove(IUpdate item){
		_list.Remove(item);
	}

	public void clear(){
		_list.Clear();
	}
}
