using System.Collections;
using UnityEngine;

/// <summary>
/// 椭圆运动
/// </summary>
public class EllipseMotion:BaseMonoBehaviour{
	
	
	[Tooltip("椭圆在X轴上的半轴")] public float a=3;
	[Tooltip("椭圆在Y轴上的半轴")] public float b=4;
	[Tooltip("椭圆的中心")] public Transform center;
	[Tooltip("速度(无方向)")] public float speed=1;
	[Tooltip("是否逆时针运动")] public bool isCCW;


	private float m_deg;

	protected override void FixedUpdate2(){
		base.FixedUpdate2();
		Vector3 position=transform.position;
		int sign=isCCW?1:-1;
		m_deg=(m_deg+sign*speed)%360f;
		position.x=center.position.x+Mathf.Cos(m_deg*Mathf.Deg2Rad)*a;
		position.y=center.position.y+Mathf.Sin(m_deg*Mathf.Deg2Rad)*b;
		transform.position=position;
	}

}
