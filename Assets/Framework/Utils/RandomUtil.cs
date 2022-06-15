using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// 随机工具类
/// </summary>
public static class RandomUtil {

    private static int[] s_rangeInts = new int[24];

    /// <summary> 随机返回浮点数 1.0f 或 -1.0f </summary>
    public static float sign => Random.value > 0.5f ? 1f : -1f;

    /// <summary> 随机返回整数 1 或 -1 </summary>
    public static int signInteger => Random.value > 0.5f ? 1 : -1;

    /// <summary>
    /// 返回一个随机不重复的int数组，元素范围是在[min,max)区间内(包括min,排除max)。
    /// <para> 注意：参数length的取值范围必须在[1,max-min]区间内，length小于1时取值：1，length大于max-min时取值：max-min。</para>
    /// <para> 例：</para>
    /// <code>
    /// GetRandomUniqueIntList(0,10,10); //返回元素在[0,10)之间，长度为10的数组
    /// </code>
    /// </summary>
    public static int[] GetRandomUniqueIntList(int min, int max, int length) {
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

    /// <summary>
    /// 从一个数组中随机获取 [min, max) 个元素
    /// </summary>
    public static T[] GetRandomElements<T>(T[] collection, int min, int max) {
        int count = Random.Range(min, max);
        return GetRandomElements(collection, count);
    }

    /// <summary>
    /// 从一个数组中随机获取 count 个元素
    /// </summary>
    public static T[] GetRandomElements<T>(T[] collection, int count) {
        count = Mathf.Clamp(count, 0, collection.Length);
        T[] results = new T[count];
        List<T> tempList = new List<T>(collection);
        for (int i = 0; i < count; i++) {
            int index = Random.Range(0, tempList.Count);
            results[i] = tempList[index];
            tempList.RemoveAt(index);
        }
        return results;
    }

    /// <summary>
    /// 获取随机化后的数组
    /// </summary>
    public static T[] GetRandomizedArray<T>(T[] collection) {
        int count = collection.Length;
        T[] results = new T[count];
        List<T> tempList = new List<T>(collection);
        for (int i = 0; i < count; i++) {
            int index = Random.Range(0, tempList.Count);
            results[i] = tempList[index];
            tempList.RemoveAt(index);
        }
        return results;
    }

    /// <summary>
    /// 返回一个介于min[包括]和max[排除]之间的随机整数（此方法与<see cref="Random.Range(int, int)"/>作用一样，增加的 ignores 参数，用于设置忽略的随机数，效率较低）
    /// </summary>
    public static int Range(int min, int max, params int[] ignores) {
        int count = max - min;
        if (s_rangeInts.Length < count) {
            s_rangeInts = new int[count];
        }

        int spaceCount = 0;
        for (int i = 0; i < count; i++) {
            int n = min + i;
            if (System.Array.IndexOf(ignores, n) > -1) {
                spaceCount++;
                continue;
            }
            s_rangeInts[i - spaceCount] = n;
        }

        count -= spaceCount;
        if (count <= 0) {
            Debug.LogError("[min,max) 区间的整数，全被忽略，全在ignores里，将返回最小整数 0");
            return 0;
        }
        return s_rangeInts[Random.Range(0, count)];
    }
}
