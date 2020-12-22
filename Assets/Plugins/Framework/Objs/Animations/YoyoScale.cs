using UnityEngine;
using System.Collections;
/// <summary>
/// Yoyo 缩放
/// </summary>
public class YoyoScale:MonoBehaviour{

	public RangeVector3 localScale=new RangeVector3(Vector3.one, Vector3.one*1.5f);

	[Range(1,100)]
	public int speed=5;

	[Range(0,180),Tooltip("用于三角函数计算的初始欧拉角，调整此值会改变在缩放值范围内初始的缩放值")]
	public int eulerAngle=0;

	private Transform m_transform;

	private void Awake() {
		m_transform=transform;
	}

	private void FixedUpdate(){
		//m_deg:[0,180]
		eulerAngle=(eulerAngle+speed)%180;
		//[0,1]
		float t=Mathf.Sin(eulerAngle*Mathf.Deg2Rad);
		
		m_transform.localScale=Vector3.Lerp(localScale.min,localScale.max,t);
	}

}
