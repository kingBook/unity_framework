using System.Collections;
using UnityEngine;
/// <summary>
/// 自定义的Log类
/// <br>1.解决Debug.Log()/Debug.LogFormat()多参数不方便和在发布版本Log不剔除问题(需要在Log方法声明的上一行加入 [Conditional("UNITY_EDITOR")] 表示只在使用编辑器调试时输出)</br>
/// <br>2.解决Vector3/Vector2很小的小数输出为0，如(1,6.167067E-11,-1.241278E-08)</br>
/// </summary>
/// 
public static class Debug2 {

    /// <summary>
    /// 画多点线
    /// </summary>
    /// <param name="points"></param>
    /// <param name="color"></param>
    /// <param name="duration"></param>
    /// <param name="depthTest"> 该线是否应被靠近摄影机的对象遮挡？ </param>
    public static void DrawPoints (Vector3[] points, Color color, float duration = 0f, bool depthTest = true) {
        for (int i = 0, length = points.Length; i < length; i++) {
            if (i < length - 1) {
                Debug.DrawLine(points[i], points[i + 1], color, duration, depthTest);
            }
        }
    }

    public static void Log (params object[] args) {
        int len = args.Length;
        string str = "";
        for (int i = 0; i < len; i++) {
            str += GetObjectString(args[i]);
            if (i < len - 1) str += ' ';
        }
        Debug.Log(str);
    }

    private static string GetListString (IList list) {
        int len = list.Count;
        string str = "";
        for (int i = 0; i < len; i++) {
            str += GetObjectString(list[i]);
            if (i < len - 1) str += ", ";
        }
        return str;
    }

    private static string GetObjectString (object obj) {
        if (obj is Vector3 v3) {
            return string.Format("({0},{1},{2})", v3.x, v3.y, v3.z);
        } else if (obj is Vector2 v2) {
            return string.Format("({0},{1})", v2.x, v2.y);
        } else if (obj is IList list) {
            return GetListString(list);
        } else if (obj is Vector4 v4) {
            return string.Format("({0},{1},{2},{3})", v4.x, v4.y, v4.z, v4.w);
        }
        return (obj == null) ? "Null" : obj.ToString();
    }

}