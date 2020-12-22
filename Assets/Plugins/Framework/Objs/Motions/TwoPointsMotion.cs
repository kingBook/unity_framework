using UnityEngine;
using System.Collections;
/// <summary>
/// 两点间运动
/// </summary>
public class TwoPointsMotion:MonoBehaviour{
	
	[Tooltip("运动的目标点")]
	public Transform targetTransform;
	[Tooltip("运动速度")]
	public float speed=1f;
	[Tooltip("当到达一个点时，需要同步转向的另一个 TwoPointsMotion")]
	public TwoPointsMotion syncOther;

	private Vector3 m_positionRecord;
	private Vector3 m_currentGotoTarget;

	private void Start(){
		m_positionRecord=transform.position;
		m_currentGotoTarget=targetTransform.position;
	}

	private void Update(){
		if(GotoTarget(transform.position,m_currentGotoTarget,Time.deltaTime*speed)){
			SetReverseGotoTarget();
			if(syncOther){
				syncOther.SetReverseGotoTarget();
			}
		}
	}

	private bool GotoTarget(Vector3 current,Vector3 target,float maxDistanceDelta){
		transform.position=Vector3.MoveTowards(current,target,maxDistanceDelta);
		float distance=Vector3.Distance(current,target);
		return distance<=0.01f;
	}

	public void SetReverseGotoTarget(){
		Vector3 position=transform.position;
		float distance1=Vector3.Distance(position,m_positionRecord);
		float distance2=Vector3.Distance(position,targetTransform.position);
		if(distance1>distance2){
			m_currentGotoTarget=m_positionRecord;
		}else{
			m_currentGotoTarget=targetTransform.position;
		}
	}
}
