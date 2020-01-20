using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 更新管理器
/// <br>统一调用FixedUpdate、Update、LateUpdate、OnGUI、OnRenderObject</br>
/// <br>解决在压力状态下引起的效率低下问题，</br>
/// <br>具体描述:https://docs.unity3d.com/Manual/BestPracticeUnderstandingPerformanceInUnity8.html （Update managers部分）</br>
/// </summary>
public sealed class UpdateManager:MonoBehaviour{
	private readonly List<IUpdate> m_list=new List<IUpdate>();
	private int m_length=0;
	private bool m_isPause;

	private void Start() {
		App.instance.onPauseOrResume+=onPauseOrResume;
	}

	private void onPauseOrResume(bool isPause){
		m_isPause=isPause;
	}

	private void FixedUpdate(){
		if(m_isPause)return;
		int i=m_length;
		while(--i>=0){
			m_list[i].FixedUpdate();
		}
	}
	private void Update(){
		if(m_isPause)return;
		int i=m_length;
		while(--i>=0){
			m_list[i].Update();
		}
	}
	private void LateUpdate(){
		if(m_isPause)return;
		int i=m_length;
		while(--i>=0){
			m_list[i].LateUpdate();
		}
	}
	private void OnGUI(){
		if(m_isPause)return;
		int i=m_length;
		while(--i>=0){
			m_list[i].OnGUI();
		}
	}
	private void OnRenderObject(){
		if(m_isPause)return;
		int i=m_length;
		while(--i>=0){
			m_list[i].OnRenderObject();
		}
	}
	
	public void Add(IUpdate item){
		//if(m_list.Contains(item))return;//这个方法非常慢
		m_list.Insert(0,item);
		m_length++;
	}

	public void Remove(IUpdate item){
		bool isSuccess=m_list.Remove(item);
		if(isSuccess){
			m_length--;
		}
	}

	public void Clear(){
		m_list.Clear();
		m_length=0;
	}

	private void OnDestroy(){
		App.instance.onPauseOrResume-=onPauseOrResume;
	}
}