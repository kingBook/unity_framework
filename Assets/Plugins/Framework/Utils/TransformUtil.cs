using UnityEngine;
using System.Collections;
/// <summary>
/// Transform工具类
/// </summary>
public static class TransformUtil {

    /// <summary>
    /// 返回一个Transform组件的子节点列表
    /// </summary>
    /// <param name="transform"></param>
    /// <returns></returns>
    public static Transform[] GetTransformChildren (Transform transform) {
        int childCount = transform.childCount;
        Transform[] children = new Transform[childCount];
        for (int i = 0; i < childCount; i++) {
            children[i] = transform.GetChild(i);
        }
        return children;
    }

    /// <summary> 转换一个 Transform 数组到顶点数组 </summary>
    public static Vector3[] CovertTransformsToVertices (Transform[] transforms, Space space = Space.World) {
        int len = transforms.Length;
        Vector3[] vertices = new Vector3[len];
        for (int i = 0; i < len; i++) {
            if (space == Space.World) vertices[i] = transforms[i].position;
            else vertices[i] = transforms[i].localPosition;
        }
        return vertices;
    }
}
