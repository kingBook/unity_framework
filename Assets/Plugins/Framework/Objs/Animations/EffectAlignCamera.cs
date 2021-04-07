using UnityEngine;
using System.Collections;

/// <summary>
/// 特效对齐相机 <br/>
/// 使用此类需要如下结构: <br/>
/// Effect->EffectChild（EffectChild是Effect子级，父级(Effect)用于控制位置，子级(EffectChild)用于控制翻转/旋转） <br/>
/// 在子级(EffectChild)添加此脚本组件 <br/>
/// 注意：<br/>
/// * 调整显示前后关系时不要设置z轴的位置，使用此脚本的 zValue 进行设置 <br/>
/// * 当需要重新设置 EffectChild 的旋转时，在设置旋转后需要调用 RecordInitEulerAngles() 函数重新记录初始的欧拉角
/// </summary>
public class EffectAlignCamera : MonoBehaviour {

    [Tooltip("用于设置显示前后的z值，值越小越靠前，当小于相机的z位置或最近裁剪面时将不显示")]
    public float zValue = -1f;
    private float m_distanceCamera;

    private Camera m_camera;
    private Transform m_transform;
    private bool m_isInited;
    private Vector3 m_eulerAnglesInit;

    /// <summary> 记录初始的欧拉角 </summary>
    public void RecordInitEulerAngles () {
        m_eulerAnglesInit = m_transform.eulerAngles;
    }

    private void Init () {
        m_isInited = true;

        RecordInitEulerAngles();

        m_distanceCamera = Vector3.Distance(m_camera.transform.position, m_transform.position) + zValue;
    }

    private void Start () {
        m_transform = transform;
        m_camera = Camera.main;
    }

    private void Update () {
        if (!m_isInited) Init();

        AlignRotation();
        AlignOrderAndScale();

    }

    private void AlignRotation () {
        m_transform.eulerAngles = m_camera.transform.eulerAngles + m_eulerAnglesInit;
    }

    private void AlignOrderAndScale () {
        Vector3 cameraOrigin = m_camera.transform.position;
        Vector3 position = m_transform.position;
        float distanceOld = Vector3.Distance(cameraOrigin,position);
        //相机原点向当前位置的射线
        Ray ray = new Ray(cameraOrigin,position-cameraOrigin);
        //计算显示前后位置
        Vector3 newPosition = ray.GetPoint(m_distanceCamera);
        m_transform.position = newPosition;
        //透视相机时计算缩放
        if (!m_camera.orthographic) {
            float distanceNew = Vector3.Distance(cameraOrigin,newPosition);
            m_transform.localScale *= distanceNew / distanceOld;
        }
    }

}
