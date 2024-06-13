using UnityEngine;
using UnityEngine.SceneManagement;

public class StateGameLevel : State {


    private string m_sceneName;

    public int levelNumber { get; private set; }

    public void SetLevelNumber(int value) {
        levelNumber = value;
    }

    protected override void OnStateEnter(Fsm fsm) {
        m_sceneName = "Scenes/Level_0";
        App.instance.sceneLoader.LoadAsync(m_sceneName);
    }

    protected override void OnStateUpdate(Fsm fsm) {

    }

    protected override void OnStateExit(Fsm fsm) {
        SceneManager.UnloadSceneAsync(m_sceneName);
    }

}