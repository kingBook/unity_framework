using UnityEngine;

/// <summary>旋转动画</summary>
public class AnimRotate : MonoBehaviour {

    [SerializeField] private RangeVector3 m_eulersVelocity = new RangeVector3(new Vector3(0, 5, 0), new Vector3(0, 10, 0));

    private Transform m_transform;
    private Vector3 m_eulers;

    private void Awake() {
        m_transform = GetComponent<Transform>();

        m_eulers = new Vector3(Random.Range(m_eulersVelocity.min.x, m_eulersVelocity.max.x),
                               Random.Range(m_eulersVelocity.min.y, m_eulersVelocity.max.y),
                               Random.Range(m_eulersVelocity.min.z, m_eulersVelocity.max.z));
    }

    private void FixedUpdate() {
        m_transform.Rotate(m_eulers);
    }
}