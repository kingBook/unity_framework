using DG.Tweening;
using UnityEngine;

/// <summary>
/// 碎片
/// </summary>
public class FracturePart : MonoBehaviour {

    public Vector3 gravity = new Vector3(0f, -9.81f, 0f);


    private Rigidbody m_rigidBody;
    private float m_localScaleXOnAwake;
    private bool m_isInited;

    public bool isExplosioning { get; private set; }


    public void ApplyExplosionForce(float explosionForce, Vector3 explosionPosition, float explosionRadius, float scaleDuration = -1f, float endLocalScaleMultiplier = -1f) {
        if (isExplosioning) return;
        isExplosioning = true;

        Init();

        m_rigidBody.constraints = RigidbodyConstraints.None;

        var meshCollider = GetComponent<MeshCollider>();
        if (meshCollider) {
            meshCollider.isTrigger = true;
        }

        m_rigidBody.isKinematic = false;
        m_rigidBody.AddExplosionForce(explosionForce, explosionPosition, explosionRadius);

        if (scaleDuration > 0f) {
            transform.DOScale(m_localScaleXOnAwake * endLocalScaleMultiplier, scaleDuration);
        }
    }

    private void Init() {
        if (m_isInited) return;
        m_isInited = true;

        m_rigidBody = GetComponent<Rigidbody>();
        if (m_rigidBody == null) {
            m_rigidBody = gameObject.AddComponent<Rigidbody>();
        }

        m_rigidBody.constraints = RigidbodyConstraints.FreezeRotation;
        m_rigidBody.isKinematic = true;
        m_rigidBody.useGravity = false;

        m_localScaleXOnAwake = transform.localScale.x;
    }

    private void Awake() {
        Init();
    }

    private void FixedUpdate() {
        if (isExplosioning) {
            m_rigidBody.AddForce(gravity, ForceMode.Acceleration);
        }
    }
}
