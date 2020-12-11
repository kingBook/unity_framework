using System.Collections;
using UnityEngine;
/// <summary>
/// 将当前位置绑定到目标 Transform
/// </summary>
public class BindPositionMotion:BaseMonoBehaviour{
	
	[Tooltip("绑定的目标")] public Transform target;
	[Tooltip("当前与目标的相对偏移量")] public Vector3 offset;

	protected override void Start(){
		base.Start();
		SyncPosition();
	}

	protected override void FixedUpdate2(){
		base.FixedUpdate2();
		SyncPosition();
	}

	private void SyncPosition(){
		Vector3 targetPosition=target.position;
		targetPosition+=offset;
		transform.position=targetPosition;
	}

}
