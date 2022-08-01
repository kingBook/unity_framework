using System.Collections;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class SpriteRendererSwitcher : MonoBehaviour {

    [SerializeField] private SpriteRenderer m_spriteRenderer;
    [SerializeField, Tooltip("切换的SpriteFrame列表，长度为:2")] private Sprite[] m_spriteFrames;

    private void Awake() {
        m_spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void AutoSwap() {
        if (m_spriteRenderer.sprite == m_spriteFrames[0]) {
            SwapTo(1);
        } else {
            SwapTo(0);
        }
    }

    public void SwapTo(int spriteFrameId) {
        m_spriteRenderer.sprite = m_spriteFrames[spriteFrameId];
    }

#if UNITY_EDITOR
    private void Reset() {
        m_spriteRenderer = GetComponent<SpriteRenderer>();
    }
#endif
}
