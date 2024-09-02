#pragma warning disable 0649

using System;
using UnityEngine;

/// <summary>
/// 关卡类
/// <br>管理关卡内的对象。</br>
/// </summary>
public sealed class Level : MonoBehaviour {


    [SerializeField] private Material m_skyboxMaterial;


    public LevelFsm fsm { get; private set; }
    public EffectsFactory effectsFactory { get; private set; }
    public CanvasLevel canvasLevel { get; private set; }
    public Material skyboxMaterial => m_skyboxMaterial;


    private void Awake() {
        effectsFactory = GetComponent<EffectsFactory>();
        canvasLevel = GameObject.Find("CanvasLevel").GetComponent<CanvasLevel>();
    }

    private void Start() {
        fsm = Fsm.Create<LevelFsm>(gameObject);
        fsm.Init(this);
        fsm.ChangeStateTo(nameof(StateLevelStart));
        
    }


}