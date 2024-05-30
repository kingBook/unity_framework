using UnityEngine;

/// <summary> 最简单、无任何实现的状态 (继承MonoBehaviour使子类能使用Invoke、StartCoroutine等函数)</summary>
public class State : MonoBehaviour {

    public virtual void OnStateEnter(Fsm fsm) {
    }

    public virtual void OnStateFixedUpdate(Fsm fsm) {
    }

    public virtual void OnStateUpdate(Fsm fsm) {
    }

    public virtual void OnStateLateUpdate(Fsm fsm) {
    }

    public virtual void OnStateExit(Fsm fsm) {
    }


}