public class FsmApp : Fsm {

    private void Start() {
        AddState<Game>();
        Init();
        
        ChangeStateTo(nameof(Game));
    }


}