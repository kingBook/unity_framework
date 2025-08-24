using UnityEngine;

/// <summary>
/// 游戏类:
/// <para> 管理游戏全局变量、本地数据、场景切换. </para>
/// <para> 通过 <c> App.instance.game </c> 访问 </para>
/// </summary>
public sealed class Game : MonoBehaviour {

    public static Game instance { get; private set; }

    public GameFsm fsm { get; private set; }

    private void Awake() {
        instance = this;
        fsm = GameObjectUtil.addChildAndComponentToNode<GameFsm>(gameObject);
    }

    private void OnDestroy() {
        instance = null;
    }


}
