using UnityEngine;
using System.Collections;
/// <summary>
/// Yoyo 缩放
/// </summary>
public class YoyoScale:BaseMonoBehaviour{

	public RangeVector3 localScale=new RangeVector3(Vector3.one, Vector3.one*1.5f);

	[Range(1,100)]
	public int speed=5;

	private int m_deg;
	private Transform m_transform;

	protected override void Awake() {
		base.Awake();
		m_transform=transform;
	}

	protected override void FixedUpdate2(){
		base.FixedUpdate2();
		//m_deg:[0,180]
		m_deg=(m_deg+speed)%180;
		//[0,1]
		float t=Mathf.Sin(m_deg*Mathf.Deg2Rad);
		
		m_transform.localScale=Vector3.Lerp(localScale.min,localScale.max,t);
	}

}
