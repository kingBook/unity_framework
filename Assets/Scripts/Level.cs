#pragma warning disable 0649

using System;
using UnityEngine;

/// <summary>
/// 关卡类
/// <br>管理关卡内的对象。</br>
/// </summary>
public sealed class Level : MonoBehaviour {

    public static Level instance { get; private set; }

    [SerializeField] private Material m_skyboxMaterial;


    public LevelFsm fsm { get; private set; }
    public EffectsFactory effectsFactory { get; private set; }
    public CanvasLevel canvasLevel { get; private set; }
    public Material skyboxMaterial => m_skyboxMaterial;


    private void Awake() {
        instance = this;
        effectsFactory = GetComponent<EffectsFactory>();
        canvasLevel = GameObject.Find("CanvasLevel").GetComponent<CanvasLevel>();
    }

    private void Start() {
        fsm = GameObjectUtil.AddNodeComponent<LevelFsm>(gameObject);
    }

    private void OnDestroy() {
        instance = null;
    }


}