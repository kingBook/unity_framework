public interface IState {
    
    void OnStateEnter(Fsm fsm);

    void OnStateFixedUpdate();

    void OnStateUpdate();

    void OnStateLateUpdate();
    
    void OnStateExit();
}