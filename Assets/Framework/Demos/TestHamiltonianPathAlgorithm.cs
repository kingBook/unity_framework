using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestHamiltonianPathAlgorithm : MonoBehaviour {

    private void Start() {
        var hm = new HamiltonianPathAlgorithm(4, 4, new int[] { 0, 0 }, new int[] { 3, 3 },
            new int[] { 1, 0 }
        );

        Debug.Log(hm.grid);

        int pathCount = hm.Execute();
        Debug.Log($"pathCount:{pathCount}");

        (int x, int y)[] path = new (int x, int y)[hm.grid.col * hm.grid.row];
        for (int i = 0; i < pathCount; i++) {
            int count = hm.GetPathNonAlloc(path, i);
            Debug.Log($"i:{i} pathString:{HamiltonianPathAlgorithm.GetPathString(path, count)}");
        }
    }

}
