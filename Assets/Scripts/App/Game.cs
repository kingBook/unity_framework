using UnityEngine;

/// <summary>
/// 游戏类:
/// <para> 管理游戏全局变量、本地数据、场景切换. </para>
/// <para> 可以通过 <code> App.instance.fsm.GetCurrentState&lt;Game&gt;() </code> 访问实例 </para>
/// </summary>
public sealed class Game : State {


    public GameFsm fsm { get; private set; }

    protected override void OnStateEnter(Fsm fsm) {
        this.fsm = GameObjectUtil.AddNodeComponent<GameFsm>(gameObject);
    }

    protected override void OnStateExit(Fsm fsm) {

    }


}
