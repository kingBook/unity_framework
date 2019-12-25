using UnityEngine;

/// <summary>
/// 所有UI回调函数的基类,每一个场景都应该实现一个子类(如果场景中有UI)
/// </summary>
public abstract class UICallbacksBase:BaseMonoBehaviour{

	/// <summary>
	/// 记录在静音时的音量，取消静音时恢复
	/// </summary>
	private float m_globalVolume;

	protected override void Awake() {
		base.Awake();
		//记录全局音量
		m_globalVolume=AudioListener.volume;
	}

	/// <summary>
	/// 更多游戏
	/// </summary>
	public void MoreGame(){
		Debug2.Log("moreGame");
	}

	/// <summary>
	/// 切换静音
	/// </summary>
	public void ToggleMute(){
		if(AudioListener.volume>0){
			//音量>0，则记录音量，并静音
			m_globalVolume=AudioListener.volume;
			AudioListener.volume=0;
		}else{
			//取消静音
			AudioListener.volume=m_globalVolume;
		}
	}

	/// <summary>
	/// 切换暂停
	/// </summary>
	public void TogglePause(){
		bool isPause=App.instance.isPause;
		App.instance.SetPause(!isPause);
	}
	
}
