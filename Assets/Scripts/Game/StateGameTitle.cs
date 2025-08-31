using UnityEngine;
using UnityEngine.SceneManagement;

public class StateGameTitle : State {

    protected override void OnStateEnter(Fsm fsm) {
        App.instance.sceneLoader.LoadAsync("Scenes/Title", false);
    }

    protected override void OnStateUpdate() {
        
    }

    protected override void OnStateExit() {
        SceneManager.UnloadSceneAsync("Scenes/Title");
    }
}