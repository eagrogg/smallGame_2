using UnityEngine;
/*
 * States are read by the state machine.
 * A state determines how the game object will behave.
*/
public abstract class State
{
    //Method for when the state machine transitions to a new state.
    public virtual void Enter(StateMachine state_machine, Player player)
    {
        
    }

    //Method for when the state machine exits the current state.
    public virtual void Exit(StateMachine state_machine, Player player)
    {

    }

    //Runs during the state machine's Update() method.
    public virtual void Update(StateMachine state_machine, Player player)
    {

    }

    //Runs during the state machine's FixedUpdate() method.
    public virtual void FixedUpdate(StateMachine state_machine, Player player)
    {

    }

    //Runs during the state machine's OnControllerColliderHit() method.
    public virtual void OnControllerColliderHit(ControllerColliderHit hit, StateMachine state_machine, Player player)
    {

    }
}
