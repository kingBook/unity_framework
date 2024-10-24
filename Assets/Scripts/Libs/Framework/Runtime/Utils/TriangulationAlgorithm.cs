﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 三角化算法
/// </summary>
public class TriangulationAlgorithm {

    public static readonly float Epsilon = 1e-7f;

    private static bool FloatLess(float value, float other) {
        return (other - value) > Epsilon;
    }

    private static bool FloatGreat(float value, float other) {
        return (value - other) > Epsilon;
    }

    private static bool FloatEqual(float value, float other) {
        return Mathf.Abs(value - other) < Epsilon;
    }

    private static bool Vector3Equal(Vector3 a, Vector3 b) {
        return FloatEqual(a.x, b.x) && FloatEqual(a.y, b.y) && FloatEqual(a.z, b.z);
    }

    /// <summary>
    /// 凸多边形，顺时针序列，以第1个点来剖分三角形，如下：
    /// 0---1
    /// |   |
    /// 3---2  -->  (0, 1, 2)、(0, 2, 3)
    /// </summary>
    /// <param name="verts">顺时针排列的顶点列表</param>
    /// <param name="indexes">顶点索引列表</param>
    /// <returns>三角形列表</returns>
    private static List<int> ConvexTriangleIndex(List<Vector3> verts, List<int> indexes) {
        int len = verts.Count;
        // 若是闭环去除最后一点
        if (len > 1 && Vector3Equal(verts[0], verts[len - 1])) {
            len--;
        }
        int triangleNum = len - 2;
        List<int> triangles = new List<int>(triangleNum * 3);
        for (int i = 0; i < triangleNum; i++) {
            triangles.Add(indexes[0]);
            triangles.Add(indexes[i + 1]);
            triangles.Add(indexes[i + 2]);
        }
        return triangles;
    }

    /// <summary>
    /// 三角化（需要转换顶点到指定平面的情况下使用此方法）
    /// </summary>
    /// <param name="vertices">顶点列表</param>
    /// <param name="indexes">顺时针索引列表，执行后将更改，存储所有三角形索引列表</param>
    /// <param name="plane">坐标系平面</param>
    public static void WidelyTriangleIndex(Vector3[] vertices, ref List<int> indexes, Plane plane) {
        int len = indexes.Count;

        // 转换到平面坐标系
        List<Vector3> tempVertices = new List<Vector3>();
        List<int> tempIndices = new List<int>();
        Quaternion rotation = Quaternion.FromToRotation(plane.normal, Vector3.back);
        for (int i = 0; i < len; i++) {
            int index = indexes[i];
            Vector3 vertex = vertices[index];
            // 旋转至切割平面坐标系（从平面上方看向平面）
            vertex = rotation * vertex;
            vertex.z = 0;
            tempVertices.Add(vertex);
            tempIndices.Add(i);
        }

        List<int> resultIndices = WidelyTriangleIndex(tempVertices, tempIndices);

        List<int> newIndices = new List<int>();
        for (int i = 0, l = resultIndices.Count; i < l; i++) {
            int index = resultIndices[i];
            index = indexes[index];
            newIndices.Add(index);
        }
        indexes = newIndices;
    }

    /// <summary>
    /// 三角化（索引列表与 vertices 全部对应的情况下使用此方法）
    /// </summary>
    /// <param name="vertices">顶点列表</param>
    /// <returns></returns>
    public static List<int> WidelyTriangleIndex(Vector3[] vertices) {
        List<int> indices = new List<int>();
        int len = vertices.Length;
        for (int i = 0; i < len; i++) {
            indices.Add(i);
        }
        return WidelyTriangleIndex(new List<Vector3>(vertices), indices);
    }

