using UnityEngine;
using System.Collections;
using System.Collections.Generic;

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
