using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

/// <summary>
/// 哈密尔顿路径算法
/// https://www.cnblogs.com/minuy/p/15592562.html
/// </summary>
public class HamiltonianPathAlgorithm {

    /// <summary> 连接点（记录连接的父级） </summary>
    public struct LinkPoint {
        public int x;
        public int y;
        /// <summary> 连接的父级（在关闭列表中的索引） </summary>
        public int parentIndex;

        public LinkPoint(int x, int y, int parentIndex) {
            this.x = x;
            this.y = y;
            this.parentIndex = parentIndex;
        }
    }

    /// <summary> 是否输出路径，如果不需要输出路径，禁用此项可提升2~3毫秒性能 </summary>
    public bool isOutputPath = true;

    /// <summary> 表示可通过点的值 </summary>
    public const sbyte NONE_VALUE = 0;
    /// <summary> 表示起始点的值 </summary>
    public const sbyte START_VALUE = 1;
    /// <summary> 表示终点的值 </summary>
    public const sbyte END_VALUE = 2;
    /// <summary> 表示障碍的值 </summary>
    public const sbyte OBSTACLE_VALUE = -1;

    /// <summary> 用于存储格子是否已被计算 </summary>
    private bool[][] m_isVisited;
    private Grid2D m_grid;
    private (int x, int y) m_start, m_end;
    private int m_col, m_row;
    /// <summary> 存储算法执行后被计算过的连接点 </summary>
    private LinkPoint[] m_closeList;
    /// <summary> 存储各路径的最后一个连接点 </summary>
    private LinkPoint[] m_pathEndPoints;
    private int m_closeListCount;
    private int m_pathEndPointsCount;

    private readonly int[][] m_dir4 = {
                new int[]{0, 1},
        new int[]{-1, 0}, new int[]{1, 0},
                new int[]{0, -1}
    };

    /// <summary>
    /// 创建网格
    /// </summary>
    /// <param name="col"> 列数，表示竖向有多少列，此值表示x，通常3x4的网格，其中的3表示此值 </param>
    /// <param name="row"> 行数，表示横向有多少行，此值表示y，通常3x4的网格，其中的4表示此值 </param>
    /// <param name="start"> 起始点(1)，如[0,0] </param>
    /// <param name="end"> 终点(2)，如[4,4] </param>
    /// <param name="obstacles"> 障碍点(-1)，如[0,3],[1,3] </param>
    /// <returns></returns>
    public static Grid2D CreateGrid(int col, int row, int[] start, int[] end, params int[][] obstacles) {
        var grid = new Grid2D(col, row);

        grid[start[0], start[1]] = START_VALUE;
        grid[end[0], end[1]] = END_VALUE;

        for (int i = 0, len = obstacles.Length; i < len; i++) {
            grid[obstacles[i][0], obstacles[i][1]] = OBSTACLE_VALUE;
        }
        return grid;
    }

    /// <summary> 获取一条路径的字符串 </summary>
    public static string GetPathString((int x, int y)[] path, int count) {
        const char symbol = ',';
        StringBuilder stringBuilder = new StringBuilder();
        for (int i = 0; i < count; i++) {
            stringBuilder.Append(path[i]);
            if (i < count - 1) {
                stringBuilder.Append(symbol);
            }
        }
        return stringBuilder.ToString();
    }

    public Grid2D grid => m_grid;


    public HamiltonianPathAlgorithm(Grid2D grid) {
        Init(grid);
    }

    public HamiltonianPathAlgorithm(sbyte[] gridData, int col, int row) {
        var grid = new Grid2D(gridData, col, row);
        Init(grid);
    }

    public HamiltonianPathAlgorithm(int col, int row, int[] start, int[] end, params int[][] obstacles) {
        var grid = CreateGrid(col, row, start, end, obstacles);
        Init(grid);
    }

    /// <summary> 设置为起始点 </summary>
    public void SetStart(int x, int y) {
        m_start = (x, y);
        m_grid[x, y] = START_VALUE;
    }

    /// <summary> 设置为终点 </summary>
    public void SetEnd(int x, int y) {
        m_end = (x, y);
        m_grid[x, y] = END_VALUE;
    }

