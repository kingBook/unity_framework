public class StateLevelFailure : StateLevelEnd {
    
    protected override void OnStateEnter(Fsm fsm) {
        base.OnStateEnter(fsm);
    }

    protected override void OnStateFixedUpdate() {
        base.OnStateFixedUpdate();
    }

    protected override void OnStateUpdate() {
        base.OnStateUpdate();
    }

    protected override void OnStateLateUpdate() {
        base.OnStateLateUpdate();
    }

    protected override void OnStateExit() {
        base.OnStateExit();
    }
}