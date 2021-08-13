#pragma warning disable 0649

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 鼠标或手指按住屏幕滑动的拖尾线
/// </summary>
public class TrailLine : MonoBehaviour {

    [Tooltip("决定显示的深度")]
    public float linePositionZ = 10f;

    private TrailRenderer m_trailRenderer;
    private Transform m_transform;

    private void Awake () {
        m_transform = transform;
        m_trailRenderer = GetComponent<TrailRenderer>();
    }

    private void Update () {
        if (Input.GetMouseButtonDown(0)) {
            // 停止划线，防止坐标瞬移导致出现一个拖尾
            m_trailRenderer.emitting = false;
            var mousPos = Input.mousePosition;
            m_transform.position = Camera.main.ScreenToWorldPoint(new Vector3(mousPos.x, mousPos.y, linePositionZ));
            return;
        }

        if (Input.GetMouseButton(0)) {
            m_trailRenderer.emitting = true;
            var mousPos = Input.mousePosition;
            m_transform.position = Camera.main.ScreenToWorldPoint(new Vector3(mousPos.x, mousPos.y, linePositionZ));
        }
    }
}
