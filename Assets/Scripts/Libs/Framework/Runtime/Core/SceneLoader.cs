﻿#pragma warning disable 0649

using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

/// <summary>
/// 场景加载器
/// </summary>
public sealed class SceneLoader : MonoBehaviour {

    [Tooltip("场景加完成后，是否调用SceneManager.SetActiveScene(scene)设置为激活场景")]
    public bool isActiveSceneOnLoaded = true;

    [Tooltip("进度条"), SerializeField]
    private PanelProgressbar m_panelProgressbar;

    [SerializeField]
    private Camera m_cameraStart;

    private AsyncOperation m_asyncOperation;

    private void Awake() {

    }

    private void OnEnable() {
        SceneManager.sceneLoaded += OnSceneLoaded;
        SceneManager.activeSceneChanged += OnActiveSceneChanged;
    }

    /// <summary>
    /// Additive模式同步加载场景
    /// </summary>
    /// <param name="sceneName">场景在BuildSettings窗口的路径或名称</param>
    public void Load(string sceneName) {
        Load(sceneName, LoadSceneMode.Additive);
    }

    /// <summary>
    /// 同步加载场景
    /// （注意：LoadSceneMode.Additive 模式加载场景时，被加载场景里的对象不能在 Awake() 或 OnEnable() 里访问 Camera.main, 会访问到 Main 场景的主相机）
    /// </summary>
    /// <param name="sceneName">场景在BuildSettings窗口的路径或名称</param>
    /// <param name="mode">加载模式</param>
    public void Load(string sceneName, LoadSceneMode mode) {
        SceneManager.LoadScene(sceneName, mode);
        //为了能够侦听场景加载完成时设置为激活场景,所以激活
        gameObject.SetActive(true);
        m_panelProgressbar.gameObject.SetActive(true);
        m_panelProgressbar.SetProgress(1.0f);
    }

    /// <summary>
    /// Additive模式异步加载场景，将显示进度条
    /// （注意：LoadSceneMode.Additive 模式加载场景时，被加载场景里的对象不能在 Awake() 或 OnEnable() 里访问 Camera.main, 会访问到 Main 场景的主相机）
    /// </summary>
    /// <param name="sceneName">场景在BuildSettings窗口的路径或名称</param>
    public void LoadAsync(string sceneName) {
        LoadAsync(sceneName, LoadSceneMode.Additive);
    }

    /// <summary>
    /// 异步加载场景，将显示进度条
    /// （注意：LoadSceneMode.Additive 模式加载场景时，被加载场景里的对象不能在 Awake() 或 OnEnable() 里访问 Camera.main, 会访问到 Main 场景的主相机）
    /// </summary>
    /// <param name="sceneName">场景在BuildSettings窗口的路径或名称</param>
    /// <param name="mode">加载模式,默认为：LoadSceneMode.Additive</param>
    public void LoadAsync(string sceneName, LoadSceneMode mode) {
        gameObject.SetActive(true);
        m_panelProgressbar.gameObject.SetActive(true);
        m_panelProgressbar.SetProgress(0.0f);
        m_panelProgressbar.SetText("Loading 0%...");
        StartCoroutine(LoadSceneAsync(sceneName, mode));
    }

    public void LoadAsyncOnBackgroud(string sceneName) {
        LoadAsyncOnBackgroud(sceneName, LoadSceneMode.Additive);
    }

    public void LoadAsyncOnBackgroud(string sceneName, LoadSceneMode mode) {
        
    }


    private IEnumerator LoadSceneAsync(string sceneName, LoadSceneMode mode) {
        m_asyncOperation = SceneManager.LoadSceneAsync(sceneName, mode);
        m_asyncOperation.completed += OnAsyncComplete;
        m_asyncOperation.allowSceneActivation = false;
        while (!m_asyncOperation.isDone) {
            float progress = m_asyncOperation.progress;
            if (progress >= 0.9f) {
                m_asyncOperation.allowSceneActivation = true;
                m_panelProgressbar.SetProgress(1.0f);
                m_panelProgressbar.SetText("Loading 100%...");
            } else {
                m_panelProgressbar.SetProgress(progress);
                m_panelProgressbar.SetText("Loading " + Mathf.FloorToInt(progress * 100) + "%...");
            }
            yield return null;
        }
    }

    private void OnAsyncComplete(AsyncOperation asyncOperation) {
        gameObject.SetActive(false);
        m_panelProgressbar.gameObject.SetActive(false);
        m_asyncOperation.completed -= OnAsyncComplete;
        m_asyncOperation = null;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode) {
        if (isActiveSceneOnLoaded) {
            SceneManager.SetActiveScene(scene);
        }
        gameObject.SetActive(false);
        m_panelProgressbar.gameObject.SetActive(false);
    }

    private void OnActiveSceneChanged(Scene current, Scene next) {
        m_cameraStart.gameObject.SetActive(next.buildIndex == m_cameraStart.gameObject.scene.buildIndex);
    }

    private void OnDisable() {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        SceneManager.activeSceneChanged -= OnActiveSceneChanged;
    }

    private void OnDestroy() {
        if (m_asyncOperation != null) {
            m_asyncOperation.completed -= OnAsyncComplete;
        }
    }

}