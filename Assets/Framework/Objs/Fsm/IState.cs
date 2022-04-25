/// <summary> 状态接口 </summary>
public interface IState {

    void OnStateEnter(Fsm fsm);

    void OnStateFixedUpdate(Fsm fsm);

    void OnStateUpdate(Fsm fsm);

    void OnStateLateUpdate(Fsm fsm);

    void OnStateExit(Fsm fsm);

}