    /// <summary>
    /// 三角剖分
    /// 1.寻找一个可划分顶点
    /// 2.分割出新的多边形和三角形
    /// 3.新多边形若为凸多边形，结束；否则继续剖分
    /// 
    /// 寻找可划分顶点
    /// 1.顶点是否为凸顶点：顶点在剩余顶点组成的图形外
    /// 2.新的多边形没有顶点在分割的三角形内
    /// </summary>
    /// <param name="points">顺时针排列的顶点列表</param>
    /// <param name="indexes">顶点索引列表</param>
    /// <returns>三角形列表</returns>
    public static List<int> WidelyTriangleIndex(List<Vector3> verts, List<int> indexes) {
        int len = verts.Count;
        if (len <= 3) return ConvexTriangleIndex(verts, indexes);

        int searchIndex = 0;
        List<int> covexIndex = new List<int>();
        bool isCovexPolygon = true; // 判断多边形是否是凸多边形

        for (searchIndex = 0; searchIndex < len; searchIndex++) {
            List<Vector3> polygon = new List<Vector3>(verts.ToArray());
            polygon.RemoveAt(searchIndex);
            if (IsPointInsidePolygon(verts[searchIndex], polygon)) {
                isCovexPolygon = false;
                break;
            } else {
                covexIndex.Add(searchIndex);
            }
        }
        if (isCovexPolygon) return ConvexTriangleIndex(verts, indexes);

        // 查找可划分顶点
        int canFragementIndex = -1; // 可划分顶点索引
        for (int i = 0; i < len; i++) {
            if (i > searchIndex) {
                List<Vector3> polygon = new List<Vector3>(verts.ToArray());
                polygon.RemoveAt(i);
                if (!IsPointInsidePolygon(verts[i], polygon) && IsFragementIndex(i, verts)) {
                    canFragementIndex = i;
                    break;
                }
            } else {
                if (covexIndex.IndexOf(i) != -1 && IsFragementIndex(i, verts)) {
                    canFragementIndex = i;
                    break;
                }
            }
        }

        if (canFragementIndex < 0) {
            Debug.LogError("数据有误找不到可划分顶点");
            return new List<int>();
        }

        // 用可划分顶点将凹多边形划分为一个三角形和一个多边形
        List<int> tTriangles = new List<int>();
        int next = (canFragementIndex == len - 1) ? 0 : canFragementIndex + 1;
        int prev = (canFragementIndex == 0) ? len - 1 : canFragementIndex - 1;
        tTriangles.Add(indexes[prev]);
        tTriangles.Add(indexes[canFragementIndex]);
        tTriangles.Add(indexes[next]);
        // 剔除可划分顶点及索引
        verts.RemoveAt(canFragementIndex);
        indexes.RemoveAt(canFragementIndex);

        // 递归划分
        List<int> leaveTriangles = WidelyTriangleIndex(verts, indexes);
        tTriangles.AddRange(leaveTriangles);

        return tTriangles;
    }

    /// <summary>
    /// 是否是可划分顶点:新的多边形没有顶点在分割的三角形内
    /// </summary>
    private static bool IsFragementIndex(int index, List<Vector3> verts) {
        int len = verts.Count;
        List<Vector3> triangleVert = new List<Vector3>();
        int next = (index == len - 1) ? 0 : index + 1;
        int prev = (index == 0) ? len - 1 : index - 1;
        triangleVert.Add(verts[prev]);
        triangleVert.Add(verts[index]);
        triangleVert.Add(verts[next]);
        for (int i = 0; i < len; i++) {
            if (i != index && i != prev && i != next) {
                if (IsPointInsidePolygon(verts[i], triangleVert)) {
                    return false;
                }
            }
        }
        return true;
    }

    /// <summary>
    /// 射线与线段相交性判断
    /// </summary>
    /// <param name="ray">射线</param>
    /// <param name="p1">线段头</param>
    /// <param name="p2">线段尾</param>
    /// <returns></returns>
    private static bool IsDetectIntersect(Ray2D ray, Vector3 p1, Vector3 p2) {
        float pointY; // 交点Y坐标，x固定值
        if (FloatEqual(p1.x, p2.x)) {
            return false;
        } else if (FloatEqual(p1.y, p2.y)) {
            pointY = p1.y;
        } else {
            // 直线两点式方程：(y-y2)/(y1-y2) = (x-x2)/(x1-x2)
            float a = p1.x - p2.x;
            float b = p1.y - p2.y;
            float c = p2.y / b - p2.x / a;

            pointY = b / a * ray.origin.x + b * c;
        }

        if (FloatLess(pointY, ray.origin.y)) {
            // 交点y小于射线起点y
            return false;
        } else {
            Vector3 leftP = FloatLess(p1.x, p2.x) ? p1 : p2; // 左端点
            Vector3 rightP = FloatLess(p1.x, p2.x) ? p2 : p1; // 右端点
            // 交点x位于线段两个端点x之外，相交与线段某个端点时，仅将射线L与左侧多边形一边的端点记为焦点(即就是：只将右端点记为交点)
            if (!FloatGreat(ray.origin.x, leftP.x) || FloatGreat(ray.origin.x, rightP.x)) {
                return false;
            }
        }

        return true;
    }

    /// <summary>
    /// 点与多边形的位置关系
    /// </summary>
    /// <param name="point">判定点</param>
    /// <param name="polygonVerts">剩余顶点按顺序排列的多边形</param>
    /// <returns>true:点在多边形之内，false:相反</returns>
    private static bool IsPointInsidePolygon(Vector3 point, List<Vector3> polygonVerts) {
        int len = polygonVerts.Count;
        Ray2D ray = new Ray2D(point, new Vector3(0, 1)); // y方向射线
        int interNum = 0;

        for (int i = 1; i < len; i++) {
            if (IsDetectIntersect(ray, polygonVerts[i - 1], polygonVerts[i])) {
                interNum++;
            }
        }

        // 不是闭环
        if (!Vector3Equal(polygonVerts[0], polygonVerts[len - 1])) {
            if (IsDetectIntersect(ray, polygonVerts[len - 1], polygonVerts[0])) {
                interNum++;
            }
        }
        int remainder = interNum % 2;
        return remainder == 1;
    }
}
