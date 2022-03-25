using System.Collections;
using UnityEngine;

public class EffectsFactory : MonoBehaviour {

    [SerializeField] private GameObject m_itemDestroyEffectPrefab;

    public GameObject CreateItemDestroyEffect (Transform parent, Vector3 position) {
        var inst = Instantiate(m_itemDestroyEffectPrefab, parent);
        inst.transform.position = position;
        return inst;
    }
}
