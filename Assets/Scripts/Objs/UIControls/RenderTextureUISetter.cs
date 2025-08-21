using System.Collections;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 将创建一个 RenderTexture，并将它设置到 Camera.targetTexture 和 RawImage.texture，且管理它的"丢失"重建、内存释放
/// </summary>
public class RenderTextureUISetter : MonoBehaviour {

    public RawImage rawImage;
    public Camera cam;

    [Space]
    public Vector2Int renderTextureSize = new Vector2Int(512, 512);
    public int depthBuffer = 16;
    public RenderTextureFormat renderTextureFormat = RenderTextureFormat.ARGB32;

    private RenderTexture m_renderTexture;

    private void OnEnable() {
        m_renderTexture = RenderTexture.GetTemporary(renderTextureSize.x, renderTextureSize.y, depthBuffer, renderTextureFormat);

        rawImage.texture = m_renderTexture;
        cam.targetTexture = m_renderTexture;
        //必须为 CameraClearFlags.SolidColor或CameraClearFlags.Depth，CameraClearFlags.Nothing 时会不显示
        cam.clearFlags = CameraClearFlags.SolidColor;

        //CameraClearFlags.SolidColor时会有背景色，需要设置背景色透明
        Color color = cam.backgroundColor;
        color.a = 0f;
        cam.backgroundColor = color;
    }

    private void Update() {
        if (!m_renderTexture.IsCreated()) {
            m_renderTexture.Create();
        }
    }

    private void OnDisable() {
        m_renderTexture.Release();
    }
}
