using UnityEngine;

#pragma warning disable 0649

/// <summary> 俯视的车(正前向方朝x轴) </summary>
public class TopViewCar:BaseMonoBehaviour{
	public float maxSpeed=15;				//移动的最大速度
	public float maxWheelRotation=45;		//轮子的最大旋转角
	public float length=100;				//车身长
	public bool isResetWheelRotation;		//在未控制车轮旋转的情况下，是否重置轮子的旋转角与车身一致
	[Range(0.0001f,1)]                  	
	public float resetWheelRate=0.5f;		//重置轮子旋转角的速率
	[Range(0,1)]                        	
	public float speedDamp=0.5f;			//速度惯性衰减[0,1]
	public Transform body;					//车的Transfrom
	public Transform[] frontWheels;			//前轮Transform列表(可选的)
	public Transform[] rearWheels;			//前轮Transform列表(可选的)
											
	private float m_speed;					//移动速度
	private float m_wheelRotation;			//轮子的旋转角<角度>
	private bool m_isDriveing;				//是否正在驱动中...
	private bool m_isResetWheeling;			//是否正在重置轮子旋转角...
	
	
	private Vector3 m_velocity=Vector3.zero;//当前的移动速度
	public Vector3 velocity=>m_velocity;

	protected override void FixedUpdate2(){
		base.FixedUpdate2();
		//轮子的旋转角<弧度>
		float wheelRadian=m_wheelRotation*Mathf.Deg2Rad;
		//当前移动速度在当前轮子方向上的分量（x：用于计算车向前的位移 y：用于计算车的旋转）。
		Vector2 speedDelta=new Vector2(m_speed*Mathf.Cos(wheelRadian),m_speed*Mathf.Sin(wheelRadian));
		//旋转车身
		float rotationDeltaY=Mathf.Atan2(speedDelta.y,length)*Mathf.Rad2Deg;
		Vector3 bodyEulerAngles=body.eulerAngles;
		bodyEulerAngles.y+=rotationDeltaY;
		body.eulerAngles=bodyEulerAngles;
		//按车身的当前的Y旋转角进行移动
		float bodyRadianY=bodyEulerAngles.y*Mathf.Deg2Rad;
		m_velocity.x=speedDelta.x*Mathf.Cos(bodyRadianY)*Time.deltaTime;
		m_velocity.z=-speedDelta.x*Mathf.Sin(bodyRadianY)*Time.deltaTime;
		body.Translate(m_velocity,Space.World);
		//旋转前轮
		int i=frontWheels.Length;
		while(--i>=0){
			Transform frontWheel=frontWheels[i];
			Vector3 frontWheelEulerAngles=frontWheel.eulerAngles;
			frontWheelEulerAngles.y=bodyEulerAngles.y+m_wheelRotation;
			frontWheel.eulerAngles=frontWheelEulerAngles;
		}
		//旋转后轮
		i=rearWheels.Length;
		while(--i>=0){
			Transform rearWheel=rearWheels[i];
			Vector3 rearWheelEulerAngles=rearWheel.eulerAngles;
			rearWheelEulerAngles.y=bodyEulerAngles.y;//与车身相同
			rearWheel.eulerAngles=rearWheelEulerAngles;
		}
		//重置轮子旋转角
		if(m_isResetWheeling){
			m_wheelRotation*=1-resetWheelRate;//速率越大恢复越快
		}
		//速度摩擦衰减
		if(!m_isDriveing){
			m_speed*=speedDamp;
		}
	}
	
	/// <summary>
	/// 驱动，按下方向控制键时每帧调用
	/// </summary>
	/// <param name="vNormalized">单位化的移动方向(y：表示前后方向，x:表示左右旋转方向)</param>
	/// <param name="moveDelta">移动速度增量</param>
	/// <param name="wheelRotateDelta">轮子旋转增量</param>
	public void Drive(Vector2 vNormalized,float moveDelta=1f,float wheelRotateDelta=1f){
		m_isDriveing=true;
		m_isResetWheeling=false;
		float speedDirection=vNormalized.y>0?1:vNormalized.y<0?-1:0;
		m_speed=Mathf.Clamp(m_speed+speedDirection*moveDelta,-maxSpeed,maxSpeed);

		float wheelAngleDirection=vNormalized.x>0?1:vNormalized.x<0?-1:0;
		m_wheelRotation=Mathf.Clamp(m_wheelRotation+wheelAngleDirection*wheelRotateDelta,-maxWheelRotation,maxWheelRotation);
		if(isResetWheelRotation){
			m_isResetWheeling=wheelAngleDirection==0;
		}
	}
	
	/// <summary> 停止驱动 </summary>
	public void StopDrive(){
		m_isDriveing=false;
		m_isResetWheeling=isResetWheelRotation;
	}
}
