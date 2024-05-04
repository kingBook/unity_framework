using UnityEngine;

public class StateGameLevel : State {

    public static readonly StateGameLevel instance = new StateGameLevel();


    protected override void OnStateEnter(Fsm fsm) {
        base.OnStateEnter(fsm);
       ((FsmGame)fsm).game.GotoLevelScene(1);
    }

    protected override void OnStateUpdate(Fsm fsm) {
        base.OnStateUpdate(fsm);
    }

    protected override void OnStateExit(Fsm fsm) {
        base.OnStateExit(fsm);
    }
}