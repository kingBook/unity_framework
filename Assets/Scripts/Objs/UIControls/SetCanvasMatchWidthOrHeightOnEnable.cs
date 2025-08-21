using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class SetCanvasMatchWidthOrHeightOnEnable : MonoBehaviour {

    [Range(0f, 1f)] public float matchWidthOrHeight = 0.5f;

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
        m_canvasScaler.matchWidthOrHeight = m_matchWidthOrHeightRecord;
    }

}
