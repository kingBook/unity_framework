public class StateGameTitle : State {
    public static readonly StateGameTitle instance = new StateGameTitle();
    
    protected override void OnStateEnter(Fsm fsm) {
        base.OnStateEnter(fsm);
        ((FsmGame)fsm).game.GotoTitleScene();
    }

    protected override void OnStateUpdate(Fsm fsm) {
        base.OnStateUpdate(fsm);
    }

    protected override void OnStateExit(Fsm fsm) {
        base.OnStateExit(fsm);
    }
}