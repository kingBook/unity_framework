using UnityEngine;

/// <summary>
/// 游戏类:
/// <para> 管理游戏全局变量、本地数据、场景切换. </para>
/// <para> 通过 <c> App.instance.game </c> 访问 </para>
/// </summary>
public sealed class Game : MonoBehaviour {

    /// <summary> 游戏单例 </summary>
    public static Game instance { get; private set; }

    /// <summary> 游戏状态机 </summary>
    public GameFsm fsm { get; private set; }

    private void Awake() {
        instance = this;

        // 初始化游戏状态机
        fsm = GameObjectUtil.AddNewChildAndComponentToNode<GameFsm>(gameObject);
        fsm.AddState<StateGameTitle>();
        fsm.AddState<StateGameLevel>();
        fsm.Init();
        // 切换到游戏标题状态
        fsm.ChangeStateTo(nameof(StateGameTitle));
    }

    private void OnDestroy() {
        instance = null;
    }


}
