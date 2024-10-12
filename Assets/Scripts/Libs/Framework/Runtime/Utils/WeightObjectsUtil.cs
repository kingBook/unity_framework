using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class WeightObjectsUtil {

    /// <summary>
    /// 随机在权重列表返回一个枚举类型
    /// </summary>
    /// <typeparam name="T"> 枚举类型 </typeparam>
    /// <param name="weightObjects"> 权重对象列表 </param>
    /// <param name="defaultType"> 默认返回的枚举类型 </param>
    /// <returns></returns>
    public static T GetTypeWithWeightObjects<T>(ICollection<(T type, int weight)> weightObjects, T defaultType) {
        // 总权重
        int sumWeight = 0;
        foreach (var (type, weight) in weightObjects) {
            sumWeight += weight;
        }

        // 随机数 [0, sumWeight)
        int n = Random.Range(0, sumWeight);
        // 根据随机数所在总权重线段上的落点计算出结果
        int m = 0;
        foreach (var (type, weight) in weightObjects) {
            if (n >= m && n < m + weight) {
                return type;
            }
            m += weight;
        }
        return defaultType;
    }
}
