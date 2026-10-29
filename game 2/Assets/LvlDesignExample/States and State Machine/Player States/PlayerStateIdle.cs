using UnityEngine;

//Idle State for the Player
public class PlayerStateIdle : PlayerState
{
    //Constants
    

    //Variables

    public override void Enter(StateMachine state_machine, Player player)
    {
        //Debug.Log("Idle State");
        player.model.GetComponent<MeshRenderer>().material.color = Color.gray;
        player.model.transform.GetChild(0).GetComponent<MeshRenderer>().material.color = Color.gray;
    }

    public override void Exit(StateMachine state_machine, Player player)
    {

    }

    public override void Update(StateMachine state_machine, Player player)
    {
        //Read values from input.
        player.direction_input = player.move_action.ReadValue<Vector2>();
    }
    public override void FixedUpdate(StateMachine state_machine, Player player)
    {

        //Change States
        #region Ground
        if (player.direction_input != Vector2.zero) //Player pressed a move key.
        {
            state_machine.SwitchState(this, state_machine.state_ground);
            return;
        }

        #endregion

        #region Air
        if (!player.controller.isGrounded) //Not being on the ground.
        {
            state_machine.SwitchState(this, state_machine.state_air);
            return;
        }

        if (player.wants_to_jump) //Jump
        {
            player.controller.Move(Vector3.up * state_machine.state_air.jump_boost * Time.fixedDeltaTime);
            player.velocity_current = player.controller.velocity;
            player.velocity_xz = new Vector3(player.controller.velocity.x, 0, player.controller.velocity.z);
            player.velocity_y = player.controller.velocity.y;
            state_machine.SwitchState(this, state_machine.state_air);
            return;
        }
        #endregion
    }
}
