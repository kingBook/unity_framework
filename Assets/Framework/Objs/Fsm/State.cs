using UnityEngine;

/// <summary>
/// 最简单、无任何实现的状态 (继承MonoBehaviour使子类能使用Invoke、StartCoroutine等函数)
/// </summary>
public class State : MonoBehaviour, IState {

    protected virtual void OnStateEnter(Fsm fsm) {
    }

    protected virtual void OnStateFixedUpdate(Fsm fsm) {
    }

    protected virtual void OnStateUpdate(Fsm fsm) {
    }

    protected virtual void OnStateLateUpdate(Fsm fsm) {
    }

    protected virtual void OnStateExit(Fsm fsm) {
    }

    // ‘显式’方式，实现 IState 接口的方法，又用 protected 进行封装，使用各方法在子类中不公开

    void IState.OnStateEnter(Fsm fsm) {
        OnStateEnter(fsm);
    }

    void IState.OnStateFixedUpdate(Fsm fsm) {
        OnStateFixedUpdate(fsm);
    }

    void IState.OnStateUpdate(Fsm fsm) {
        OnStateUpdate(fsm);
    }

    void IState.OnStateLateUpdate(Fsm fsm) {
        OnStateLateUpdate(fsm);
    }

    void IState.OnStateExit(Fsm fsm) {
        OnStateExit(fsm);
    }
}