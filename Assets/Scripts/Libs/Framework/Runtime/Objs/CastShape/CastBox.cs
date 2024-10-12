using System.Collections;
using UnityEngine;

public class CastBox : CastShapeBase {

    public Vector3 center;
    public Vector3 size = Vector3.one;

    public Vector3 centerWorld => transform.localToWorldMatrix.MultiplyPoint(center);
    public Vector3 halfExtentsWorld {
        get {
            Vector3 halfExtents = size * 0.5f;
            halfExtents.Scale(transform.lossyScale);
            return halfExtents;
        }
    }

#if UNITY_EDITOR
    protected override void Reset() {
        center = Vector3.zero;
        size = Vector3.one;
    }

    protected override void OnDrawGizmosSelected() {
        Matrix4x4 gizmosMatrixRecord = Gizmos.matrix;
        Color gizmosColorRecord = Gizmos.color;
        Gizmos.color = m_gizomsColor;
        Gizmos.matrix = transform.localToWorldMatrix;
        Gizmos.DrawWireCube(center, size);
        Gizmos.color = gizmosColorRecord;
        Gizmos.matrix = gizmosMatrixRecord;
    }
#endif
}
