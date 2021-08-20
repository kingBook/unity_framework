using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 绳子切割类
/// </summary>
public class RopeCutter : MonoBehaviour {

    public InputLine inputLine;
    public UltimateRope[] ropes;

    private Camera m_cameraMain;

    /// <summary>
    /// 切割绳子事件，切割到一条绳子发出此事件。格式: <code> void OnCuttingRope(UltimateRope rope, ConfigurableJoint joint) </code>
    /// </summary>
    public event System.Action<UltimateRope, ConfigurableJoint> onCuttingRopeEvent;

    private void CheckCutting () {
        // 不够两个点时不检测切割
        if (inputLine.points.Count < 2) return;

        // 初始化相机
        if (!m_cameraMain) {
            m_cameraMain = Camera.main;
            if (m_cameraMain.gameObject.scene != gameObject.scene) {
                Debug.LogError("错误：Camera.main 与绑定 RopeCutter 的对象不在同一场景，请确保 Camera.main 得到的相机是否正确，此问题会导致坐标转换出错");
            }
        }

        for (int i = 0, len = ropes.Length; i < len; i++) {
            UltimateRope rope = ropes[i];
			if (!rope) continue;
            if (!rope.gameObject.activeInHierarchy) continue;

            for (int j = 0, nodeCount = rope.RopeNodes.Count; j < nodeCount; j++) {
                UltimateRope.RopeNode node = rope.RopeNodes[j];
                int linkJointCount = node.linkJoints.Length;

                if (linkJointCount > 1) {
                    for (int k = 1; k < linkJointCount; k++) {
                        var prevLinkJoint = node.linkJoints[k - 1];
                        var curLinkJoint = node.linkJoints[k];

                        if (prevLinkJoint && curLinkJoint &&
                            prevLinkJoint.gameObject.activeInHierarchy && curLinkJoint.gameObject.activeInHierarchy &&
                            prevLinkJoint.transform.parent == rope.transform && curLinkJoint.transform.parent == rope.transform) {

                            Vector3 prevLinkJointScreenPoint = m_cameraMain.WorldToScreenPoint(prevLinkJoint.transform.position);
                            Vector3 curLinkJointScreenPoint = m_cameraMain.WorldToScreenPoint(curLinkJoint.transform.position);
                            prevLinkJointScreenPoint.z = 0f;
                            curLinkJointScreenPoint.z = 0f;

                            if (IsInsectInputLine(prevLinkJointScreenPoint, curLinkJointScreenPoint)) {
                                // 派发切割事件
                                onCuttingRopeEvent?.Invoke(rope, curLinkJoint);
                                // 销毁 ConfigurableJoint 绳子便断了
                                Destroy(curLinkJoint);
                            }
                        }
                    }
                }
            }
        }
    }

    private bool IsInsectInputLine (Vector3 prevNodeScreenPoint, Vector3 curNodeScreenPoint) {
        for (int i = 1, len = inputLine.points.Count; i < len; i++) {
            var prev = inputLine.points[i - 1];
            var current = inputLine.points[i];

            bool insect = GeomUtil.GetTwoLineSegmentsIntersection(prevNodeScreenPoint, curNodeScreenPoint, current, prev, out _, out _, out _);
            if (insect) {
                return true;
            }
        }
        return false;
    }

    private void OnDrawnLine (List<Vector3> points) {
        //CheckCutting();
    }

#if UNITY_EDITOR
    private void Reset () {
        if (!inputLine) {
            inputLine = GetComponent<InputLine>();
        }
    }
#endif

    private void Awake () {
        // 不能应用于 LinkJointBreakForce 和 LinkJointBreakTorque 都无穷大的绳子，切断会导致绳子的蒙皮网格出错
        for (int i = 0, len = ropes.Length; i < len; i++) {
            var rope = ropes[i];
            if (rope && rope.LinkJointBreakForce == Mathf.Infinity && rope.LinkJointBreakTorque == Mathf.Infinity) {
                Debug.LogError("不能应用于 LinkJointBreakForce 和 LinkJointBreakTorque 都无穷大的绳子, rope.gameObject.name:" + rope.gameObject.name);
            }
        }

        inputLine.onDrawnEvent += OnDrawnLine;
    }

    private void Update () {
        CheckCutting();
    }

    private void OnDestroy () {
        inputLine.onDrawnEvent -= OnDrawnLine;
    }

}
