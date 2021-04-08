using UnityEngine;
/// <summary>
/// 目标展示纹理渲染器
/// </summary>
public class PoseTextureRenderer : MonoBehaviour {

    /// <summary>
    /// 用于拍照的相机
    /// </summary>
    [Tooltip("用于拍照的相机")]
    public Camera targetCamera;

    /// <summary>
    /// 相机所拍摄的游戏对象
    /// </summary>
    [Tooltip("相机所拍摄的游戏对象")]
    public GameObject target;

    /// <summary>
    /// 用于定义渲染包围盒大小的碰撞器，当为None时，将自动取对象及子级的所有Renderer组件定义渲染包围大小
    /// </summary>
    [Tooltip("用于定义渲染包围盒大小的碰撞器，当为None时，将自动取对象及子级的所有Renderer组件定义渲染包围大小")]
    public Collider targetBoundsCollider;

    /// <summary>
    /// 用于渲染目标的纹理
    /// </summary>
    [Tooltip("用于渲染目标的纹理")]
    public RenderTexture targetTexture;

    [Tooltip("在Start函数是否设置相机看向目标")]
    public bool isLookToTargetOnStart = false;

    private bool m_targetCameraActiveRecord;
    private bool m_targetRendererActiveRecord;

    private void Start () {
        if (isLookToTargetOnStart) {
            SetCameraLookToTarget();
        }
    }

    /// <summary>
    /// 将对目标拍照渲染
    /// </summary>
    /// <returns></returns>
    public void Render () {
        //记录激活状态
        m_targetCameraActiveRecord = targetCamera.gameObject.activeSelf;
        m_targetRendererActiveRecord = target.activeSelf;
        //激活
        targetCamera.gameObject.SetActive(true);
        target.SetActive(true);
        //设置相机看向目标，并渲染
        SetCameraLookToTarget();
        targetCamera.targetTexture = targetTexture;
        targetCamera.Render();
        targetCamera.targetTexture = null;
        //恢复激活
        targetCamera.gameObject.SetActive(m_targetCameraActiveRecord);
        target.SetActive(m_targetRendererActiveRecord);
    }

    /// <summary>
    /// 设置相机的参数看向目标包围盒
    /// </summary>
    public void SetCameraLookToTarget () {
        //相机旋转朝向目标对象
        Bounds bounds = targetBoundsCollider ? targetBoundsCollider.bounds : GetGameObjectBounds(target);
        Vector3 boundsCenter = bounds.center;
        targetCamera.transform.LookAt(boundsCenter);
        //包围盒角点
        Vector3[] points = FuncUtil.GetBoundsCorners(boundsCenter, bounds.extents);
        //所有角点投射到平面
        Vector3 planeNormal = boundsCenter - targetCamera.transform.position;
        FuncUtil.WorldPointsToPlane(points, points.Length, planeNormal);
        //平面中心
        Vector3 planeCenter = Vector3.ProjectOnPlane(boundsCenter, planeNormal);
        //取平面上各个点与平面中心的最大距离作为相机的视野矩形框大小
        float halfHeight = GetMaxDistanceToPlaneCenter(points, points.Length, planeCenter);
        //相机与包围盒中心的距离(世界坐标为单位)
        float distance = Vector3.Distance(boundsCenter, targetCamera.transform.position);
        //得到视野大小
        targetCamera.fieldOfView = Mathf.Atan2(halfHeight, distance) * Mathf.Rad2Deg * 2;
    }

    /// <summary>
    /// 返回平面上各个点与平面中心的最大距离
    /// </summary>
    /// <param name="points">平面上的各个点</param>
    /// <param name="pointCount">点数量</param>
    /// <param name="planeCenter">平面中心</param>
    /// <returns></returns>
    private float GetMaxDistanceToPlaneCenter (Vector3[] points, int pointCount, Vector3 planeCenter) {
        float maxDistance = float.MinValue;
        for (int i = 0; i < pointCount; i++) {
            var vertex = points[i];
            float distance = Vector3.Distance(vertex, planeCenter);
            if (distance > maxDistance) maxDistance = distance;
        }
        return maxDistance;
    }

    private Bounds GetGameObjectBounds (GameObject gameObj) {
        Renderer[] renderers = gameObj.GetComponentsInChildren<Renderer>();
        int len = renderers.Length;
        if (len <= 0) return new Bounds();
        Bounds bounds = renderers[0].bounds;
        if (len > 1) {
            Vector3 min = bounds.min;
            Vector3 max = bounds.max;
            for (int i = 1; i < len; i++) {
                Bounds tempBounds = renderers[i].bounds;
                min.x = Mathf.Min(tempBounds.min.x, min.x);
                min.y = Mathf.Min(tempBounds.min.y, min.y);
                min.z = Mathf.Min(tempBounds.min.z, min.z);
                max.x = Mathf.Max(tempBounds.max.x, max.x);
                max.y = Mathf.Max(tempBounds.max.y, max.y);
                max.z = Mathf.Max(tempBounds.max.z, max.z);
            }
            bounds.SetMinMax(min, max);
        }
        return bounds;
    }
}