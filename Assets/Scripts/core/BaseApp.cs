using System;
using System.Collections.Generic;
using UnityEngine;

public enum Language{AUTO,CN,EN}
	
/// <summary>
/// 整个应用程序的单例抽象类(基类)
/// <br>子类的以下方法：FixedUpdate、Update、LateUpdate、OnGUI、OnRenderObject，</br>
/// <br>将使用以下代替：FixedUpdate2、Update2、LateUpdate2、OnGUI2、OnRenderObject2。</br>
/// </summary>
public abstract class BaseApp<T>:BaseMonoBehaviour where T:class,new(){
	
	protected static T _instance;
	/// <summary>
	/// 应用程序的单例实例
	/// </summary>
	public static T instance{ get => _instance; }

	[Tooltip("用于调试其它场景需要调用该脚本，" +
	 "\n在Hierarchy中拖入该脚本所在的.unity文件时，" +
	 "\n不执行不载入标题、其他场景等，将在代码中判定实现" +
	 "\n发布工程时必须为false。")
	]
	[SerializeField]
	private bool _isDebug=false;

	/// <summary>
	/// 改变语言事件
	/// </summary>
	public event Action<Language> onChangeLanguage;
	[Tooltip("AUTO:运行时根据系统语言决定是CN/EN " +
	 "\nCN:中文 " +
	 "\nEN:英文")
	]
	[SerializeField,SetProperty("language")]//此处使用SetProperty序列化setter方法，用法： https://github.com/LMNRY/SetProperty
	protected Language _language=Language.AUTO;

	[Tooltip("场景加载器")]
	[SerializeField]
	private SceneLoader _sceneLoader=null;

	[Tooltip("更新管理器")]
	[SerializeField]
	private UpdateManager _updateManager=null;

	/// <summary>
	/// 暂停或恢复事件，在调用setPause(bool)时方法发出
	/// </summary>
	public event Action<bool> onPauseOrResume;
	private bool _isPause;

	protected override void Awake() {
		base.Awake();
		//再次加载场景时，如果已有实例则删除
		if(_instance==null){
			_instance=this as T;
			if(_language==Language.AUTO){
				initLanguage();
			}
		}else{
			Destroy(gameObject);
		}
	}

	private void initLanguage(){
		bool isCN=Application.systemLanguage==SystemLanguage.Chinese;
		isCN=isCN||Application.systemLanguage==SystemLanguage.ChineseSimplified;
		isCN=isCN||Application.systemLanguage==SystemLanguage.ChineseTraditional;
		_language=isCN?Language.CN:Language.EN;
		//改变语言事件
		onChangeLanguage?.Invoke(_language);
	}

	/// <summary>
	/// 设置暂停/恢复更新、物理模拟
	/// </summary>
	/// <param name="value"></param>
	public void setPause(bool value){
		if(_isPause==value)return;
		_isPause=value;
		//暂停或恢复3D物理模拟
		Physics.autoSimulation=!_isPause;
		//暂停或恢复2D物理模拟
		Physics2D.autoSimulation=!_isPause;
		//发出事件
		onPauseOrResume?.Invoke(value);
	}
	
	protected override void OnDestroy(){
		if(_instance!=null){
			if(_instance.Equals(this)){
				_instance=null;
			}
		}
		base.OnDestroy();
	}

	public bool isDebug{ get => _isDebug; }
	
	/// <summary>
	/// 应用程序的语言
	/// </summary>
	public Language language{
		get => _language;
		set{
			_language=value;
			onChangeLanguage?.Invoke(_language);
		}
	}

	/// <summary>
	/// 场景加载器(有进度条)
	/// </summary>
	public SceneLoader sceneLoader{ get => _sceneLoader; }

	/// <summary>
	/// 更新管理器
	/// </summary>
	public UpdateManager updateManager{ get => _updateManager; }

	/// <summary>
	/// 是否已暂停
	/// </summary>
	public bool isPause{ get => _isPause; }


}
