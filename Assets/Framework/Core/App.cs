﻿#pragma warning disable 0649

using DG.Tweening;
using System;
using System.Reflection;
using UnityEngine;

/// <summary>
/// 整个应用程序的单例
/// </summary>
public sealed class App : MonoBehaviour {

    /// <summary> 应用程序的单例实例 </summary>
    public static App instance { get; private set; }

    public enum Language { AUTO, CN, EN }

    /// <summary> 暂停或恢复事件，在调用setPause(bool)时方法发出，回调函数格式：<code> void OnPauseOrResumeHandler(bool isPause) </code> </summary>
    public event Action<bool> onPauseOrResumeEvent;
    /// <summary> 更改语言事件, 回调函数格式: <code> void OnChangedLanguageHandler(App.Language language) </code> </summary>
    public event Action<Language> onChangedLanguageEvent;


    [Tooltip("AUTO:运行时根据系统语言决定是CN/EN " +
     "\nCN:中文 " +
     "\nEN:英文")
    ]
    [SerializeField, SetProperty(nameof(language))] // 此处使用SetProperty序列化setter方法，用法： https://github.com/LMNRY/SetProperty
    private Language m_language = Language.AUTO;
    [Tooltip("进度条")]
    [SerializeField] private PanelProgressbar m_panelProgressbar;
    [Tooltip("开始的 Logo 屏幕")]
    [SerializeField] private PanelLogoScreen m_panelLogoScreen;
    [Tooltip("调试助手面板")]
    [SerializeField] private PanelDebugHelper m_panelDebugHelper;
    [Tooltip("文件加载器")]
    [SerializeField] private FileLoader m_fileLoader;
    [Tooltip("场景加载器")]
    [SerializeField] private SceneLoader m_sceneLoader;
    [Tooltip("音频管理器")]
    [SerializeField] private AudioManager m_audioManager;
    [Tooltip("移动设备震动器")]
    [SerializeField] private Vibrator m_vibrator;
    [Tooltip("游戏列表")]
    [SerializeField] private BaseGame[] m_games = new BaseGame[0];


    /// <summary> 应用程序的语言 </summary>
    public Language language {
        get => m_language;
        set {
            m_language = value;
            onChangedLanguageEvent?.Invoke(m_language);
        }
    }

    /// <summary> 进度条 </summary>
    public PanelProgressbar panelProgressbar => m_panelProgressbar;

    /// <summary> 开始的 Logo 屏幕 </summary>
    public PanelLogoScreen panelLogoScreen => m_panelLogoScreen;

    /// <summary> 调试助手面板 </summary>
    public PanelDebugHelper panelDebugHelper => m_panelDebugHelper;

    /// <summary> 文件加载器 </summary>
    public FileLoader fileLoader => m_fileLoader;

    /// <summary> 场景加载器(有进度条) </summary>
    public SceneLoader sceneLoader => m_sceneLoader;

    /// <summary> 音频管理器 </summary>
    public AudioManager audioManager => m_audioManager;

    /// <summary> 移动设备震动器 </summary>
    public Vibrator vibrator => m_vibrator;

    /// <summary>
    /// 返回 <see cref="m_games"/>[0]
    /// </summary>
    /// <typeparam name="T"> <see cref="BaseGame"/> </typeparam>
    /// <returns></returns>
    public T GetGame<T>() where T : BaseGame => (T)m_games[0];

    /// <summary>
    /// 返回 <see cref="m_games"/>[index]
    /// </summary>
    /// <typeparam name="T"> <see cref="BaseGame"/> </typeparam>
    /// <param name="index"> 索引 </param>
    /// <returns></returns>
    public T GetGame<T>(int index) where T : BaseGame => (T)m_games[index];

    /// <summary>
    /// 返回 <see cref="m_games"/>.Length
    /// </summary>
    public int gameCount => m_games.Length;

    /// <summary> 是否已暂停 </summary>
    public bool isPause { get; private set; }

    /// <summary> 打开应用的次数 </summary>
    public int openCount { get; private set; }


    /// <summary>
    /// 设置暂停/恢复更新、物理模拟
    /// </summary>
    /// <param name="isPause"> 是否暂停 </param>
    /// <param name="isSetPhysics"> 是否设置物理引擎 </param>
    /// <param name="isSetVolume"> 是否设置音量 </param>
    public void SetPause(bool isPause, bool isSetPhysics = true, bool isSetVolume = true) {
        if (this.isPause == isPause) return;
        this.isPause = isPause;
        if (isSetPhysics) {
            // 暂停或恢复3D物理模拟
            Physics.autoSimulation = !this.isPause;
            // 暂停或恢复2D物理模拟
            Physics2D.autoSimulation = !this.isPause;
        }
        if (isSetVolume) {
            AudioListener.pause = this.isPause;
        }
        // 发出事件
        onPauseOrResumeEvent?.Invoke(isPause);
    }

    private void InitDOTween() {
        DOTween.SetTweensCapacity(500, 500);
    }

    private void AddOpenCount() {
        const string key = "ApplicationOpenCount";
        openCount = PlayerPrefs.GetInt(key, 0) + 1;
        PlayerPrefs.SetInt(key, openCount);
        PlayerPrefs.Save();
    }

    private void InitLanguage() {
        bool isCN = Application.systemLanguage == SystemLanguage.Chinese;
        isCN = isCN || Application.systemLanguage == SystemLanguage.ChineseSimplified;
        isCN = isCN || Application.systemLanguage == SystemLanguage.ChineseTraditional;
        m_language = isCN ? Language.CN : Language.EN;
        //改变语言事件
        onChangedLanguageEvent?.Invoke(m_language);
    }

    private void Awake() {
        instance = this;
        // 初始化 DOTween
        InitDOTween();
        // 增加应用打开的次数 
        AddOpenCount();
        // 初始化语言
        if (m_language == Language.AUTO) {
            InitLanguage();
        }
    }

    private void OnApplicationQuit() {
#if UNITY_EDITOR // 自定义进入播放模式（不重新加载域时），销毁 DOTween.instance
        if (DOTween.instance != null) {
            DOTween.Clear(true);
            FieldInfo isQuittingField = typeof(DOTween).GetField("isQuitting", BindingFlags.Static | BindingFlags.NonPublic);
            isQuittingField.SetValue(DOTween.instance, false);
            Destroy(DOTween.instance);
            DOTween.instance = null;
        }
#endif
    }

    private void OnDestroy() {
        // 不需要销毁instance
        //instance=null;
    }
}