using System.Collections;
using UnityEngine;
/// <summary>
/// 游戏对象工具类
/// </summary>
public static class GameObjectUtil {

    /// <summary>
    /// 返回一个游戏对象的包围盒(将跳过计算未激活的Renderer)
    /// </summary>
    /// <param name="gameObject">游戏对象</param>
    /// <param name="filterChildren">需要过滤的子级对象</param>
    /// <returns></returns>
    public static Bounds GetGameObjectRenderersBounds(GameObject gameObject, params GameObject[] filterChildren) {
        int filterChildCount = filterChildren.Length;
        Renderer[] filterChildRenderers = new Renderer[filterChildCount];
        for (int i = 0; i < filterChildCount; i++) {
            filterChildRenderers[i] = filterChildren[i].GetComponent<Renderer>();
        }

        Bounds bounds = new Bounds();
        Renderer rootRenderer = gameObject.GetComponent<Renderer>();
        if (rootRenderer != null && rootRenderer.enabled) {
            bounds = rootRenderer.bounds;
        }
        Renderer[] subRenderers = gameObject.GetComponentsInChildren<Renderer>();
        int j = subRenderers.Length;
        while (--j >= 0) {
            Renderer renderer = subRenderers[j];
            if (!renderer.enabled) continue;
            if (System.Array.IndexOf(filterChildRenderers, renderer) > -1) continue;
            if (bounds.min.magnitude == 0f && bounds.max.magnitude == 0f) {
                bounds = renderer.bounds;
            } else {
                bounds.Encapsulate(renderer.bounds);
            }
        }
        return bounds;
    }

    /// <summary>
    /// 吊销所有子级
    /// </summary>
    /// <param name="parent"></param>
    public static void DeactiveChildren(GameObject parent) {
        TransformUtil.DeactiveChildren(parent.transform);
    }


    /// <summary>
    /// 激活一个子级并吊销其他子级
    /// </summary>
    /// <param name="parent"></param>
    /// <param name="childSiblingIndex"></param>
    public static void ActiveChildAndDeactiveOtherChildren(GameObject parent, int childSiblingIndex) {
        TransformUtil.ActiveChildAndDeactiveOtherChildren(parent.transform, childSiblingIndex);
    }
    
}