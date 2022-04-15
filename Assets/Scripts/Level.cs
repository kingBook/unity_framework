#pragma warning disable 0649

using UnityEngine;
using System.Collections;
using UnityEngine.Rendering;

/// <summary>
/// 关卡类
/// <br>管理关卡内的对象。</br>
/// </summary>
public class Level : MonoBehaviour {

    [SerializeField] private Material m_skyboxMaterial;

    protected Game m_game;


    public EffectsFactory effectsFactory { get; private set; }

    public CanvasLevel canvasLevel { get; private set; }


    public void Victory() {

    }

    public void Failure() {

    }

    private void InitRenderSettings() {
        RenderSettings.skybox = m_skyboxMaterial; // 天空盒

        RenderSettings.ambientMode = AmbientMode.Flat; // 平面环境光照
        RenderSettings.ambientLight = new Color(0.5988371f, 0.5701693f, 0.5574281f); // 环境光颜色

        RenderSettings.fog = false; // 开启雾效果
        ColorUtility.TryParseHtmlString("#041F45", out Color fogColor);
        RenderSettings.fogColor = fogColor; // 雾颜色
        RenderSettings.fogDensity = 0.002f; // 雾密度
    }

    private void Awake() {
        effectsFactory = GetComponent<EffectsFactory>();
        canvasLevel = GameObject.Find("CanvasLevel").GetComponent<CanvasLevel>();
    }

    private void Start() {
        m_game = App.instance.GetGame<Game>();
        m_game.SetCurrentLevel(this);

        //InitRenderSettings();
    }

    private void OnDestroy() {
        if (m_game) {
            m_game.SetCurrentLevel(null);
        }
    }

}
