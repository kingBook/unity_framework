using System.Collections;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 管理 Camera.targetTexture 或 RawImage.texture 中已存在的 RenderTexture 的"丢失"重建、内存释放
/// </summary>
public class RenderTextureRecreateAndGC : MonoBehaviour {

    [SerializeField] private RenderTexture m_renderTexture;

    private bool m_inited;

#if UNITY_EDITOR
    private void Reset () {
        Init();
    }
#endif

    private void Init () {
        if (!m_renderTexture) {
            Camera camera = GetComponent<Camera>();
            if (camera) {
                m_renderTexture = camera.targetTexture;
            }
        }

        if (!m_renderTexture) {
            RawImage rawImage = GetComponent<RawImage>();
            if (rawImage) {
                m_renderTexture = rawImage.texture as RenderTexture;
            }
        }

        if (!m_renderTexture) {
            Debug.LogError($"Error：在 {gameObject.name} 对象的 Camera.targetTexture 或 RawImage.texture 中，没有找到 RenderTexture，或没有 Camera 或 RawImage 组件");
        }
    }

    private void Update () {
        if (!m_inited) {
            Init();
            m_inited = true;
        }
        if (!m_renderTexture.IsCreated()) {
            m_renderTexture.Create();
        }
    }

    private void OnDisable () {
        m_inited = false;
    }

    private void OnDestroy () {
        m_inited = false;
        m_renderTexture.Release();
    }

}
