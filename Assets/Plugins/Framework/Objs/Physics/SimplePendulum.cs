#pragma warning disable 0649

using UnityEngine;

/// <summary> 单摆 </summary>
public class SimplePendulum : MonoBehaviour {

    public float gravity = -9.81f;

    [SerializeField, Tooltip("单摆的原点，如果使用代码设置时，请保留该值为 null，并使用 SetOrigin 函数设置")]
    private Transform m_origin;
    [Tooltip("摆动的目标")]
    public Transform target;
    [Tooltip("如果为 true，将旋转目标保持红色轴的方向与原点到目标的方向一致")]
    public bool isUpdateTargetRotation;

    private float m_deltaTime;
    private float m_currentAngle;

    private bool m_isPause;
    private bool m_isInited;
    private Vector2 m_velocity;
    private Vector2 m_originPosition;

    /// <summary> 摆长 </summary>
    public float length { get; set; }
    /// <summary> 返回单摆当前欧拉角度 </summary>
    public float eulerAngle => LocalAngleToWorld(m_currentAngle) * Mathf.Rad2Deg;
    /// <summary> 目标当前位置相对于上一帧位置的产生的位移速度 </summary>
    public Vector2 velocity => m_velocity;
    /// <summary> 角速度 </summary>
    public float w { get; private set; }

    public void Init () {
        m_deltaTime = Time.fixedDeltaTime;

        Vector2 relative = (Vector2)target.position - m_originPosition;

        length = relative.magnitude;

        float angle = Mathf.Atan2(relative.y, relative.x);
        m_currentAngle = WorldAngleToLocal(angle);

        w = 0f;
    }

    /// <summary> 设置原点的位置（注意只在 <see cref="m_origin"/> 为 null时，才能使用此方法） </summary>
    public void SetOrigin (Vector2 value) {
        if (m_origin) {
            Debug.LogError("m_origin 非 null 时，不能使用此方法，请直接设置 m_origin 的位置");
            return;
        }
        m_originPosition = value;
    }

    /// <summary>
    /// 用角度设置目标初始位置。
    /// 调用此函数需要保证原点（<see cref="m_originPosition"/>）已正确设置，
    /// 此函数以摆动目标到原点的距离作为摆长，再根据指定的角度计算目标的初始位置。
    /// </summary>
    /// <param name="angleRadian"></param>
    public void SetTargetInitialPositionWithAngle (float angleRadian) {
        float distance = Vector2.Distance(target.position, m_originPosition);

        Vector3 targetPosition = target.position;
        targetPosition.x = m_originPosition.x + Mathf.Cos(angleRadian) * distance;
        targetPosition.y = m_originPosition.y + Mathf.Sin(angleRadian) * distance;
        target.position = targetPosition;
    }

    /// <summary>
    /// 设置暂停/取消暂停
    /// </summary>
    /// <param name="value"></param>
    public void SetPause (bool value) {
        m_isPause = value;
    }

    /// <summary>
    /// 立即反向运动（用于摆动到某个角度时立即反向摆动）
    /// </summary>
    /// <param name="damping">区间[0,1]，角速度的阻尼</param>
    public void Reverse (float damping) {
        damping = Mathf.Clamp01(damping);
        w = -w * damping;
    }

    private float WorldAngleToLocal (float worldAngle) {
        return worldAngle + Mathf.PI * 0.5f;
    }

    private float LocalAngleToWorld (float localAngle) {
        return localAngle - Mathf.PI * 0.5f;
    }

    private void UpdateTargetRotation () {
        Vector2 relative = (Vector2)target.position - m_originPosition;

        Vector3 eulerAngles = target.eulerAngles;
        eulerAngles.z = Mathf.Atan2(relative.y, relative.x) * Mathf.Rad2Deg;
        target.eulerAngles = eulerAngles;
    }

#if UNITY_EDITOR
    private void Reset () {
        if (!target) {
            target = transform;
        }
    }
#endif

    private void Awake () {
        if (m_origin) {
            m_originPosition = m_origin.position;
        }
    }

    private void FixedUpdate () {
        if (m_isPause) return;

        if (!m_isInited) {
            m_isInited = true;
            Init();
        }

        // 实时更新 m_origin
        if (m_origin) {
            m_originPosition = m_origin.position;
        }

        if (isUpdateTargetRotation) {
            UpdateTargetRotation();
        }

        float k1, k2, k3, k4;
        float l1, l2, l3, l4;
        {
            k1 = w;
            l1 = (gravity / length) * Mathf.Sin(m_currentAngle);

            k2 = w + m_deltaTime * l1 / 2f;
            l2 = (gravity / length) * Mathf.Sin(m_currentAngle + m_deltaTime * k1 / 2f);

            k3 = w + m_deltaTime * l2 / 2f;
            l3 = (gravity / length) * Mathf.Sin(m_currentAngle + m_deltaTime * k2 / 2f);

            k4 = w + m_deltaTime * l3;
            l4 = (gravity / length) * Mathf.Sin(m_currentAngle * m_deltaTime * k3);

            m_currentAngle += m_deltaTime * (k1 + 2f * k2 + 2f * k3 + k4) / (6f/*2f*Math.PI*/);
            w += m_deltaTime * (l1 + 2f * l2 + 2f * l3 + l4) / (6f/*2f*Math.PI*/);
        }
        float newX = m_originPosition.x + Mathf.Sin(m_currentAngle) * length;
        float newY = m_originPosition.y - Mathf.Cos(m_currentAngle) * length;

        Vector3 targetPos = target.position;

        m_velocity.x = newX - targetPos.x;
        m_velocity.y = newY - targetPos.y;

        targetPos.x = newX;
        targetPos.y = newY;
        target.position = targetPos;
    }

    private void OnDisable () {
        m_isInited = false;
    }
}