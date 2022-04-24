#pragma warning disable 0649

using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class TestCityBloxxShake : MonoBehaviour {

    [SerializeField] private GameObject m_blockPrefab;

    private void Start() {
        m_blockPrefab.SetActive(false);

        Transform parent = null;
        for (int i = 0; i < 20; i++) {
            var inst = Instantiate(m_blockPrefab);
            inst.name = "CityBloxxBlock " + i;


            if (parent) {
                inst.transform.SetParent(parent);
            } else {
                inst.transform.SetParent(transform);
            }

            var block = inst.GetComponent<CityBloxxBlock>();
            block.SetId(i);

            inst.transform.localPosition = new Vector3(0f, 1f, 0f);
            inst.SetActive(true);

            parent = inst.transform;
        }
    }


    private void Update() {

    }
}
