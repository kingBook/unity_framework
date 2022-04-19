﻿/// <summary> 最简单、无任何实现的状态 </summary>
public class State : IState {

    void IState.OnStateEnter(Fsm fsm) {
        OnStateEnter(fsm);
    }

    void IState.OnStateUpdate(Fsm fsm) {
        OnStateUpdate(fsm);
    }

    void IState.OnStateExit(Fsm fsm) {
        OnStateExit(fsm);
    }


    protected virtual void OnStateEnter(Fsm fsm) {
    }

    protected virtual void OnStateUpdate(Fsm fsm) {
    }

    protected virtual void OnStateExit(Fsm fsm) {
    }


}