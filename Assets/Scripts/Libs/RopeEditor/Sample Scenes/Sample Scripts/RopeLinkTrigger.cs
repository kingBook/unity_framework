using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(UltimateRope))]
public class RopeLinkTrigger : MonoBehaviour {

    [Min(0)] public int triggerStartCount = 1;
    [Min(0)] public int triggerEndCount = 1;

    private UltimateRope m_ultimateRope;
    private List<ConfigurableJoint> m_tempJoints = new List<ConfigurableJoint>();

    private void CheckCutting () {
        if (!m_ultimateRope.gameObject.activeInHierarchy) return;

        for (int i = 0, nodeCount = m_ultimateRope.RopeNodes.Count; i < nodeCount; i++) {
            UltimateRope.RopeNode node = m_ultimateRope.RopeNodes[i];
            int linkJointCount = node.linkJoints.Length;
            for (int j = 0; j < linkJointCount; j++) {
                if (m_tempJoints.IndexOf(node.linkJoints[j]) < 0) {
                    m_tempJoints.Add(node.linkJoints[j]);
                }
            }
        }

        for (int i = 0, len = m_tempJoints.Count; i < len; i++) {
            ConfigurableJoint joint = m_tempJoints[i];
            if (joint) {
                Collider collider = m_tempJoints[i].GetComponent<Collider>();
                if (collider) {
                    collider.isTrigger = i < triggerStartCount || i >= len - triggerEndCount;
                }
            }
        }
        m_tempJoints.Clear();
    }


    private void Awake () {
        m_ultimateRope = GetComponent<UltimateRope>();
    }

    private void Update () {
        CheckCutting();
    }

}
