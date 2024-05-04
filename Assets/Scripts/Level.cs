#pragma warning disable 0649

using System;
using UnityEngine;

/// <summary>
/// 关卡类
/// <br>管理关卡内的对象。</br>
/// </summary>
public sealed class Level : MonoBehaviour {

    [SerializeField] private Material m_skyboxMaterial;

    private Game m_game;

    public FsmLevel fsmLevel { get; private set; }
    public EffectsFactory effectsFactory { get; private set; }
    public CanvasLevel canvasLevel { get; private set; }
    public Material skyboxMaterial => m_skyboxMaterial;


    private void Awake() {
        effectsFactory = GetComponent<EffectsFactory>();
        canvasLevel = GameObject.Find("CanvasLevel").GetComponent<CanvasLevel>();
    }

    private void Start() {
        m_game = App.instance.GetGame<Game>();
        m_game.SetCurrentLevel(this);
        
        fsmLevel = new FsmLevel(StateLevelStart.instance, this);
    }

    private void FixedUpdate() {
        fsmLevel.FixedUpdate();
    }

    private void Update() {
        fsmLevel.Update();
    }

    private void LateUpdate() {
        fsmLevel.LateUpdate();
    }

    private void OnDestroy() {
        if (m_game) {
            m_game.SetCurrentLevel(null);
        }
        fsmLevel.OnDestroy();
    }

}