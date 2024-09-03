using UnityEngine.SceneManagement;

public class StateGameTitle : State {

    protected override void OnStateEnter(Fsm fsm) {
        App.instance.sceneLoader.Load("Scenes/Title");
    }

    protected override void OnStateUpdate(Fsm fsm) {
        
    }

    protected override void OnStateExit(Fsm fsm) {
        SceneManager.UnloadSceneAsync("Scenes/Title");
    }
}