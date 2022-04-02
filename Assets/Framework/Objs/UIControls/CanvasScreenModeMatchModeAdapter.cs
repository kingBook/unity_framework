using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 在 OnEnable 时重新设置 CanvasScaler 的 matchWidthOrHeight ，OnDisable 时再还原
/// </summary>
public class CanvasScreenModeMatchModeAdapter : MonoBehaviour {

    public float matchWidthOrHeight = 0f;

    private CanvasScaler m_canvasScaler;
    private float m_matchWidthOrHeightRecord;

    private void Awake() {
        m_canvasScaler = GetComponentInParent<CanvasScaler>();
    }

    private void OnEnable() {
        m_matchWidthOrHeightRecord = m_canvasScaler.matchWidthOrHeight;
        m_canvasScaler.matchWidthOrHeight = matchWidthOrHeight;
    }

    private void OnDisable() {
        if(this && m_canvasScaler) {
            m_canvasScaler.matchWidthOrHeight = m_matchWidthOrHeightRecord;
        }
    }
}
