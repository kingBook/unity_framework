#pragma warning disable 0649

using UnityEngine;
using System.Collections;

/// <summary> 两点间运动 </summary>
public class TwoPointsMotion : MonoBehaviour {

    [Tooltip("运动的起始点，None 时以当前位置为起始点（只读取在 Start() 时的位置）")] public Transform startTransform;
	[Tooltip("运动的目标点（只读取在 Start() 时的位置）")] public Transform targetTransform;
	[Tooltip("true 时，首次运动时是否朝目标点；false 时，首次运动时是否朝起始点")] public bool isFirstToTarget = true;
	[Tooltip("运动速度。")] public float speed = 1f;
	[Tooltip("到达一个点时，等待的时间(秒)"), Range(0f, 10f)] public float waitTime;
	[Tooltip("当到达一个点时，需要同步转向的其他 TwoPointsMotion")] public TwoPointsMotion[] syncReversesObjects;

	private Vector3 m_positionRecord;
	private Vector3 m_targetRecord;
	private Vector3 m_currentGotoTarget;
	private bool m_isWaiting;
	private Vector3 m_lastPosition;


	/// <summary>
	/// 移动事件，回调格式：void onMoveing (Vector3 velocity)
	/// </summary>
	public event System.Action<Vector3> onMoveingEvent;

	public Vector3 velocity { get; private set; }

	/// <summary>
	/// 设置反转运动
	/// </summary>
	public void SetReverseGotoTarget () {
		m_currentGotoTarget = (m_currentGotoTarget == m_positionRecord) ? m_targetRecord : m_positionRecord;
		//如果在等待中，则停止计时
		StopWaitTimer();
	}
	
	/// <summary>
	/// 添加自己到同步反转的对象
	/// </summary>
	/// <param name="syncObj"></param>
	private void AddSelfToSyncReversesObject (TwoPointsMotion syncObj) {
		bool isOtherListNull = syncObj.syncReversesObjects == null;
		if (isOtherListNull || System.Array.IndexOf(syncObj.syncReversesObjects, this) < 0) {
			if (isOtherListNull) {
				syncObj.syncReversesObjects = new TwoPointsMotion[] { this };
			} else {
				int len = syncObj.syncReversesObjects.Length;

				TwoPointsMotion[] list = new TwoPointsMotion[len + 1];
				System.Array.Copy(syncObj.syncReversesObjects, list, len);

				list[len] = this;

				syncObj.syncReversesObjects = list;
			}

		}
	}

	private bool GotoTarget (Vector3 current, Vector3 target, float maxDistanceDelta) {
		transform.position = Vector3.MoveTowards(current, target, maxDistanceDelta);
		float distance = Vector3.Distance(current, target);
		return distance <= 0.01f;
	}

	private void ReverseSyncObjects () {
		if (syncReversesObjects != null) {
			for (int i = 0, len = syncReversesObjects.Length; i < len; i++) {
				TwoPointsMotion syncObj = syncReversesObjects[i];
				if (syncObj) {
					syncObj.SetReverseGotoTarget();
				}
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
		//把自身添加到所有同步反转的对象的同步列表
		for (int i = 0, len = syncReversesObjects.Length; i < len; i++) {
			TwoPointsMotion syncObj = syncReversesObjects[i];
			if (syncObj) AddSelfToSyncReversesObject(syncObj);
		}
		
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

		//记录初始位置
		m_lastPosition = transform.position;
	}

	private void FixedUpdate () {
		if (m_isWaiting) return;

		Vector3 position = transform.position;

		if (GotoTarget(position, m_currentGotoTarget, Time.deltaTime * speed)) {
			if (waitTime > 0) {
				StartWaitTimer();
			} else {
				SetReverseGotoTarget();
				ReverseSyncObjects();
			}
		}

		//计算移动速度
		velocity = position - m_lastPosition;

		//调用移动事件
		if (velocity.magnitude > 0f) {
			onMoveingEvent?.Invoke(velocity);
		}

		//记录上一次位置，用于计算移动速度向量
		m_lastPosition = position;
	}
}
