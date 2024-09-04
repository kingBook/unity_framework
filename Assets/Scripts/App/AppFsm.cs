public class AppFsm : Fsm {

    private void Start() {
        AddState<Game>();
        Init();

        ChangeStateTo(nameof(Game));
    }


}