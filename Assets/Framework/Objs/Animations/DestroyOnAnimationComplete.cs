#pragma warning disable 0649

using UnityEngine;
using System.Collections;

/// <summary>
/// 在动画播放后销毁一个对象 <br/>
/// 注意：绑定此脚本的游戏必须有 Animator 组件，将在 Animator 组件动画剪辑列表[0]添加动画完成事件
/// </summary>
[RequireComponent(typeof(Animator))]
public class DestroyOnAnimationComplete : MonoBehaviour {

    [SerializeField] private GameObject m_destroyOnComplete;

    private Animator m_animator;

    private void OnComplete (UnityEngine.Object objectReference) {
        if (objectReference != this) return;
        if (m_destroyOnComplete) {
            Destroy(m_destroyOnComplete);
        }
    }

    private void Awake () {
        m_animator = GetComponent<Animator>();
    }

    private void Start () {
        AnimatorClipInfo[] animatorClipInfos = m_animator.GetCurrentAnimatorClipInfo(0);
        AnimationClip animationClip = animatorClipInfos[0].clip;
        AnimationEvent animationEvent = new AnimationEvent {
            objectReferenceParameter = this,
            functionName = nameof(OnComplete),
            time = animationClip.length
        };
        animationClip.AddEvent(animationEvent);
    }

#if UNITY_EDITOR
    private void Reset () {
        if (m_destroyOnComplete == null) {
            m_destroyOnComplete = gameObject;
        }
    }
#endif

}