    /// <summary> 设置为障碍点 </summary>
    public void SetObstacle(int x, int y) {
        m_grid[x, y] = OBSTACLE_VALUE;
    }

    /// <summary> 设置为可通过点 </summary>
    public void SetNone(int x, int y) {
        m_grid[x, y] = NONE_VALUE;
    }

    /// <summary>
    /// 算法执行后，根据指定索引，获取一条路径
    /// </summary>
    /// <param name="result">  </param>
    /// <param name="pathIndex"> 必须在<see cref="Execute"/>方法返回路径数量范围内 </param>
    /// <returns> 返回路径点的个数 </returns>
    public int GetPathNonAlloc((int x, int y)[] result, int pathIndex) {
        if (m_pathEndPointsCount <= 0) return 0;
        pathIndex = Mathf.Clamp(pathIndex, 0, m_pathEndPointsCount - 1);

        int idx = 0;
        LinkPoint pt = m_pathEndPoints[pathIndex];
        while (pt.parentIndex > -1) {
            result[idx++] = (pt.x, pt.y);
            pt = m_closeList[pt.parentIndex];
        }
        result[idx++] = (m_start.x, m_start.y);
        System.Array.Reverse(result, 0, idx);
        return idx;
    }

    /// <summary>
    /// 执行算法
    /// </summary>
    /// <returns> 返回能从起始点到达终点的哈密尔顿路径数量 </returns>
    public int Execute() {
        m_closeListCount = 0;
        m_pathEndPointsCount = 0;

        int c = 0;
        for (int i = 0; i < m_col; i++) {
            for (int j = 0; j < m_row; j++) {
                m_isVisited[i][j] = false;

                int cellValue = m_grid[i, j];
                if (cellValue == NONE_VALUE) {
                    c++;
                } else if (cellValue == START_VALUE) {
                    m_start.x = i;
                    m_start.y = j;
                    c++;
                } else if (cellValue == END_VALUE) {
                    m_end.x = i;
                    m_end.y = j;
                    c++;
                }
            }
        }
        return Dfs(m_start.x, m_start.y, c, -1);
    }

    private int Dfs(int x, int y, int c, int parentIndex) {
        if (isOutputPath) {
            m_closeList[m_closeListCount].x = x;
            m_closeList[m_closeListCount].y = y;
            m_closeList[m_closeListCount].parentIndex = parentIndex;
            parentIndex = m_closeListCount;
            m_closeListCount++;
            if (m_closeListCount >= m_closeList.Length) {
                System.Array.Resize(ref m_closeList, m_closeList.Length + 128);
            }
        }

        m_isVisited[x][y] = true;
        c--;

        if (c == 0) {
            m_isVisited[x][y] = false; // 回溯，给其他路径留活路
            if (x == m_end.x && y == m_end.y) {
                if (isOutputPath) {
                    m_pathEndPoints[m_pathEndPointsCount++] = m_closeList[m_closeListCount - 1];
                    if (m_pathEndPointsCount >= m_pathEndPoints.Length) {
                        System.Array.Resize(ref m_pathEndPoints, m_closeList.Length + 8);
                    }
                }
                return 1; // 找到了一条哈密尔顿路径
            }
            return 0;
        }

        int res = 0;
        // 获取相邻顶点
        for (int i = 0; i < 4; i++) {
            int[] ints = m_dir4[i];
            int x1 = x + ints[0];
            int y1 = y + ints[1];
            if (x1 >= 0 && y1 >= 0 && x1 < m_col && y1 < m_row && m_grid[x1, y1] != OBSTACLE_VALUE && !m_isVisited[x1][y1]) {
                res += Dfs(x1, y1, c, parentIndex);
            }
        }

        m_isVisited[x][y] = false; // 回溯
        return res;
    }

    private void Init(Grid2D grid) {
        m_grid = grid;
        m_col = m_grid.col;
        m_row = m_grid.row;

        m_isVisited = new bool[m_col][];
        for (int i = 0; i < m_col; i++) {
            m_isVisited[i] = new bool[m_row];
            for (int j = 0; j < m_row; j++) {
                m_isVisited[i][j] = false;
            }
        }

        m_closeList = new LinkPoint[200000];
        m_pathEndPoints = new LinkPoint[128];
    }



}
