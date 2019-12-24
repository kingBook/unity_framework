﻿using System;
using UnityEngine;

public enum Language{AUTO,CN,EN}
	
/// <summary>
/// 整个应用程序的单例抽象类(基类)
/// <br>子类的以下方法：FixedUpdate、Update、LateUpdate、OnGUI、OnRenderObject，</br>
/// <br>将使用以下代替：FixedUpdate2、Update2、LateUpdate2、OnGUI2、OnRenderObject2。</br>
/// </summary>
public abstract class BaseApp<T>:BaseMonoBehaviour where T:class,new(){
	
	/// <summary>应用程序的单例实例</summary>
	public static T instance{ get; private set; }

	[Tooltip("标记为调试（不载入其他场景）")]
	[SerializeField] private bool m_isDebug=false;

	/// <summary>改变语言事件</summary>
	public event Action<Language> onChangeLanguage;

	[Tooltip("AUTO:运行时根据系统语言决定是CN/EN " +
	 "\nCN:中文 " +
	 "\nEN:英文")
	]
	[SerializeField,SetProperty("language")]//此处使用SetProperty序列化setter方法，用法： https://github.com/LMNRY/SetProperty
	protected Language m_language=Language.AUTO;

	[Tooltip("进度条")]
	[SerializeField] private Progressbar m_progressbar=null;

	[Tooltip("文件加载器")]
	[SerializeField] private FileLoader m_fileLoader=null;

	[Tooltip("场景加载器")]
	[SerializeField] private SceneLoader m_sceneLoader=null;

	[Tooltip("更新管理器")]
	[SerializeField] private UpdateManager m_updateManager=null;

	/// <summary>暂停或恢复事件，在调用setPause(bool)时方法发出</summary>
	public event Action<bool> onPauseOrResume;

	/// <summary>是否为调试模式，调试模式下不加载其他场景</summary>
	public bool isDebug{ get=>m_isDebug; }

	/// <summary>应用程序的语言</summary>
	public Language language{
		get => m_language;
		set{
			m_language=value;
			onChangeLanguage?.Invoke(m_language);
		}
	}

	/// <summary>进度条</summary>
	public Progressbar progressbar{ get => m_progressbar; }

	/// <summary>文件加载器</summary>
	public FileLoader fileLoader{ get => m_fileLoader; }

	/// <summary>场景加载器(有进度条)</summary>
	public SceneLoader sceneLoader{ get => m_sceneLoader; }

	/// <summary>更新管理器</summary>
	public UpdateManager updateManager{ get => m_updateManager; }

	/// <summary>是否已暂停</summary>
	public bool isPause{ get;private set; }

	/// <summary>是否第一次打开当前应用</summary>
	public bool isFirstOpen{ get; private set; }


	protected override void Awake() {
		base.Awake();
		instance=this as T;

		InitFirstOpenApp();

		if(m_language==Language.AUTO){
			InitLanguage();
		}
	}

	private void InitFirstOpenApp(){
		const string key="isFirstOpenApp";
		isFirstOpen=PlayerPrefs.GetInt(key,1)==1;
		if(isFirstOpen) {
			PlayerPrefs.SetInt(key,0);
			PlayerPrefs.Save();
		}
	}

	private void InitLanguage(){
		bool isCN=Application.systemLanguage==SystemLanguage.Chinese;
		isCN=isCN||Application.systemLanguage==SystemLanguage.ChineseSimplified;
		isCN=isCN||Application.systemLanguage==SystemLanguage.ChineseTraditional;
		m_language=isCN?Language.CN:Language.EN;
		//改变语言事件
		onChangeLanguage?.Invoke(m_language);
	}

	/// <summary>
	/// 设置暂停/恢复更新、物理模拟
	/// </summary>
	/// <param name="isPause">是否暂停</param>
	/// <param name="isSetPhysics">是否设置物理引擎</param>
	/// <param name="isSetVolume">是否设置音量</param>
	public void SetPause(bool isPause,bool isSetPhysics=true, bool isSetVolume=true){
		if(this.isPause==isPause)
			return;
		this.isPause=isPause;
		if(isSetPhysics){
			//暂停或恢复3D物理模拟
			Physics.autoSimulation=!this.isPause;
			//暂停或恢复2D物理模拟
			Physics2D.autoSimulation=!this.isPause;
		}
		if(isSetVolume){
			AudioListener.pause=this.isPause;
		}
		//发出事件
		onPauseOrResume?.Invoke(isPause);
	}
	
	protected override void OnDestroy(){
		base.OnDestroy();
		//不需要销毁instance
		//instance=null;
	}

	
	
	


}