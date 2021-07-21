
using UnityEngine;

/// <summary>
/// 数学工具类
/// </summary>
public class Mathk {

    /// <summary>
    /// 等差数列前 n 项的和
    /// </summary>
    /// <param name="a1"> 数列的最小项 </param>
    /// <param name="n"> 有多少个项 </param>
    /// <param name="d"> 数列差 </param>
    /// <returns> 和 </returns>
    public static float GetArithmeticSequenceSum (float a1, float n, float d) {
        return n * a1 + n * (n - 1f) / 2f * d;
    }

    /// <summary>
    /// 根据等差数列前 n 项的和计算 n，结果为x1,x2两个根，正的根就是n
    /// </summary>
    /// <param name="sum"> 等差数列前 n 项的和 </param>
    /// <param name="a1"> 数列的最小项 </param>
    /// <param name="d"> 数列差 </param>
    /// <returns> 根的数量 </returns>
    public static int GetArithmeticSequenceN (float sum, float a1, float d, out float x1, out float x2) {
        int count = QuadraticEquation(d / 2f, a1 - d / 2f, -sum, out x1, out x2);
        return count;
    }

    /// <summary>
    /// 一元二次方程求根(ax²+bx+c=0)
    /// </summary>
    /// <param name="a"></param>
    /// <param name="b"></param>
    /// <param name="c"></param>
    /// <param name="x1"></param>
    /// <param name="x2"></param>
    /// <returns> 根的数量 </returns>
    public static int QuadraticEquation (float a, float b, float c, out float x1, out float x2) {
        float delta = b * b - 4 * a * c; // 根的判别式
        int count;
        if (delta > 0) {
            // 有两个不相等的实数根
            count = 2;
        } else if (delta == 0f) {
            // 有两个相等的实数根
            count = 1;
        } else {
            // 无实数根
            count = 0;
        }

        if (count > 0) {
            // 求根公式
            x1 = (-b + Mathf.Sqrt(delta)) / (2 * a);
            x2 = (-b - Mathf.Sqrt(delta)) / (2 * a);
        } else {
            x1 = 0f;
            x2 = 0f;
        }
        return count;
    }

    /// <summary>
    /// 将任意角度转换为[-180°,180°]，并返回转换后的角度
    /// </summary>
    /// <param name="rotation">需要转换的角度</param>
    /// <returns></returns>
    public static float GetRotationTo180 (float rotation) {
        rotation %= 360.0f;
        if (rotation > 180.0f) rotation -= 360.0f;
        else if (rotation < -180.0f) rotation += 360.0f;
        return rotation;
    }

    /// <summary>
    /// 计算出目标角减当前角的差（取到达目标角度的最近旋转方向）,并返回这个差
    /// </summary>
    /// <param name="rotation">当前角度</param>
    /// <param name="targetRotation">目标角度</param>
    /// <returns></returns>
    public static float GetRotationDifference (float rotation, float targetRotation) {
        rotation = GetRotationTo360(rotation);
        targetRotation = GetRotationTo360(targetRotation);
        float offset = targetRotation - rotation;
        if (Mathf.Abs(offset) > 180.0f) {
            float reDir = offset >= 0 ? -1 : 1;
            offset = reDir * (360.0f - Mathf.Abs(offset));
        }
        return offset;
    }

    /// <summary>
    /// 将任意角度转换为[0°,360°]的值,并返回转换后的值（此函数的与 Mathf.Repeat(rotation,360f) 效果一样）
    /// </summary>
    /// <param name="rotation">需要转换的角度</param>
    /// <returns></returns>
    public static float GetRotationTo360 (float rotation) {
        rotation = GetRotationTo180(rotation);
        if (rotation < 0) rotation += 360f;
        return rotation;
    }


}
