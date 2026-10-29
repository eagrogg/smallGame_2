using UnityEngine;

//Air State for the Player
public class PlayerStateAir : PlayerState
{
    //Constants
    const float AIR_SPEED = 10;
    const float FALL_SPEED = -30; //Must be negative.

    //Variables
    float acceleration = 42f;
    float deceleration = 6f;
    public float jump_boost = 23f;
    float gravity = 50f; //Must be positive.

    public override void Enter(StateMachine state_machine, Player player)
    {
        //Debug.Log("Air State");
        player.model.GetComponent<MeshRenderer>().material.color = Color.blue;
        player.model.transform.GetChild(0).GetComponent<MeshRenderer>().material.color = Color.blue;
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

            //Accelerate the player by moving thier velocity_xz towards direction_current * SPEED.
            //NOTE: Mathf.MoveTowards() is a great way to blend between values.
            player.velocity_xz = Vector3.MoveTowards(player.velocity_xz, player.direction_current * AIR_SPEED, acceleration * Time.deltaTime);
        }
        //Player is not holding a key down...
        else
        {
            //Decelerate the player by moving thier velocity_xz Vector3.zero.
            player.velocity_xz = Vector3.MoveTowards(player.velocity_xz, Vector3.zero, deceleration * Time.deltaTime);
        }

        //Gravity
        player.velocity_y = Mathf.MoveTowards(player.velocity_y, FALL_SPEED, gravity * Time.deltaTime);

        //Combine velocities.
        player.velocity_current = player.velocity_xz + new Vector3(0, player.velocity_y, 0);
    }
    public override void FixedUpdate(StateMachine state_machine, Player player)
    {
        //Move the player.
        player.controller.Move(player.velocity_current * Time.fixedDeltaTime);

        //Update velocities after the move.
        player.velocity_xz = new Vector3(player.controller.velocity.x, 0, player.controller.velocity.z);
        player.velocity_y = player.controller.velocity.y;

        #region Change to Ground State
        if (player.controller.isGrounded && player.velocity_y < 0.5f)
        {
            state_machine.SwitchState(this, state_machine.state_ground);
            return;
        }
        #endregion
    }
    public override void OnControllerColliderHit(ControllerColliderHit hit, StateMachine state_machine, Player player)
    {
        //Change to WallSlide State.
        //OnControllerColliderHit runs when the character controller hits a collider during the move.
        //In this time we can get information about the hit using the 'hit' parameter that is passed into the function.

        //To see if we touched a wall, take the dot product. 
        float dot = Vector3.Dot(hit.normal, Vector3.up);
        //NOTE: Both vectors used for the dot product are unit vectors, meaning they have a length of one. They dont always need to be but it can make other calculations involving the dot product easier to understand.
        //The dot product allows us to compare the direction of two vectors.
        //There are three outcomes of this comparison:
        //If the vectors face the same direction, the result is positive.
        //If they face the opposite direction, the result is negative.
        //If they are perpendicular, the result is zero.
        //In our case, we are comparing the normal of what we hit to the up vector.
        //A normal is the direction a face is pointing.
        //We will be using the up vector to represent flat ground and we will define a wall as something that is perpendicular to the up vector.
        //This means that are wall needs to be perfectly straight up and down.
        //So that means that the dot product between the two will need to be zero.

        //So we can just do if(dot == 0.0f) right?
        //No unfortunately, we run into something called floating point errors.
        //Basically very small numbers accumulate when ever we do a calculations.
        //So even though our dot should be 0.0f it might actually be 0.00000000000003f.

        //To remidy this, we just need to account for that error by comparing our calculation against an 'epsilon' value.
        //An epsilon value is how much error we will tolerate in our calculations.
        //In the if statement below we take the absolute value of the difference between our dot product and the value we want, that 0.0f.
            //Although it is funny to subtract zero since it does nothing.
        //This gives us the error in our calculations.
        //We can see if the error is at or below our epsilon value, 0.0015f, and if so we will change states.
        if (Mathf.Abs(dot - 0.0f) <= 0.0015f) //NOTE: I will say this does seem to fail sometimes. So I will need to find a better way to do things.
        {

            //Pass info to the wall slide state.
            state_machine.state_wall_slide.wall_normal = hit.normal;
            state_machine.state_wall_slide.wall_point = hit.point;

            //Change states
            state_machine.SwitchState(this, state_machine.state_wall_slide);
        }
        //I just want to add a little more comments to discuss some other things I figured out while writing the code above.

        //First, The Mathf class has a Approximately() function that checks two floats and sees if they are approximately eqaul.
        //My issues with this function is that it uses Unity's default epsilon value. Which is very very small, meaning we need to be really percise.
        //In practiced this worked about 50% of the time.
        //We can change Unity's epsilon value but it makes it program wide and you might want to have different epsilon values based on what you are trying to do.

        //Second, if you ever want to get the angle between the two vectors you used for the dot product you can use Mathf.Acos(dot).
        //It is important to note that this only works if both vectors are unit vectors.
    }
}
