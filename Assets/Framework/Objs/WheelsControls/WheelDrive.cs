using UnityEngine;
using UnityEngine.Serialization;
#pragma warning disable 0649

// 轮子的外观必须是 WheelCollider 组件对象的子对象，并且在 0 索引。
public class WheelDrive : MonoBehaviour {

    [System.Serializable]
    public enum DriveType {
        /// <summary> 后轮驱动 </summary>
        RearWheelDrive,
        /// <summary> 前轮驱动 </summary>
        FrontWheelDrive,
        /// <summary> 所有轮驱动 </summary>
        AllWheelDrive
    }

    [Tooltip("车轮的最大转向角")]
    public float maxAngle = 30f;
    [Tooltip("施加在驱动轮上的最大扭矩")]
    public float maxTorque = 1000f;
    [Tooltip("施加在驱动轮上的最大制动(刹车)扭矩"), FormerlySerializedAs("brakeTorque")]
    public float maxBrakeTorque = 30000f;
    [Tooltip("当物理引擎可以使用不同数量的 sub-steps（m/s为单位）时的车速（用于 WheelCollider.ConfigureVehicleSubsteps方法）")]
    public float speedThreshold = 5f;
    [Tooltip("当速度低于临界值时，模拟 sub-steps（用于WheelCollider.ConfigureVehicleSubsteps方法）")]
    public int stepsBelowThreshold = 5;
    [Tooltip("当速度超过临界值时，模拟 sub-steps（用于WheelCollider.ConfigureVehicleSubsteps方法）")]
    public int stepsAboveThreshold = 3;
    [Tooltip("车辆的驱动类型：RearWheelDrive（后轮驱动）、FrontWheelDrive（前轮驱动）、AllWheelDrive（所有轮驱动）")]
    public DriveType driveType;
    [Tooltip("true 时， maxBrakeTorque 和 SetBrakeTorqueInterpolation 方法无效，只能调用 SetCustomBrakeTorque 方法设置刹车扭矩")]
    public bool useCustomBrakeTorque;
    [Tooltip("true 时， maxTorque 和 SetMotorTorqueNormalized 方法无效，只能调用 SetCustomMotorTorque 方法设置驱动轮扭矩")]
    public bool useCustomMotorTorque;
    [Space]
    public WheelCollider[] frontWheels;
    public WheelCollider[] rearWheels;

    private float m_steerAngleNormalized;
    private float m_motorTorqueNormalized;
    private float m_brakeTorqueInterpolation;
    private float m_customBrakeTorque;
    private float m_customMotorTorque;

    /// <summary> 当前的转向角 [-1,1] </summary>
    public float steerAngleNormalized => m_steerAngleNormalized;
    /// <summary> 驱动轮上的最大扭矩 [-1,1] </summary>
    public float motorTorqueNormalized => m_motorTorqueNormalized;
    /// <summary> 刹车扭矩插值 [0,1] </summary>
    public float brakeTorqueInterpolation => m_brakeTorqueInterpolation;
    /// <summary> 当前轮轴转速（以每分钟转数为单位）</summary>
    public float rpm => frontWheels[0].rpm;
    /// <summary> 自定义方式的刹车扭矩 </summary>
    public float customBrakeTorque => m_customBrakeTorque;
    /// <summary> 自定义方式的车轮轴电机扭矩 </summary>
    public float customMotorTorque => m_customMotorTorque;
    /// <summary> 指示车轮当前是否与某物发生碰撞 </summary>
    public bool isGrounded {
        get {
            int i = frontWheels.Length;
            while (--i >= 0) {
                if (frontWheels[i].isGrounded) {
                    return true;
                }
            }
            i = rearWheels.Length;
            while (--i >= 0) {
                if (rearWheels[i].isGrounded) {
                    return true;
                }
            }
            return false;
        }
    }

    /// <summary> 自定义方式的刹车扭矩, <see cref="useCustomBrakeTorque"/> 为 true 时才有效 </summary>
    public void SetCustomBrakeTorque(float value) {
        m_customBrakeTorque = value;
    }

    /// <summary> 自定义方式的车轮轴电机扭矩, <see cref="useCustomMotorTorque"/> 为 true 时才有效 </summary>
    public void SetCustomMotorTorque(float value) {
        m_customMotorTorque = value;
    }

    /// <summary> 设置刹车扭矩插值 [0,1]，<see cref="useCustomBrakeTorque"/> 为 false 时才有效 </summary>
    public void SetBrakeTorqueInterpolation(float t) {
        t = Mathf.Clamp01(t);
        m_brakeTorqueInterpolation = t;
    }

    /// <summary> 设置单位化的车轮轴电机扭矩 [-1,1], <see cref="useCustomMotorTorque"/> 为 false 时才有效 </summary>
    public void SetMotorTorqueNormalized(float value) {
        m_motorTorqueNormalized = value;
    }

    /// <summary> 设置单位化的转向角度 [-1,1] </summary>
    public void SetSteerAngleNormalized(float value) {
        m_steerAngleNormalized = value;
    }

    /// <summary> 更新车轮外观 </summary>
    private void UpdateWheelSkin(WheelCollider wheel) {
        wheel.GetWorldPose(out Vector3 p, out Quaternion q);
        Transform skinTransform = wheel.transform.GetChild(0);
        skinTransform.position = p;
        skinTransform.rotation = q;
    }

    private void FixedUpdate() {
        // 配置车辆子步进参数
        // 每次进行固定更新时，车辆模拟将该固定增量时间拆分为较小的子步骤，并计算每个较小增量的悬架和轮胎力。然后，汇总所有计算得出的力和扭矩，将它们整合到一起并应用于车身。
        // 利用该函数，您可以自定义在高于和低于速度阈值时模拟将执行的子步骤数。
        // 对于每辆汽车，调用该函数一次即可，因为它实际上是向车辆而不是向某个车轮设置参数。
        frontWheels[0].ConfigureVehicleSubsteps(speedThreshold, stepsBelowThreshold, stepsAboveThreshold);
        //
        float steerAngle = m_steerAngleNormalized * maxAngle;
        float motorTorque = m_motorTorqueNormalized * maxTorque;
        if (useCustomMotorTorque) {
            motorTorque = customMotorTorque;
        }
        float brakeTorqueValue = maxBrakeTorque * m_brakeTorqueInterpolation;
        if (useCustomBrakeTorque) {
            brakeTorqueValue = customBrakeTorque;
        }
        ////////////////////////////设置前轮//////////////////////////
        int i = frontWheels.Length;
        while (--i >= 0) {
            WheelCollider wheel = frontWheels[i];
            wheel.steerAngle = steerAngle;//前轮设置转向角
            if (driveType != DriveType.RearWheelDrive) {
                wheel.motorTorque = motorTorque;
            }
            UpdateWheelSkin(wheel);
        }
        ////////////////////////////设置后轮//////////////////////////
        i = rearWheels.Length;
        while (--i >= 0) {
            WheelCollider wheel = rearWheels[i];
            wheel.brakeTorque = brakeTorqueValue; // 后轮设置刹车扭矩
            if (driveType != DriveType.FrontWheelDrive) {
                wheel.motorTorque = motorTorque;
            }
            UpdateWheelSkin(wheel);
        }
    }

}