using System.Collections;
using UnityEngine;

/// <summary>
/// 生成一个新的 RenderTexture，并管理它的"丢失"重建、内存释放
/// </summary>
public class RenderTextureGetter : MonoBehaviour {

    public enum InitMethod {
        Awake,
        OnEnable,
        Start,
        Update,
        Manual
    }

    public Vector2Int renderTextureSize = new Vector2Int(512, 512);
    public int depthBuffer = 16;
    public RenderTextureFormat renderTextureFormat = RenderTextureFormat.ARGB32;

    [Space]
    public InitMethod initMethod = InitMethod.Awake;


    private RenderTexture m_renderTexture;
    private bool m_isInited;

    /// <summary>
    /// 获取生成的 RenderTexture
    /// </summary>
    public RenderTexture renderTexture {
        get {
            if (!m_isInited) Debug.LogError(nameof(RenderTextureGetter) + " 未初始化，不能访问 renderTexture 属性");
            return m_renderTexture;
        }
    }

    public void Init() {
        if (m_isInited) return;
        m_isInited = true;

        m_renderTexture = RenderTexture.GetTemporary(renderTextureSize.x, renderTextureSize.y, depthBuffer, renderTextureFormat);
    }

    public void Release() {
        if (!m_isInited) return;
        m_isInited = false;

        m_renderTexture.Release();
    }

    private void Awake() {
        if (initMethod == InitMethod.Awake) {
            Init();
        }
    }

    private void OnEnable() {
        if (initMethod == InitMethod.OnEnable) {
            Init();
        }
    }

    private void Start() {
        if (initMethod == InitMethod.Start) {
            Init();
        }
    }

    private void Update() {
        if (initMethod == InitMethod.Update) {
            Init();
        }

        if (m_isInited) {
            if (!m_renderTexture.IsCreated()) {
                m_renderTexture.Create();
            }
        }
    }

    private void OnDisable() {
        if (initMethod == InitMethod.OnEnable) {
            Release();
        }
    }

    private void OnDestroy() {
        if (initMethod != InitMethod.OnEnable) {
            Release();
        }
    }
}
