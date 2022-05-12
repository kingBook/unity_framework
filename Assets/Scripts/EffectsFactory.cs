using System.Collections;
using UnityEngine;

public class EffectsFactory : MonoBehaviour {

    [SerializeField] private GameObject m_itemDestroyEffectPrefab;

    private GameObject CreateInstance(GameObject prefab, Transform parent, Vector3 position, Quaternion rotation) {
        var inst = Instantiate(prefab, parent);
        inst.transform.SetPositionAndRotation(position, rotation);
        return inst;
    }


    public GameObject CreateItemDestroyEffect(Transform parent, Vector3 position, Quaternion rotation) {
        return CreateInstance(m_itemDestroyEffectPrefab, parent, position, rotation);
    }


}
