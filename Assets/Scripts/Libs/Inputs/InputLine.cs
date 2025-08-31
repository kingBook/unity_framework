using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// 计算在小段时间内手指/鼠标在屏幕上划时输入的屏幕坐标列表
/// </summary>
public class InputLine : MonoBehaviour {

    /// <summary>
    /// 画线完成事件（在画一条线完成后释放鼠标或手指时发出一次此事件）回调函数格式:  
    /// <code> void(List&lt;Vector3&gt; points) </code>
    /// </summary>
    public event System.Action<List<Vector3>> onDrawnEvent;

    [Tooltip("记录点的最小距离，距离小于此值时，将忽略")] public float minDistanceThreshold = 2.0f;
    [Tooltip("只记录在此段时间内位置点，超过此时间将清除 <秒>")] public float durationThreshold = 0.4f;
    [Tooltip("在编辑器模式时，是否显示已画的线")] public bool isDisplayLineOnEditorMode;

    private List<Vector3> m_points = new List<Vector3>();

    /// <summary>
    /// 记录每个位置点的时间，长度与 <see cref="m_points"/> 保持一致
    /// </summary>
    private List<float> m_times = new List<float>();

    private Material m_lineMaterial;

    /// <summary>
    /// 线的顶点列表（长度可能为 0）
    /// </summary>
    public List<Vector3> points => m_points;


    private void CheckInputAndAddPoints() {


        // 方法1：当有多个触摸点时，取所有点的中心
        if (Input.GetMouseButton(0)) {
            Vector3 inputScreenPoint = Input.mousePosition;
            // 方法2：此过程只个侦听第一个触摸点
            //if (InputUtil.GetPressScreenPoint(true, out Vector3 inputScreenPoint, out int inputFingerId)) {

            inputScreenPoint.z = 0f;

            float time = Time.time;

            // 清除超过时间的位置点
            int i = m_times.Count;
            while (--i >= 0) {
                if (time - m_times[i] > durationThreshold) {
                    m_times.RemoveRange(0, i + 1);
                    m_points.RemoveRange(0, i + 1);
                    break;
                }
            }

            // 将屏幕点添加到列表
            if (m_points.Count > 0) {
                float distance = Vector3.Distance(m_points.Last(), inputScreenPoint);
                if (distance >= minDistanceThreshold) {
                    m_points.Add(inputScreenPoint);
                    m_times.Add(time);
                }
            } else {
                m_points.Add(inputScreenPoint);
                m_times.Add(time);
            }
        } else {
            // 发出画线完成事件
            onDrawnEvent?.Invoke(m_points);

            // 没有在屏幕上按下时，清空列表
            m_points.Clear();
            m_times.Clear();
        }
    }

    private void CreateLineMaterial() {
        //如果材质球不存在
        if (!m_lineMaterial) {
            //用代码的方式实例一个材质球
            Shader shader = Shader.Find("Hidden/Internal-Colored");
            m_lineMaterial = new Material(shader);
            m_lineMaterial.hideFlags = HideFlags.HideAndDontSave;
            //设置参数
            m_lineMaterial.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
            m_lineMaterial.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
            //设置参数
            m_lineMaterial.SetInt("_Cull", (int)UnityEngine.Rendering.CullMode.Off);
            //设置参数
            m_lineMaterial.SetInt("_ZWrite", 0);
        }
    }

    private void Update() {
        CheckInputAndAddPoints();
    }

    private void OnRenderObject() {
        // 不显示时返回
        if (!isDisplayLineOnEditorMode) return;

        if (m_points.Count > 0) {
            //创建材质球
            CreateLineMaterial();

            //激活第一个着色器 Pass（本例中，它是唯一的 Pass）
            m_lineMaterial.SetPass(0);

            //渲染入栈  在Push——Pop之间写GL代码
            GL.PushMatrix();
            {
                //设置用屏幕坐标绘图
                GL.LoadPixelMatrix();

                // 开始画线  在Begin——End之间写画线方式
                //GL.LINES 画线
                GL.Begin(GL.LINES);
                {
                    GL.Color(Color.green);
                    for (int i = 0, len = m_points.Count; i < len; i++) {
                        GL.Vertex(m_points[i]);
                    }
                }
                GL.End();
                //渲染出栈
            }
            GL.PopMatrix();
        }
    }
}
