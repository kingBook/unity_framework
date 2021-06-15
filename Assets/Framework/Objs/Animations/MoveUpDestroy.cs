using UnityEngine;
using System.Collections;
using DG.Tweening;

/// <summary>
/// 向上移动一定的距离后销毁
/// </summary>
public class MoveUpDestroy : MonoBehaviour {

    [Tooltip("向上移动的距离"), Range(10, 100), SerializeField]
    private float m_moveUpDistance = 40;

    [Tooltip("向上移动的持续时间"),]
    private float m_duration = 1.0f;

    private void Start () {
        transform.DOMoveY(transform.position.y + m_moveUpDistance, m_duration).onComplete = () => {
            Destroy(gameObject);
        };
    }

    private void OnDestroy () {
        transform.DOKill();
    }
}
