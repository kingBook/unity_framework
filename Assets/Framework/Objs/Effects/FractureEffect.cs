#pragma warning disable 0649

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 破碎效果
/// </summary>
public class FractureEffect : MonoBehaviour {

    public RangeFloat explosionRadiusRange = new RangeFloat(3f, 10f);
    public RangeFloat explosionForceRange = new RangeFloat(100f, 300);
    public Vector3 explosionCenterOffset = new Vector3(0f, -0.5f, 0f);

    private const uint FlagExplosioning = 1;

    private FracturePart[] m_parts;
    private List<int> m_explosionLittleUsedIndices;

    private bool m_isInited;

    public bool isExplosioning { get; private set; }


    public void Explosion (Transform parent = null, float destroySelfDuration = 3f, float scaleDuration = -1f, float fragmentEndLocalScaleMultiplier = 0.1f) {
        if (isExplosioning) return;
        isExplosioning = true;

        gameObject.SetActive(true);

        Init();

        if (parent) {
            transform.SetParent(parent, true);
        }

        Vector3 explosionPosition = transform.position;
        explosionPosition += explosionCenterOffset;
        for (int i = 0; i < m_parts.Length; i++) {
            FracturePart part = m_parts[i];
            float explosionForce = Random.Range(explosionForceRange.min, explosionForceRange.max);
            float explosionRadius = Random.Range(explosionRadiusRange.min, explosionRadiusRange.max);
            part.ApplyExplosionForce(explosionForce, explosionPosition, explosionRadius, scaleDuration, fragmentEndLocalScaleMultiplier);
        }

        Invoke(nameof(DestroySelf), destroySelfDuration);
    }

    public void ExplosionLittle (Transform parent = null) {
        Init();

        gameObject.SetActive(true);

        if (parent) {
            transform.SetParent(parent, true);
        }

        if (m_explosionLittleUsedIndices == null) {
            m_explosionLittleUsedIndices = new List<int>();
        }
        Vector3 explosionPosition = transform.position;

        // 准备未爆炸的碎片列表
        List<int> partIndices = new List<int>();
        for (int i = 0, len = m_parts.Length; i < len; i++) {
            if (m_explosionLittleUsedIndices.Contains(i)) continue;
            partIndices.Add(i);
        }

        // 从未爆炸碎片列表中随机取几个(注意此列表中的元素是 partIndices 的索引)
        int[] randomIndices = RandomUtil.GetRandomUniqueIntList(0, partIndices.Count, Random.Range(2, 4));

        FracturePart[] littleParts = new FracturePart[randomIndices.Length];
        for (int i = 0, len = randomIndices.Length; i < len; i++) {
            int index = partIndices[randomIndices[i]];
            FracturePart part = m_parts[index];
            float explosionForce = Random.Range(explosionForceRange.min, explosionForceRange.max);
            float explosionRadius = Random.Range(explosionRadiusRange.min, explosionRadiusRange.max);
            part.ApplyExplosionForce(explosionForce, explosionPosition, explosionRadius);
            littleParts[i] = part;
            m_explosionLittleUsedIndices.Add(index);
        }

        StartCoroutine(DisableLittleFragments(littleParts));
    }

    IEnumerator DisableLittleFragments (FracturePart[] littleParts) {
        yield return new WaitForSeconds(3f);
        int i = littleParts.Length;
        while (--i >= 0) {
            littleParts[i].gameObject.SetActive(false);
        }

        bool isAllParttsDisabled = true;
        i = m_parts.Length;
        while (--i >= 0) {
            if (m_parts[i].gameObject.activeSelf) {
                isAllParttsDisabled = false;
                break;
            }
        }
        if (isAllParttsDisabled) {
            DestroySelf();
        }
    }

    private void DestroySelf () {
        Destroy(gameObject);
    }

    private void Init () {
        if (m_isInited) return;
        m_isInited = true;

        m_parts = GetComponentsInChildren<FracturePart>();
    }

    private void Awake () {
        Init();
    }
}