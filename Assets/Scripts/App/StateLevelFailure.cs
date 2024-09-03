public class StateLevelFailure : StateLevelEnd {
    
    protected override void OnStateEnter(Fsm fsm) {
        base.OnStateEnter(fsm);
    }

    protected override void OnStateFixedUpdate(Fsm fsm) {
        base.OnStateFixedUpdate(fsm);
    }

    protected override void OnStateUpdate(Fsm fsm) {
        base.OnStateUpdate(fsm);
    }

    protected override void OnStateLateUpdate(Fsm fsm) {
        base.OnStateLateUpdate(fsm);
    }

    protected override void OnStateExit(Fsm fsm) {
        base.OnStateExit(fsm);
    }
}