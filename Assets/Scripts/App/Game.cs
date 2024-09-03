/// <summary>
/// 游戏类:
/// <para> 管理游戏全局变量、本地数据、场景切换. </para>
/// <para> 可以通过 <see cref="SetCurrentLevel(Level)"/> 方法设置持有当前关卡的引用，但不访问关卡内的对象. </para>
/// <para> 可以通过 <c> App.instance.fsm.GetCurrentState&lt;Game&gt;() </c> 访问实例 </para>
/// </summary>
public sealed class Game : State {


    public GameFsm gameFsm { get; private set; }
    public Level currentLevel { get; private set; }


    public void SetCurrentLevel(Level level) {
        currentLevel = level;
    }
    
    protected override void OnStateEnter(Fsm fsm) {
        this.gameFsm = Fsm.Create<GameFsm>(gameObject);
        this.gameFsm.Init(this);
        this.gameFsm.ChangeStateTo(nameof(StateGameTitle));
    }

    protected override void OnStateExit(Fsm fsm) {
        
    }


}