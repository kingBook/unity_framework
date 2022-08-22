using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// 随机工具类
/// </summary>
public static class RandomUtil {

    /// <summary> 随机返回浮点数 1.0f 或 -1.0f </summary>
    public static float sign => Random.value > 0.5f ? 1f : -1f;

    /// <summary> 随机返回整数 1 或 -1 </summary>
    public static int signInteger => Random.value > 0.5f ? 1 : -1;

    /// <summary>
    /// 返回一个不重复的随机int数组，元素是在[min,max)区间内
    /// <para> 注意：result 的长度可能大于实际返回元素的个数 </para>
    /// <code> 例: GetRandomInts(0, 10, tempInts, result, 10); // result 数组的元素在[0,10)之间，长度为10 </code>
    /// </summary>
    /// <param name="min"> 元素的最小值 </param>
    /// <param name="max"> 元素的最大值 </param>
    /// <param name="tempInts"> 用于内部计算的临时数组，长度不能小于  max - min </param>
    /// <param name="result"> 存储结果的数组，长度不能小于 count </param>
    /// <param name="count"> 返回元素的个数，必须在[1,max-min]区间内 </param>
    /// <returns> 返回元素的个数 </returns>
    public static int GetRandomInts(int min, int max, int[] tempInts, int[] result, int count) {
        int maxCount = max - min;
        if (tempInts.Length < maxCount) {
            Debug.LogError("The length of tempInts cannot be less than max - min");
        }
        if (count < 1 || count > maxCount) {
            Debug.LogError("The count cannot be out of range [1, max-min)");
        }
        if (result.Length < count) {
            Debug.LogError("The length of result cannot be less than count");
        }

        for (int i = 0; i < maxCount; i++) {
            tempInts[i] = min + i;
        }

        RandomizeArray<int>(tempInts, maxCount);

        for (int i = 0; i < count; i++) {
            result[i] = tempInts[i];
        }
        return count;
    }

    /// <summary>
    /// 从一个数组中随机获取 [min, max) 个元素
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="collection"> 源数组 </param>
    /// <param name="tempCollection"> 函数内部使用的临时数组，长度不能小于 collection.Length </param>
    /// <param name="result"> 存储结果的数组 </param>
    /// <param name="min">  </param>
    /// <param name="max">  </param>
    /// <returns> 返回获取随机元素的个数 </returns>
    public static int GetRandomElements<T>(T[] collection, T[] tempCollection, T[] result, int min, int max) {
        if (result.Length < max) {
            Debug.LogError($"The length of result cannot be less than max");
        }
        int count = Random.Range(min, max);
        GetRandomElements(collection, tempCollection, result, count);
        return count;
    }

    /// <summary>
    /// 从一个 List 中随机获取 [min, max) 个元素
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="collection"> 源 List </param>
    /// <param name="tempCollection"> 函数内部使用的临时数组，长度不能小于 collection.Length </param>
    /// <param name="result"> 存储结果的数组 </param>
    /// <param name="min">  </param>
    /// <param name="max">  </param>
    /// <returns> 返回获取随机元素的个数 </returns>
    public static int GetRandomElements<T>(List<T> collection, T[] tempCollection, T[] result, int min, int max) {
        if (result.Length < max) {
            Debug.LogError($"The length of result cannot be less than max");
        }
        int count = Random.Range(min, max);
        GetRandomElements(collection, tempCollection, result, count);
        return count;
    }

    /// <summary>
    /// 从一个数组中随机获取 count 个元素
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="collection"> 源数组 </param>
    /// <param name="tempCollection"> 函数内部使用的临时数组，长度不能小于 collection.Length </param>
    /// <param name="result"> 存储结果的数组 </param>
    /// <param name="count"> 获取随机元素的个数 </param>
    /// <returns> 返回获取随机元素的个数 </returns>
    public static int GetRandomElements<T>(T[] collection, T[] tempCollection, T[] result, int count) {
        if (tempCollection.Length < collection.Length) {
            Debug.LogError($"The length of tempCollection cannot be less than {collection.Length} (collection.Length)");
        }
        if (result.Length < count) {
            Debug.LogError($"The length of result cannot be less than {count}");
        }

        collection.CopyTo(tempCollection, 0);

        RandomizeArray(tempCollection, count);

        for (int i = 0; i < count; i++) {
            result[i] = tempCollection[i];
        }
        return count;
    }

    /// <summary>
    /// 从一个 List 中随机获取 count 个元素
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="collection"> 源 List </param>
    /// <param name="tempCollection"> 函数内部使用的临时数组，长度不能小于 collection.Count </param>
    /// <param name="result"> 存储结果的数组 </param>
    /// <param name="count"> 获取随机元素的个数 </param>
    /// <returns> 返回获取随机元素的个数 </returns>
    public static int GetRandomElements<T>(List<T> collection, T[] tempCollection, T[] result, int count) {
        if (tempCollection.Length < collection.Count) {
            Debug.LogError($"The length of tempCollection cannot be less than {collection.Count} (collection.Count)");
        }
        if (result.Length < count) {
            Debug.LogError($"The length of result cannot be less than {count}");
        }

        collection.CopyTo(tempCollection, 0);

        RandomizeArray(tempCollection, count);

        for (int i = 0; i < count; i++) {
            result[i] = tempCollection[i];
        }
        return count;
    }

    /// <summary>
    /// 随机化的一个数组
    /// </summary>
    public static void RandomizeArray<T>(T[] collection) {
        RandomizeArray<T>(collection, collection.Length);
    }

    /// <summary>
    /// 随机化的一个数组
    /// </summary>
    public static void RandomizeArray<T>(T[] collection, int length) {
        for (int i = 0; i < length; i++) {
            int randomIndex = Random.Range(0, length);
            T val = collection[i];
            collection[i] = collection[randomIndex];
            collection[randomIndex] = val;
        }
    }

    /// <summary>
    /// 随机化的一个 List
    /// </summary>
    public static void RandomizeList<T>(List<T> collection) {
        RandomizeList<T>(collection, collection.Count);
    }

    /// <summary>
    /// 随机化的一个 List
    /// </summary>
    public static void RandomizeList<T>(List<T> collection, int length) {
        for (int i = 0; i < length; i++) {
            int randomIndex = Random.Range(0, length);
            T val = collection[i];
            collection[i] = collection[randomIndex];
            collection[randomIndex] = val;
        }
    }

    /// <summary>
    /// 返回一个介于min[包括]和max[排除]之间的随机整数（此方法与<see cref="Random.Range(int, int)"/>作用一样）
    /// <para> 增加的 ignores 参数，用于设置忽略的随机数 </para>
    /// <para> 注意：tempInts用于内部计算，长度不能小于 max-min-ignores.Length </para>
    /// </summary>
    public static int Range(int min, int max, int[] ignores, int[] tempInts) {
        int count = max - min;

        if (tempInts.Length < count - ignores.Length) {
            Debug.LogError($"The length of tempInts cannot be less than {count - ignores.Length} (max-min-ignores.Length).");
        }

        int ignoreCount = 0;
        for (int i = 0; i < count; i++) {
            int n = min + i;
            if (System.Array.IndexOf(ignores, n) > -1) {
                ignoreCount++;
                continue;
            }
            tempInts[i - ignoreCount] = n;
        }

        count -= ignoreCount;
        if (count <= 0) {
            Debug.LogError("All numbers from min to max are in the ignored range.");
            return 0;
        }
        return tempInts[Random.Range(0, count)];
    }
}
