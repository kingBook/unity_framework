#pragma warning disable 0649

using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestJumpToTarget : MonoBehaviour {

    [SerializeField] private Rigidbody m_playerBody;
    [SerializeField] private Transform m_target;

    private void Start() {

    }

    private void Update() {
        if (Input.GetMouseButtonDown(0)) {
            // 方法1：
            //m_playerBody.DOJump(m_target.position, 5f, 1, 2f);

            // 方法2：
            //m_playerBody.velocity = BallisticVel(m_target, m_playerBody.transform, 60);

            // 方法3：
            m_playerBody.velocity = GetBallisticVelocity(m_target.position, m_playerBody.transform, 60);
        }

    }

    private Vector3 BallisticVel(Transform target, Transform source, float angle) {
        Vector3 dir = target.position - source.position;  // get target direction
        float h = dir.y;  // get height difference
        dir.y = 0;  // retain only the horizontal direction
        float dist = dir.magnitude;  // get horizontal distance
        float a = angle * Mathf.Deg2Rad;  // convert angle to radians
        dir.y = dist * Mathf.Tan(a);  // set dir to the elevation angle
        dist += h / Mathf.Tan(a);  // correct for small height differences
                                   // calculate the velocity magnitude
        float vel = Mathf.Sqrt(dist * Physics.gravity.magnitude / Mathf.Sin(2 * a));
        return vel * dir.normalized;
    }

    private Vector3 GetBallisticVelocity(Vector3 targetPosition, Transform source, float angle) {
        Quaternion rotationRecord = source.rotation;

        // think of it as top-down view of vectors: 
        //   we don't care about the y-component(height) of the initial and target position.
        Vector3 projectileXZPos = new Vector3(source.position.x, 0.0f, source.position.z);
        Vector3 targetXZPos = new Vector3(targetPosition.x, 0.0f, targetPosition.z);

        // rotate the object to face the target
        source.LookAt(targetXZPos);

        // shorthands for the formula
        float R = Vector3.Distance(projectileXZPos, targetXZPos);
        float G = Physics.gravity.y;
        float tanAlpha = Mathf.Tan(angle * Mathf.Deg2Rad);
        float H = targetPosition.y - source.position.y;

        // calculate the local space components of the velocity 
        // required to land the projectile on the target object 
        float Vz = Mathf.Sqrt(G * R * R / (2.0f * (H - R * tanAlpha)));
        float Vy = tanAlpha * Vz;

        // create the velocity vector in local space and get it in global space
        Vector3 localVelocity = new Vector3(0f, Vy, Vz);
        Vector3 globalVelocity = source.TransformDirection(localVelocity);

        source.rotation = rotationRecord;

        return globalVelocity;
    }


}
