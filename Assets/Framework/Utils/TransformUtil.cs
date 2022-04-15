using UnityEngine;
using System.Collections;

/// <summary>
/// Transform 工具类
/// </summary>
public static class TransformUtil {

    /// <summary>
    /// 销毁所有的子级对象
    /// </summary>
    /// <param name="ignoreChildren"> 忽略销毁的子对象，不设置时传递 null </param>
    /// <param name="transform"> 父级 Transform </param>
    /// <param name="ignoreInActive"> 是否忽略不激活的对象 </param>
    /// <param name="isImmediate"> 是否立即销毁 </param>
    public static void DestroyAllChildren(Transform transform, Transform[] ignoreChildren, bool ignoreInActive = false, bool isImmediate = false) {
        int i = transform.childCount;
        while (--i >= 0) {
            Transform child = transform.GetChild(i);

            if (ignoreInActive && !child.gameObject.activeSelf) {
                continue;
            }

            if (ignoreChildren != null && System.Array.IndexOf(ignoreChildren, child) > -1) {
                continue;
            }

            if (isImmediate) {
                Object.DestroyImmediate(child.gameObject);
            } else {
                Object.Destroy(child.gameObject);
            }
        }
    }

    /// <summary>
    /// 返回一个Transform组件的子节点列表
    /// </summary>
    /// <param name="transform"></param>
    /// <returns></returns>
    public static Transform[] GetTransformChildren(Transform transform) {
        int childCount = transform.childCount;
        Transform[] children = new Transform[childCount];
        for (int i = 0; i < childCount; i++) {
            children[i] = transform.GetChild(i);
        }
        return children;
    }

    /// <summary> 转换一个 Transform 数组到顶点数组 </summary>
    public static Vector3[] CovertTransformsToVertices(Transform[] transforms, Space space = Space.World) {
        int len = transforms.Length;
        Vector3[] vertices = new Vector3[len];
        for (int i = 0; i < len; i++) {
            if (space == Space.World) vertices[i] = transforms[i].position;
            else vertices[i] = transforms[i].localPosition;
        }
        return vertices;
    }
}
