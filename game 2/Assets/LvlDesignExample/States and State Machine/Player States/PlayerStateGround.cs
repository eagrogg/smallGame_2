using UnityEngine;

//Ground State for the Player
public class PlayerStateGround : PlayerState
{
    //Constants
    const float GROUND_SPEED = 10;

    //Variables
    float acceleration = 42f;
    float deceleration = 30f;
    float gravity = -11f;

    public override void Enter(StateMachine state_machine, Player player)
    {
        //Debug.Log("Ground State");
        player.model.GetComponent<MeshRenderer>().material.color = Color.green;
        player.model.transform.GetChild(0).GetComponent<MeshRenderer>().material.color = Color.green;

        //Set y velocity to a negative value so the character controller will be able to know it is on the ground.
        //This is the force keeping the controller on the ground. Lower values result in the controller hopping down slopes rather than sliding.
        player.velocity_y = gravity;
    }

    public override void Exit(StateMachine state_machine, Player player)
    {

    }

    public override void Update(StateMachine state_machine, Player player)
    {
        //Player is pressing a move key...
        if (player.direction_input != Vector2.zero) 
        {
            //Target_Angle is the angle between 0-360 that we want the player to eventually move in.
            float target_angle = (Mathf.Atan2(player.direction_input.x, player.direction_input.y) * Mathf.Rad2Deg) + player.cam.transform.eulerAngles.y;
            //Atan2 retyurns the angle between the direction of our input and Vector2.Right.
            //We then convert that angle into degrees.
            //Then we add it to the camera's current rotation.

            //Get the final direction by turing the angle into a quaternion and multiplying it by the forward direction.
            player.direction_final = Quaternion.Euler(0f, target_angle, 0f) * Vector3.forward;
            player.direction_final = player.direction_final.normalized;

            //Make the current direction move to the final direction.
            player.direction_current = Vector3.Lerp(player.direction_current, player.direction_final, 8f * Time.deltaTime);

            //Make the player model face in the current direction.
            player.model.transform.forward = player.direction_current;

            //Accelerate the player by moving thier velocity_xz towards direction_current * MAX_SPEED.
            //NOTE: Mathf.MoveTowards() is a great way to blend between values.
            player.velocity_xz = Vector3.MoveTowards(player.velocity_xz, player.direction_current * GROUND_SPEED, acceleration * Time.deltaTime);
        }
        //Player is not holding a key down...
        else
        {
            //Decelerate the player by moving thier velocity_xz Vector3.zero.
            player.velocity_xz = Vector3.MoveTowards(player.velocity_xz, Vector3.zero, deceleration * Time.deltaTime);
        }

        //Set the xz velocity of the player.
        //player.SetXZVel(GROUND_SPEED, acceleration, deceleration);

        //Combine velocities. (This applies gravity.)
        player.velocity_current = player.velocity_xz + new Vector3(0, gravity, 0);
    }
    public override void FixedUpdate(StateMachine state_machine, Player player)
    {
        //NOTE: The player needs to change states at different sections in the code.
        //Sometimes it will be before the states Move() and sometimes it will be after.
        //It depends on when and how we react to the move.
        #region Change to Air State via Jumping
        if (player.wants_to_jump) //Jump
        {
            //Make the player move upward.
            player.controller.Move(new Vector3(player.controller.velocity.x, state_machine.state_air.jump_boost, player.controller.velocity.z) * Time.fixedDeltaTime);
            //Sync velocities to the move.
            player.velocity_current = player.controller.velocity;
            player.velocity_xz = new Vector3(player.controller.velocity.x, 0, player.controller.velocity.z);
            player.velocity_y = player.controller.velocity.y;

            //Switch State
            state_machine.SwitchState(this, state_machine.state_air);

            //Return since we dont need to move again.
            return; 
        }
        #endregion

        //Move the player.
        player.controller.Move(player.velocity_current * Time.fixedDeltaTime);

        //Sync velocities to the controller's velocities after the move.
        //Since the rigidbody moved, we need to update the internal velocities we are using to move the player.
        player.velocity_xz = new Vector3(player.controller.velocity.x, 0, player.controller.velocity.z); //Only need to do xz since thats really the only one that matters.

        #region Change to Air State via Moving Off the Ground
        //NOTE: IsGrounded only updates after the controller has moved.
        if (!player.controller.isGrounded) 
        {
            //Since we fell off a ledge, we want to cancel the downward force of gravity.
            //We do this by just moving up by -gravity and setting the y velocity to 0.
            player.controller.Move(new Vector3(0, -gravity * Time.fixedDeltaTime, 0));
            player.velocity_y = 0;
            state_machine.SwitchState(this, state_machine.state_air);
            return;
        }
        #endregion

        //Player has fully stopped moving along xz plane.
        #region Change to Idle State
        if (player.velocity_xz == Vector3.zero) 
        {
            state_machine.SwitchState(this, state_machine.state_idle);
            return;
        }
        
        #endregion
    }
}
