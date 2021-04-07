using UnityEngine;

/// <summary>单摆 </summary>
public class SimplePendulum : MonoBehaviour {

    public float gravity=-9.81f;
    public Vector2 originPosition;
    public Transform targetTransform;

    private float m_deltaTime;
    private float m_currentAngle;

    private bool m_isPause;
    private bool m_isInited;
    private Vector2 m_velocity;

    /// <summary> 摆长 </summary>
    public float length { get; set; }
    /// <summary> 返回单摆当前欧拉角度 </summary>
    public float eulerAngle => LocalAngleToWorld(m_currentAngle) * Mathf.Rad2Deg;
    public Vector2 velocity => m_velocity;
    /// <summary> 角速度 </summary>
    public float w { get; private set; }

    public void Init () {
        m_deltaTime = Time.fixedDeltaTime;

        Vector2 relative=(Vector2)targetTransform.position-originPosition;

        length = relative.magnitude;

        float angle=Mathf.Atan2(relative.y,relative.x);
        m_currentAngle = WorldAngleToLocal(angle);

        w = 0f;
    }

    private void FixedUpdate () {
        if (m_isPause) return;

        if (!m_isInited) {
            m_isInited = true;
            Init();
        }

        float k1,k2,k3,k4;
        float l1,l2,l3,l4;
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
        float newX=originPosition.x+Mathf.Sin(m_currentAngle)*length;
        float newY=originPosition.y-Mathf.Cos(m_currentAngle)*length;

        Vector3 targetPos=targetTransform.position;

        m_velocity.x = newX - targetPos.x;
        m_velocity.y = newY - targetPos.y;

        targetPos.x = newX;
        targetPos.y = newY;
        targetTransform.position = targetPos;
    }

    private float WorldAngleToLocal (float worldAngle) {
        return worldAngle + Mathf.PI * 0.5f;
    }

    private float LocalAngleToWorld (float localAngle) {
        return localAngle - Mathf.PI * 0.5f;
    }

    public void SetPause (bool value) {
        m_isPause = value;
    }

    /// <summary>
    /// 立即反向运动（用于摆动到某个角度时立即反向摆动）
    /// </summary>
    /// <param name="damping">区间[0,1]，角速度的阻尼</param>
    public void Reverse (float damping) {
        damping = Mathf.Clamp01(damping);
        w = -w * 0.5f;
    }

    private void OnDisable () {
        m_isInited = false;
    }
}