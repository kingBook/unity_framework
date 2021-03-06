﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// 随机工具类
/// </summary>
public static class RandomUtil {
	
	/// <summary>
    /// 随机返回 1.0f 或 -1.0f
    /// </summary>
    public static float sign => Random.value > 0.5f ? 1f : -1f;
	
	
    /// <summary>
    /// 返回一个指定长度的随机int数组，数组元素范围是在[min,max)区间内(包括min,排除max)不重复的整数。
    /// 注意：参数length的取值范围必须在[1,max-min]区间内，length小于1时取值：1，length大于max-min时取值：max-min。
    /// 例：
    /// ```
    /// GetRandomUniqueIntList(0,10,10); //返回元素在[0,10)之间，长度为10的数组
    /// ```
    /// </summary>
    public static int[] GetRandomUniqueIntList (int min, int max, int length) {
        int sourceLength = max - min;
        length = Mathf.Clamp(length, 1, sourceLength);

        int[] results = new int[length];

        List<int> sourceList = new List<int>(sourceLength);
        int i;
        for (i = 0; i < sourceLength; i++) {
            sourceList.Add(min + i);
        }

        int randomIndex;
        for (i = 0; i < length; i++) {
            randomIndex = Random.Range(0, sourceList.Count);
            results[i] = sourceList[randomIndex];
            sourceList.RemoveAt(randomIndex);
        }
        return results;
    }

}
