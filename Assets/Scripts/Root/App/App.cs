#pragma warning disable 0649

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

    public enum Language { Auto, Cn, En }

    /// <summary> 暂停或恢复事件，在调用setPause(bool)时方法发出，回调函数格式：<code> void OnPauseOrResumeHandler(bool isPause) </code> </summary>
    public event Action<bool> onPauseOrResumeEvent;
    /// <summary> 更改语言事件, 回调函数格式: <code> void OnChangedLanguageHandler(App.Language language) </code> </summary>
    public event Action<Language> onChangedLanguageEvent;

    // 此处使用SetProperty序列化setter方法，用法： https://github.com/LMNRY/SetProperty
    [SerializeField, SetProperty(nameof(language)), Tooltip("AUTO:运行时根据系统语言决定是CN/EN \nCN:中文 \nEN:英文")]
    private Language _language = Language.Auto;
    [SerializeField, Tooltip("main 场景的主相机")] private Camera _cameraMain;
    [SerializeField, Tooltip("进度条")] private PanelProgressbar _panelProgressbar;
    [SerializeField, Tooltip("开始的 Logo 屏幕")] private PanelLogoScreen _panelLogoScreen;
    [SerializeField, Tooltip("调试助手面板")] private PanelDebugHelper _panelDebugHelper;

    /// <summary> 文件加载器 </summary>
    private FileLoader _fileLoader;
    /// <summary> 场景加载器 </summary>
    private SceneLoader _sceneLoader;
    /// <summary> 音频管理器 </summary>
    private AudioManager _audioManager;
    /// <summary> 移动设备振动器 </summary>
    private Vibrator _vibrator;


    /// <summary> 应用程序的语言 </summary>
    public Language language {
        get => _language;
        set {
            _language = value;
            onChangedLanguageEvent?.Invoke(_language);
        }
    }

    /// <summary> 进度条 </summary>
    public PanelProgressbar panelProgressbar => _panelProgressbar;

    /// <summary> 开始的 Logo 屏幕 </summary>
    public PanelLogoScreen panelLogoScreen => _panelLogoScreen;

    /// <summary> 调试助手面板 </summary>
    public PanelDebugHelper panelDebugHelper => _panelDebugHelper;

    /// <summary> 文件加载器 </summary>
    public FileLoader fileLoader => _fileLoader;

    /// <summary> 场景加载器(有进度条) </summary>
    public SceneLoader sceneLoader => _sceneLoader;

    /// <summary> 音频管理器 </summary>
    public AudioManager audioManager => _audioManager;

    /// <summary> 移动设备震动器 </summary>
    public Vibrator vibrator => _vibrator;

    /// <summary> 游戏类 </summary>
    public Game game { get; private set; }

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
            Physics.simulationMode = !this.isPause ? SimulationMode.FixedUpdate : SimulationMode.Script;
            // 暂停或恢复2D物理模拟
            Physics2D.simulationMode = !this.isPause ? SimulationMode2D.FixedUpdate : SimulationMode2D.Script;
        }
        if (isSetVolume) {
            AudioListener.pause = this.isPause;
        }
        // 发出事件
        onPauseOrResumeEvent?.Invoke(isPause);
    }

    private void InitDoTween() {
        // 设置 DOTween 缓动和序列的最大数量
        DOTween.SetTweensCapacity(500, 500);
    }

    /// <summary> 打开应用的次数 </summary>
    private void AddOpenCount() {
        const string key = "ApplicationOpenCount";
        openCount = PlayerPrefs.GetInt(key, 0) + 1;
        PlayerPrefs.SetInt(key, openCount);
        PlayerPrefs.Save();
    }

    /// <summary> 初始语言 </summary>
    private void InitLanguage() {
        bool isCn = Application.systemLanguage == SystemLanguage.Chinese;
        isCn = isCn || Application.systemLanguage == SystemLanguage.ChineseSimplified;
        isCn = isCn || Application.systemLanguage == SystemLanguage.ChineseTraditional;
        _language = isCn ? Language.Cn : Language.En;

        //改变语言事件
        onChangedLanguageEvent?.Invoke(_language);
    }

    private void Awake() {
        instance = this;

        // 初始化 DOTween
        InitDoTween();

        // 增加应用打开的次数 
        AddOpenCount();

        // 初始化语言
        if (_language == Language.Auto) {
            InitLanguage();
        }

        // 文件加载器
        _fileLoader = GameObjectUtil.AddNewChildAndComponentToNode<FileLoader>(gameObject);
        _fileLoader.Init(_panelProgressbar);

        // 场景加载器
        _sceneLoader = GameObjectUtil.AddNewChildAndComponentToNode<SceneLoader>(gameObject);
        _sceneLoader.Init(_cameraMain, _panelProgressbar);

        // 音频管理
        _audioManager = GameObjectUtil.AddNewChildAndComponentToNode<AudioManager>(gameObject);

        // 振动管理
        _vibrator = GameObjectUtil.AddNewChildAndComponentToNode<Vibrator>(gameObject);

        // 游戏类
        game = GameObjectUtil.AddNewChildAndComponentToNode<Game>(gameObject);

    }

    private void OnApplicationQuit() {
#if UNITY_EDITOR // 自定义进入播放模式（不重新加载域时），销毁 DOTween.instance
        DOTween.Clear(true);
        if (DOTween.instance != null) {
            FieldInfo isQuittingField = typeof(DOTween).GetField("isQuitting", BindingFlags.Static | BindingFlags.NonPublic);
            isQuittingField.SetValue(DOTween.instance, false);
            Destroy(DOTween.instance);
            DOTween.instance = null;
        }
#endif
    }

    private void OnDestroy() {
        instance = null;
    }
}