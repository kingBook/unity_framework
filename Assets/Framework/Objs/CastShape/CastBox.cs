using System.Collections;
using UnityEngine;

public class CastBox : CastShapeBase {

    public Vector3 center;
    public Vector3 size = Vector3.one;

#if UNITY_EDITOR
    protected override void Reset () {
        center = Vector3.zero;
        size = Vector3.one;
    }

    protected override void OnDrawGizmosSelected () {
        Color gizmosColorRecord = Gizmos.color;
        Gizmos.color = m_gizomsColor;


        Matrix4x4 localToWorldMatrix = transform.localToWorldMatrix;

        Gizmos.DrawWireCube(transform.position, localToWorldMatrix * size);

        Gizmos.color = gizmosColorRecord;
    }
#endif
}
