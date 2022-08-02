using System.Collections;
using UnityEngine;

/// <summary>
/// 左右撞墙检测器
/// </summary>
[RequireComponent(typeof(Rigidbody2D))]
public class CollideWallDetector2D : MonoBehaviour {

    [Range(0f, 1f), Tooltip("左右是否撞墙的法线阀值，绝对值大于此值表示撞墙")] public float normalThresholdX = 0.7f;

    private readonly ContactPoint2D[] m_contacts = new ContactPoint2D[64];
    private Rigidbody2D m_rigidbody2D;

    /// <summary>
    /// -1：碰右墙；1：碰左墙
    /// </summary>
    public int collideSign { get; private set; }

    private void Awake() {
        m_rigidbody2D = GetComponent<Rigidbody2D>();
    }

    private void FixedUpdate() {
        int collideSignValue = 0;
        int count = m_rigidbody2D.GetContacts(m_contacts);
        for (int i = 0; i < count; i++) {
            var contact = m_contacts[i];
            if (!contact.enabled) continue;
            if (Mathf.Abs(contact.normal.x) > normalThresholdX) {
                collideSignValue = (int)Mathf.Sign(contact.normal.x);
                break;
            }
        }
        collideSign = collideSignValue;
    }
}
