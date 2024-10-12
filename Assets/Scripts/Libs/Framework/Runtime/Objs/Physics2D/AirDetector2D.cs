using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 空中检测器
/// </summary>
[RequireComponent(typeof(Rigidbody2D))]
public class AirDetector2D : MonoBehaviour {

    [Range(0f, 1f), Tooltip("碰撞平面的法线阀值y，大于此值表示在地面")] public float normalThresholdY = 0.5f;

    /// <summary>
    /// 改变在空中变量时，回调函数格式：<code> void OnChangedHandler(bool isInAir) </code>
    /// </summary>
    public event System.Action<bool> onChangedEvent;

    private readonly ContactPoint2D[] m_contacts = new ContactPoint2D[64];
    private Rigidbody2D m_rigidbody2D;
    private bool m_isInAir = true;

    public bool isInAir => m_isInAir;


    private void Awake() {
        m_rigidbody2D = GetComponent<Rigidbody2D>();
    }

    private void FixedUpdate() {
        bool isAir = true;
        int count = m_rigidbody2D.GetContacts(m_contacts);
        for (int i = 0; i < count; i++) {
            var contact = m_contacts[i];
            if (!contact.enabled) continue;
            if (contact.normal.y > normalThresholdY) {
                isAir = false;
                break;
            }
        }

        if (m_isInAir != isAir) {
            onChangedEvent?.Invoke(isAir);
            m_isInAir = isAir;
        }

    }


}
