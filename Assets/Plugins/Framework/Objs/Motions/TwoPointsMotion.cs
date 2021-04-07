using UnityEngine;
using System.Collections;

/// <summary> 两点间运动 </summary>
public class TwoPointsMotion : MonoBehaviour {

    [Tooltip("运动的起始点，None 时以当前位置为起始点（只读取在 Start() 时的位置）")]  public Transform startTransform;
    [Tooltip("运动的目标点（只读取在 Start() 时的位置）")]                          public Transform targetTransform;
    [Tooltip("true 时，首次运动时是否朝目标点；false 时，首次运动时是否朝起始点")]     public bool isFirstToTarget=true;
    [Tooltip("运动速度。")]                                                      public float speed=1f;
    [Tooltip("到达一个点时，等待的时间(秒)"), Range(0f,10f)]                       public float waitTime;
    [Tooltip("当到达一个点时，需要同步转向的其他 TwoPointsMotion")]                 public TwoPointsMotion[] syncReversesObjects;

    private Vector3 m_positionRecord;
    private Vector3 m_targetRecord;
    private Vector3 m_currentGotoTarget;
    private bool m_isWaiting;

    /// <summary>
    /// 设置反转运动
    /// </summary>
    public void SetReverseGotoTarget () {
        m_currentGotoTarget = (m_currentGotoTarget == m_positionRecord) ? m_targetRecord : m_positionRecord;
        //如果在等待中，则停止计时
        StopWaitTimer();
    }

    private bool GotoTarget (Vector3 current, Vector3 target, float maxDistanceDelta) {
        transform.position = Vector3.MoveTowards(current, target, maxDistanceDelta);
        float distance=Vector3.Distance(current,target);
        return distance <= 0.01f;
    }

    private void ReverseSyncObjects () {
        if (syncReversesObjects != null) {
            for (int i = 0, len = syncReversesObjects.Length; i < len; i++) {
                syncReversesObjects[i].SetReverseGotoTarget();
            }
        }
    }

    private void StartWaitTimer () {
        Invoke(nameof(OnWaitTimeEnd), waitTime);
    }

    private void StopWaitTimer () {
        CancelInvoke(nameof(OnWaitTimeEnd));
    }

    private void OnWaitTimeEnd () {
        SetReverseGotoTarget();
        ReverseSyncObjects();
    }

    private void Start () {
        //记录起始点
        if (startTransform) {
            m_positionRecord = startTransform.position;
        } else {
            m_positionRecord = transform.position;
        }

        //记录目标点
        m_targetRecord = targetTransform.position;

        //设置首次运动的目标
        if (isFirstToTarget) {
            m_currentGotoTarget = m_targetRecord;
        } else {
            m_currentGotoTarget = m_positionRecord;
        }
    }

    private void FixedUpdate () {
        if (m_isWaiting) return;

        if (GotoTarget(transform.position, m_currentGotoTarget, Time.deltaTime * speed)) {
            if (waitTime > 0) {
                StartWaitTimer();
            } else {
                SetReverseGotoTarget();
                ReverseSyncObjects();
            }
        }
    }
}
