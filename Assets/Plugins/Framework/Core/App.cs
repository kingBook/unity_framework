using DG.Tweening;
using System;
using System.Reflection;
using UnityEngine;
#pragma warning disable 0649

/// <summary>
/// 整个应用程序的单例
/// </summary>
public sealed class App:BaseMonoBehaviour{
	
	/// <summary>应用程序的单例实例</summary>
	public static App instance{ get; private set; }

	[Tooltip("标记为调试（不载入其他场景）")]
	[SerializeField] private bool m_isDebug=false;

	public enum Language{AUTO,CN,EN}
	/// <summary>改变语言事件</summary>
	public event Action<Language> onChangeLanguageEvent;

	[Tooltip("AUTO:运行时根据系统语言决定是CN/EN " +
	 "\nCN:中文 " +
	 "\nEN:英文")
	]
	[SerializeField,SetProperty(nameof(language))]//此处使用SetProperty序列化setter方法，用法： https://github.com/LMNRY/SetProperty
	private Language m_language=Language.AUTO;
	
	[Tooltip("全局用于播放不循环音频的AudioSource")]
	[SerializeField] private AudioSource m_effectAudioSource;
	
	[Tooltip("全局用于播放循环音频的AudioSource（unity中同时播放多个循环音频时，需要在不同GameObject添加多个AudioSource）")]
	[SerializeField] private AudioSource m_loopAudioSource;

	[Tooltip("进度条")]
	[SerializeField] private PanelProgressbar m_panelProgressbar;

	[Tooltip("开始Logo屏幕")]
	[SerializeField] private PanelLogoScreen m_panelLogoScreen;

	[Tooltip("文件加载器")]
	[SerializeField] private FileLoader m_fileLoader;

	[Tooltip("场景加载器")]
	[SerializeField] private SceneLoader m_sceneLoader;

	[Tooltip("更新管理器")]
	[SerializeField] private UpdateManager m_updateManager;

	[Tooltip("游戏列表")]
	[SerializeField] private BaseGame[] m_games=new BaseGame[0];

	/// <summary>暂停或恢复事件，在调用setPause(bool)时方法发出</summary>
	public event Action<bool> onPauseOrResumeEvent;

	/// <summary>是否为调试模式，调试模式下不加载其他场景</summary>
	public bool isDebug=>m_isDebug;

	/// <summary>应用程序的语言</summary>
	public Language language{
		get => m_language;
		set{
			m_language=value;
			onChangeLanguageEvent?.Invoke(m_language);
		}
	}
	
	/// <summary>全局用于播放不循环音频的AudioSource</summary>
	public AudioSource effectAudioSource=>m_effectAudioSource;
	
	/// <summary>全局用于播放循环音频的AudioSource（unity中同时播放多个循环音频时，需要在不同GameObject添加多个AudioSource）</summary>
	public AudioSource loopAudioSource=>m_loopAudioSource;
	
	/// <summary>进度条</summary>
	public PanelProgressbar panelProgressbar=>m_panelProgressbar;

	/// <summary>开始Logo屏幕</summary>
	public PanelLogoScreen panelLogoScreen=>m_panelLogoScreen;

	/// <summary>文件加载器</summary>
	public FileLoader fileLoader=>m_fileLoader;

	/// <summary>场景加载器(有进度条)</summary>
	public SceneLoader sceneLoader=>m_sceneLoader;

	/// <summary>更新管理器</summary>
	public UpdateManager updateManager=>m_updateManager;

	/// <summary>
	/// 返回<see cref="m_games"/>[0]
	/// </summary>
	/// <typeparam name="T"><see cref="BaseGame"/></typeparam>
	/// <returns></returns>
	public T GetGame<T>() where T:BaseGame=>(T)m_games[0];

	/// <summary>
	/// 返回<see cref="m_games"/>[index]
	/// </summary>
	/// <typeparam name="T"><see cref="BaseGame"/></typeparam>
	/// <param name="index">索引</param>
	/// <returns></returns>
	public T GetGame<T>(int index) where T:BaseGame=>(T)m_games[index];

	/// <summary>
	/// 返回<see cref="m_games"/>.Length
	/// </summary>
	public int gameCount=>m_games.Length;

	/// <summary>是否已暂停</summary>
	public bool isPause{ get;private set; }

	/// <summary>打开应用的次数</summary>
	public int openCount{ get; private set; }

	protected override void Awake() {
		base.Awake();
		instance=this;
		AddOpenCount();
		
		if(m_language==Language.AUTO){
			InitLanguage();
		}
	}

	private void AddOpenCount(){
		const string key="ApplicationOpenCount";
		openCount=PlayerPrefs.GetInt(key,0)+1;
		PlayerPrefs.SetInt(key,openCount);
		PlayerPrefs.Save();
	}

	private void InitLanguage(){
		bool isCN=Application.systemLanguage==SystemLanguage.Chinese;
		isCN=isCN||Application.systemLanguage==SystemLanguage.ChineseSimplified;
		isCN=isCN||Application.systemLanguage==SystemLanguage.ChineseTraditional;
		m_language=isCN?Language.CN:Language.EN;
		//改变语言事件
		onChangeLanguageEvent?.Invoke(m_language);
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
		onPauseOrResumeEvent?.Invoke(isPause);
	}

	protected override void OnApplicationQuit(){
		base.OnApplicationQuit();
		#if UNITY_EDITOR //自定义进入播放模式（不重新加载域时），销毁DOTween.instance
		if(DOTween.instance!=null){
			DOTween.Clear(true);
			FieldInfo isQuittingField=typeof(DOTween).GetField("isQuitting",BindingFlags.Static|BindingFlags.NonPublic);
			isQuittingField.SetValue(DOTween.instance,false);
			Destroy(DOTween.instance);
			DOTween.instance=null;
		}
		#endif
	}

	protected override void OnDestroy(){
		base.OnDestroy();
		//不需要销毁instance
		//instance=null;
	}
}