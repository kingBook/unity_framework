using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TestParams : MonoBehaviour {

    private int[] m_ints = new int[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
    // Start is called before the first frame update
    private int[] m_ignores = new int[] {1,6,7,8 };
    private int[] m_tempInts = new int[10];
    void Start() {
        var result = new int[3];
        RandomUtil.GetRandomElements<int>(m_ints, m_tempInts, result, result.Length);

        for (int i = 0; i < result.Length; i++) {
            Debug.Log(result[i]);
        }


    }

    // Update is called once per frame
    void Update() {
        
    }

    private void Test(int a, params int[] ints) {

    }
}
