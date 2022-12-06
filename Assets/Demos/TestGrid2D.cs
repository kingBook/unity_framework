using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestGrid2D : MonoBehaviour {

    private void Start() {
        sbyte[] gridData = new sbyte[]{
            0,1,2,
            3,4,5,
            6,7,8
        };

        var grid = new Grid2D(gridData, 3, 3);

        Debug.Log($"(0, 2) to 1D: {grid.GetIndex(0, 2)}"); // output: (0, 2) to 1D: 6

        // 一维数组索引 6，在网格的格子位置
        var pos = new int[2];
        grid.GetPosNonAlloc(6, pos);
        Debug.Log($"6 to 2D: ({pos[0]}, {pos[1]})"); // output: 6 to 2D: (0, 2)

        // output:
        // 0  1  2
        // 3  4  5
        // 6  7  8
        Debug.Log(grid.ToString());
        // output:
        // 0,  1,  2,
        // 3,  4,  5,
        // 6,  7,  8
        Debug.Log(grid.ToString(","));

        Debug.Log(grid[0, 2]); // output: 6

        grid[0, 2] = -1;
        Debug.Log(grid[0, 2]); // output: -1

        // output:
        //  0  1  2
        //  3  4  5
        // -1  7  8
        Debug.Log(grid);
    }

}
