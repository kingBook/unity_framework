using System.Collections;
using System.Text;
using UnityEngine;


public class Grid2D {

    /// <summary> 一维网格数据数组 </summary>
    public sbyte[] data { get; private set; }
    /// <summary> 注意：表示竖向有多少列，此值表示x，通常3x4的网格，其中的3表示此值 </summary>
    public int col { get; private set; }
    /// <summary> 注意：表示横向有多少行，此值表示y，通常3x4的网格，其中的4表示此值 </summary>
    public int row { get; private set; }


    /// <summary>
    /// 构建网格
    /// </summary>
    /// <param name="col"> 注意：表示竖向有多少列，此值表示x，通常3x4的网格，其中的3表示此值 </param>
    /// <param name="row"> 注意：表示横向有多少行，此值表示y，通常3x4的网格，其中的4表示此值 </param>
    public Grid2D(int col, int row) {
        sbyte[] data = new sbyte[col * row];
        Init(data, col, row);
    }

    /// <summary>
    /// 构建网格
    /// </summary>
    /// <param name="data"> 一维网格数据数组 </param>
    /// <param name="col"> 注意：表示竖向有多少列，此值表示x，通常3x4的网格，其中的3表示此值 </param>
    /// <param name="row"> 注意：表示横向有多少行，此值表示y，通常3x4的网格，其中的4表示此值 </param>
    public Grid2D(sbyte[] data, int col, int row) {
        Init(data, col, row);
    }

    private void Init(sbyte[] data, int col, int row) {
        this.data = data;
        this.col = col;
        this.row = row;
    }

    /// <summary>
    /// 根据指定的格子位置，获取在一维网格数据数组中的索引
    /// </summary>
    /// <param name="x"> 表示网格中的第几列 </param>
    /// <param name="y"> 表示网格中的第几行 </param>
    /// <returns></returns>
    public int GetIndex(int x, int y) {
        return y * col + x;
    }

    /// <summary>
    /// 根据一维网格数据数组中的索引，获取格子的位置 [x,y]
    /// </summary>
    /// <param name="index"> 一维网格数据数组中的索引 </param>
    /// <param name="result"> 长度为2的int数组 </param>
    public void GetPosNonAlloc(int index, int[] result) {
        int x = index % col;
        int y = index / col;
        result[0] = x;
        result[1] = y;
    }

    /// <summary>
    /// 获取或设置格子的值
    /// </summary>
    /// <param name="x"> 注意：表示第几列，通常3x4的网格，其中的3表示此值 </param>
    /// <param name="y"> 注意：表示第几行，通常3x4的网格，其中的4表示此值 </param>
    /// <returns></returns>
    public sbyte this[int x, int y] {
       get => data[y * col + x];
        set => data[y * col + x] = value;
    }

    public override string ToString() {
        return ToString(string.Empty);
    }

    /// <summary>
    /// 返回 <see cref="Grid2D"/> 字符串表
    /// </summary>
    /// <param name="spacingSymbol"> 元素之间间隔符号 </param>
    /// <returns></returns>
    public string ToString(string spacingSymbol) {
        StringBuilder stringBuilder = new StringBuilder();
        const string space = " ";
        for (int i = 0, len = col * row; i < len; i++) {
            stringBuilder.Append(data[i] < 0 ? data[i].ToString() : space + data[i].ToString());
            if (i < len - 1) {
                if (string.IsNullOrEmpty(spacingSymbol)) {
                    stringBuilder.Append((i + 1) % col == 0 ? "\n" : space);
                } else {
                    stringBuilder.Append((i + 1) % col == 0 ? spacingSymbol + "\n" : spacingSymbol + space);
                }
            }
        }
        return stringBuilder.ToString();
    }
}
