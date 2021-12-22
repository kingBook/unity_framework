using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestTouching : MonoBehaviour {

    public Collider collider1;
    public Collider collider2;

    public Transform point0;
    public Transform point1;

    public static readonly Collider[] TempColliders = new Collider[32];

    // Start is called before the first frame update
    void Start () {
        
    }

    // Update is called once per frame
    void Update () {
        Debug.Log(IsTouching(collider1, collider2));
    }

    public bool IsTouching (Collider collider1, Collider collider2) {
        int count = 0;
        Transform transform1 = collider1.transform;
        if (collider1 is BoxCollider) {
            BoxCollider boxCollider = (BoxCollider)collider1;
            Vector3 center = transform1.TransformPoint(boxCollider.center);
            Vector3 halfExtents = boxCollider.size * 0.5f;
            halfExtents.Scale(transform1.lossyScale);
            Quaternion orientation = transform1.rotation;
            count = Physics.OverlapBoxNonAlloc(center, halfExtents, TempColliders, orientation);
        } else if (collider1 is SphereCollider) {
            SphereCollider sphereCollider = (SphereCollider)collider1;
            Vector3 position = transform1.TransformPoint(sphereCollider.center);
            float radius = sphereCollider.radius*Mathf.Max(transform1.lossyScale.x, transform1.lossyScale.y, transform1.lossyScale.z);
            count = Physics.OverlapSphereNonAlloc(position, radius, TempColliders);
        } else if (collider1 is CapsuleCollider) {
            CapsuleCollider capsuleCollider = (CapsuleCollider)collider1;

            Vector3 direction = Vector3.up;
            switch (capsuleCollider.direction) {
                case 0: // X-Axis
                    direction = Vector3.right;
                    break;
                case 1: // Y-Axis
                    direction = Vector3.up;
                    break;
                case 2: // Z-Axis
                    direction = Vector3.forward;
                    break;
            }

            Vector3 localPoint0 = capsuleCollider.center - direction * (capsuleCollider.height * 0.5f- capsuleCollider.radius);
            Vector3 localPoint1 = capsuleCollider.center + direction * (capsuleCollider.height * 0.5f- capsuleCollider.radius);

            Vector3 point0 = transform1.TransformPoint(localPoint0);
            Vector3 point1 = transform1.TransformPoint(localPoint1);

            this.point0.position = point0;
            this.point1.position = point1;

            float radius = transform1.TransformVector(new Vector3(capsuleCollider.radius, 0f, 0f)).x;

            count = Physics.OverlapCapsuleNonAlloc(point0, point1, radius, TempColliders);
        } else {

        }

        for (int i = 0; i < count; i++) {
            if (TempColliders[i] == collider2) {
                return true;
            }
        }
        return false;
    }
}
