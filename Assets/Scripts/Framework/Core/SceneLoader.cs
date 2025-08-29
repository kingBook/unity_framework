#pragma warning disable 0649

using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.Events;

/// <summary>
/// 场景加载器
/// </summary>
public sealed class SceneLoader : MonoBehaviour {


    [Tooltip("进度条"), SerializeField]
    private PanelProgressbar _panelProgressbar;

    /// <summary> main 场景的主相机 </summary>
    [SerializeField] private Camera _cameraMain;


    private void Awake() {
        // 场景加载完成回调
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    /// <summary>
    /// 异步加载场景，将显示进度条
    /// （注意：LoadSceneMode.Additive 模式加载场景时，被加载场景里的对象不能在 Awake() 或 OnEnable() 里访问 Camera.main, 会访问到 Main 场景的主相机）
    /// </summary>
    /// <param name="sceneName"> 场景在BuildSettings窗口的路径或名称 </param>
    /// <param name="progressBarVisible"> 显示加载进度页面 </param>
    /// <param name="onComplete"> 加载完成回调 </param>
    public void LoadAsync(string sceneName, bool progressBarVisible = true, UnityAction onComplete = null) {
        // 开始异步加载场景协程
        StartCoroutine(LoadAsync(sceneName, LoadSceneMode.Additive, progressBarVisible, onComplete));
    }

    /// <summary>
    /// 异步加载场景
    /// （注意：LoadSceneMode.Additive 模式加载场景时，被加载场景里的对象不能在 Awake() 或 OnEnable() 里访问 Camera.main, 会访问到 Main 场景的主相机）
    /// </summary>
    /// <param name="sceneName"> 场景在BuildSettings窗口的路径或名称 </param>
    /// <param name="mode"> 加载场景的模式 </param>
    /// <param name="progressBarVisible"> 显示加载进度页面 </param>
    /// <param name="onComplete"> 加载完成回调 </param>
    /// <returns></returns>
    public IEnumerator LoadAsync(string sceneName, LoadSceneMode mode, bool progressBarVisible, UnityAction onComplete) {
        // 显示进度条 0%
        if (progressBarVisible) {
            _panelProgressbar.gameObject.SetActive(true);
            _panelProgressbar.SetProgress(0.0f);
        }

        // 加载场景异步操作
        var asyncOperation = SceneManager.LoadSceneAsync(sceneName, mode);

        // 加载完成回调
        asyncOperation.completed += (asyncOp) => {
            _panelProgressbar.gameObject.SetActive(false);
        };

        // 一旦准备好，允许场景被激活
        asyncOperation.allowSceneActivation = false;

        // 异步操作未完成
        while (!asyncOperation.isDone) {
            // 加载进度
            if (progressBarVisible) _panelProgressbar.SetProgress(asyncOperation.progress);
            yield return null;
        }

        // 异步操作完成
        if (progressBarVisible) {
            _panelProgressbar.SetProgress(1.0f);
            _panelProgressbar.gameObject.SetActive(false);
        }

        // 设置为激活场景
        SceneManager.SetActiveScene(SceneManager.GetSceneByName(sceneName));

        // 加载完成回调
        onComplete?.Invoke();
    }

    /// <summary> 场景加载完成回调 </summary>
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode) {
        // 已加载场景非 main 场景时，吊销 main 场景的主相机
        _cameraMain.gameObject.SetActive(scene.buildIndex == _cameraMain.gameObject.scene.buildIndex);
    }

    private void OnDestroy() {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

}