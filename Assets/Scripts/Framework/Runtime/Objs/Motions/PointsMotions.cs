#pragma warning disable 0649
using UnityEngine;
using System.Collections;
/// <summary>
/// 多点运动
/// </summary>
public class PointsMotion : MonoBehaviour {

    [Tooltip("运动速度")]
    public float speed = 1f;
    [SerializeField, Tooltip("运动路径点列表")]
    private Transform[] m_points;
    [SerializeField, Tooltip("初始是否逆顺序运动，默认按路径点索引低到高运动")]
    private bool m_isReverseOnStart;
    [SerializeField, Tooltip("是否闭合路径运动， 不闭合时运动到最后一个点将反向运动")]
    private bool m_isClosed;
    [SerializeField, Tooltip("在Awake时暂停")]
    private bool m_pauseOnAwake;

    /// <summary> 目标点索引 </summary>
    private int m_targetPointIndex;
    /// <summary> 运动的方向符号 </summary>
    private int m_motionDirectionSign;

    /// <summary> 设置暂停 </summary>
    public bool isPause { get; set; }


    private void Awake() {
        isPause = m_pauseOnAwake;
    }

    private void Start() {
        m_motionDirectionSign = m_isReverseOnStart ? -1 : 1;

        m_targetPointIndex = GetClosestPointIndex();
    }

    private void FixedUpdate() {
        if (isPause) return;
        if (m_targetPointIndex < 0) return;
        if (GotoTarget(transform.position, m_points[m_targetPointIndex].position, speed * Time.deltaTime)) {
            m_targetPointIndex = GetNextPointIndex(m_targetPointIndex);
        }
    }

    private bool GotoTarget(Vector3 current, Vector3 target, float maxDistanceDelta) {
        transform.position = Vector3.MoveTowards(current, target, maxDistanceDelta);
        float distance = Vector3.Distance(current, target);
        return distance <= 0.01f;
    }

    /// <summary> 获取最近点的索引，如果点列表长度为0时，则返回-1。 </summary>
    private int GetClosestPointIndex() {
        int closestIndex = -1;
        Vector3[] vertices = TransformUtil.CovertTransformsToVertices(m_points);
        var polyLine = GeomUtil.GetClosestPolyLineToPoint(transform.position, vertices, m_isClosed);//获取距离当前点最近的边
        if (polyLine.startIndex > -1 && polyLine.endIndex > -1) {
            closestIndex = m_isReverseOnStart ? polyLine.startIndex : polyLine.endIndex;
        }
        return closestIndex;
    }

    private int GetNextPointIndex(int currentIndex) {
        int len = m_points.Length;
        int i = currentIndex;
        if (m_isClosed) {
            if (m_isReverseOnStart) {
                i = (i - 1 + len) % len;
            } else {
                i = (i + 1) % len;
            }
        } else {
            if (i >= len - 1 || i <= 0) m_motionDirectionSign = -m_motionDirectionSign;
            i = i + m_motionDirectionSign;
        }
        return i;
    }

}
