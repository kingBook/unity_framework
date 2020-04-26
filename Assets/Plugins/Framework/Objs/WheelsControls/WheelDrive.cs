using UnityEngine;
#pragma warning disable 0649
//轮子的外观必须是WheelCollider组件对象的子对象,并且在0索引。
public class WheelDrive:BaseMonoBehaviour{
	[System.Serializable]
	public enum DriveType{
		RearWheelDrive,//后轮驱动
		FrontWheelDrive,//前轮驱动
		AllWheelDrive//所有轮驱动
	}

	[Tooltip("Maximum steering angle of the wheels.")]
	public float maxAngle=30f;//车轮的最大转向角。

	[Tooltip("Maximum torque applied to the driving wheels.")]
	public float maxTorque=1000f;//施加在驱动轮上的最大扭矩。

	[Tooltip("Maximum brake torque applied to the driving wheels.")]
	public float brakeTorque=30000f;//施加在驱动轮上的最大制动(刹车)扭矩。

	[Tooltip("The vehicle's speed when the physics engine can use different amount of sub-steps (in m/s).")]
	public float criticalSpeed=5f;//子步进算法的speedThreshold（用于WheelCollider.ConfigureVehicleSubsteps方法）。

	[Tooltip("Simulation sub-steps when the speed is below critical.")]
	public int stepsBelow=5;//当车辆速度低于 speedThreshold 时，模拟子步骤的数量（用于WheelCollider.ConfigureVehicleSubsteps方法）。

	[Tooltip("Simulation sub-steps when the speed is above critical.")]
	public int stepsAbove=3;//当车辆速度高于 speedThreshold 时，模拟子步骤的数量（用于WheelCollider.ConfigureVehicleSubsteps方法）。

	[Tooltip("The vehicle's drive type: rear-wheels drive, front-wheels drive or all-wheels drive.")]
	public DriveType driveType;//车辆的驱动类型：RearWheelDrive（后轮驱动）、FrontWheelDrive（前轮驱动）、AllWheelDrive（所有轮驱动）。
	
	[Space,SerializeField] private WheelCollider[] m_frontWheels;//前轮列表
	
	[Space,SerializeField] private WheelCollider[] m_rearWheels;//后轮列表
	
	private float _steerAngleNormalized=0;
	private float _motorTorqueNormalized=0;
	private bool _isBrake;

	protected override void FixedUpdate2(){
		base.FixedUpdate2();
		//配置车辆子步进参数
		//每次进行固定更新时，车辆模拟将该固定增量时间拆分为较小的子步骤，并计算每个较小增量的悬架和轮胎力。然后，汇总所有计算得出的力和扭矩，将它们整合到一起并应用于车身。
		//利用该函数，您可以自定义在高于和低于速度阈值时模拟将执行的子步骤数。
		//对于每辆汽车，调用该函数一次即可，因为它实际上是向车辆而不是向某个车轮设置参数。
		m_frontWheels[0].ConfigureVehicleSubsteps(criticalSpeed,stepsBelow,stepsAbove);
		//
		float steerAngle=_steerAngleNormalized*maxAngle;
		float motorTorque=_motorTorqueNormalized*maxTorque;
		float brakeTorqueValue=_isBrake?brakeTorque:0;
		////////////////////////////设置前轮//////////////////////////
		int i=m_frontWheels.Length;
		while(--i>=0){
			WheelCollider wheel=m_frontWheels[i];
			wheel.steerAngle=steerAngle;//前轮设置转向角
			if(driveType!=DriveType.RearWheelDrive){
				wheel.motorTorque=motorTorque;
			}
			UpdateWheelSkin(wheel);
		}
		////////////////////////////设置后轮//////////////////////////
		i=m_rearWheels.Length;
		while(--i>=0){
			WheelCollider wheel=m_rearWheels[i];
			wheel.brakeTorque=brakeTorqueValue;//后轮设置刹车扭矩
			if(driveType!=DriveType.FrontWheelDrive){
				wheel.motorTorque=motorTorque;
			}
			UpdateWheelSkin(wheel);
		}
	}
	
	/// <summary>更新车轮外观</summary>
	private void UpdateWheelSkin(WheelCollider wheel){
		wheel.GetWorldPose(out Vector3 p,out Quaternion q);
		Transform skinTransform=wheel.transform.GetChild(0);
		skinTransform.position=p;
		skinTransform.rotation=q;
	}
	
	/// <summary>设置刹车</summary>
	public void SetBrake(bool value){
		_isBrake=value;
	}
	
	/// <summary>设置单位化的转向角度[-1,1]</summary>
	public void SetSteerAngleNormalized(float value){
		_steerAngleNormalized=value;
	}
	
	/// <summary>设置单位化的车轮轴电机扭矩[-1,1]</summary>
	public void SetMotorTorqueNormalized(float value){
		_motorTorqueNormalized=value;
	}
	
	/// <summary>当前轮轴转速（以每分钟转数为单位）</summary>
	public float rpm=>m_frontWheels[0].rpm;
	
	/// <summary>指示车轮当前是否与某物发生碰撞</summary>
	public bool isGrounded{
		get{
			int i=m_frontWheels.Length;
			while(--i>=0){
				if(m_frontWheels[i].isGrounded){
					return true;
				}
			}
			i=m_rearWheels.Length;
			while(--i>=0){
				if(m_rearWheels[i].isGrounded){
					return true;
				}
			}
			return false;
		}
	}
}