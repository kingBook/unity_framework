using System.Collections;
using UnityEngine;

[System.Serializable]
public abstract class StateBase {

    public abstract void OnStateEnter (Fsm fsm);

    public abstract void OnStateUpdate (Fsm fsm);

    public abstract void OnStateExit (Fsm fsm);

}
