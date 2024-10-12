using System.Collections;
using UnityEngine;
using System.Text;

public static class Grid2DUtil {

    /// <summary>
    /// 根据指定的格子位置，获取在一维网格数据数组中的索引
    /// </summary>
    /// <param name="x"> 表示网格中的第几列 </param>
    /// <param name="y"> 表示网格中的第几行 </param>
    /// <param name="col"> 注意：表示竖向有多少列，此值表示x，通常3x4的网格，其中的3表示此值 </param>
    /// <returns></returns>
    public static int GetIndex(int x, int y, int col) {
        return y * col + x;
    }

    /// <summary>
    /// 根据一维网格数据数组中的索引，获取格子的位置 [x,y]
    /// </summary>
    /// <param name="index"> 一维网格数据数组中的索引 </param>
    /// <param name="col"> 注意：表示竖向有多少列，此值表示x，通常3x4的网格，其中的3表示此值 </param>
    /// <param name="result"> 长度为2的int数组 </param>
    public static void GetPosNonAlloc(int index, int col, int[] result) {
        int x = index % col;
        int y = index / col;
        result[0] = x;
        result[1] = y;
    }

    /// <summary>
    /// 获取网格字符串
    /// </summary>
    /// <param name="gridData"> 二维网格数据 </param>
    /// <returns></returns>
    public static string GetGridString(int[][] gridData) {
        int col = gridData.Length;
        int row = gridData[0].Length;

        StringBuilder stringBuilder = new StringBuilder();
        for (int i = 0; i < row; i++) {
            for (int j = 0; j < col; j++) {
                stringBuilder.Append(gridData[j][i] < 0 ? gridData[j][i].ToString() : ' ' + gridData[j][i].ToString());
                stringBuilder.Append(j + 1 == col ? '\n' : ' ');
            }
        }
        return stringBuilder.ToString();
    }

    /// <summary>
    /// 获取网格字符串
    /// </summary>
    /// <param name="gridData"> 二维网格数据 </param>
    /// <returns></returns>
    public static string GetGridString(sbyte[][] gridData) {
        int col = gridData.Length;
        int row = gridData[0].Length;

        StringBuilder stringBuilder = new StringBuilder();
        for (int i = 0; i < row; i++) {
            for (int j = 0; j < col; j++) {
                stringBuilder.Append(gridData[j][i] < 0 ? gridData[j][i].ToString() : ' ' + gridData[j][i].ToString());
                stringBuilder.Append(j + 1 == col ? '\n' : ' ');
            }
        }
        return stringBuilder.ToString();
    }

    /// <summary>
    /// 获取网格字符串
    /// </summary>
    /// <param name="gridData"> 一维的网格数据 </param>
    /// <param name="col"> 注意：表示竖向有多少列，此值表示x，通常3x4的网格，其中的3表示此值</param>
    /// <param name="row"> 注意：表示横向有多少行，此值表示y，通常3x4的网格，其中的4表示此值 </param>
    /// <returns></returns>
    public static string GetGridString(sbyte[] gridData, int col, int row) {
        StringBuilder stringBuilder = new StringBuilder();
        for (int i = 0, len = col * row; i < len; i++) {
            stringBuilder.Append(gridData[i] < 0 ? gridData[i].ToString() : ' ' + gridData[i].ToString());
            stringBuilder.Append((i + 1) % col == 0 ? '\n' : ' ');
        }
        return stringBuilder.ToString();
    }
}
