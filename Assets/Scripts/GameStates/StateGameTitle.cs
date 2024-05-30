using UnityEngine.SceneManagement;

public class StateGameTitle : State {

    public override void OnStateEnter(Fsm fsm) {
        App.instance.sceneLoader.Load("Scenes/Title");
    }

    public override void OnStateUpdate(Fsm fsm) {

    }

    public override void OnStateExit(Fsm fsm) {
        SceneManager.UnloadSceneAsync("Scenes/Title");
    }
}