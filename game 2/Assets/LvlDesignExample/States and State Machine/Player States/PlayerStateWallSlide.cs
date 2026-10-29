using UnityEngine;

//WallSlide State for the Player
public class PlayerStateWallSlide : PlayerState
{
    //Constants
    const float SLIDE_SPEED = -5;
    const float JUMP_BOOST = 27; //Force the player jumps off the wall with.
    const float JUMP_ANGLE = 55; //The angle at which the player jumps off the wall.

    //Variables
    //The normal of the wall the player is on.
    [HideInInspector] public Vector3 wall_normal;
    //The point in which the palyer collided with the wall.
    [HideInInspector] public Vector3 wall_point;
    float gravity = 14f; //Must be positive.

    public override void Enter(StateMachine state_machine, Player player)
    {
        //Debug.Log("Wall Slide State");
        player.model.GetComponent<MeshRenderer>().material.color = Color.red;
        player.model.transform.GetChild(0).GetComponent<MeshRenderer>().material.color = Color.red;

        //Face the player away from the wall.
        player.direction_current = wall_normal;
        player.model.transform.forward = wall_normal;
    }

    public override void Exit(StateMachine state_machine, Player player)
    {
        
    }

    public override void Update(StateMachine state_machine, Player player)
    {
        //Set the xz velocity of the player.
        //set_xz_vel(player, AIR_SPEED, acceleration, deceleration);

        //Give the player a gravity multiplier based on their y velocity.
        //This prevent the player from sliding too fast up/down a wall do to the reduced gravity.
        float grav_multi = Mathf.Max(1, Mathf.Abs(player.velocity_y) / 5.3f); //5.3 is a magic number. Just went with something that felt right.

        //Gravity
        player.velocity_y = Mathf.MoveTowards(player.velocity_y, SLIDE_SPEED, gravity * grav_multi * Time.deltaTime);

        //Combine velocities.
        player.velocity_current = new Vector3(0, player.velocity_y, 0);
    }
    public override void FixedUpdate(StateMachine state_machine, Player player)
    {
        #region Change to Air State via Jumping Off Wall
        if (player.wants_to_jump)
        {
            //Here's a good use of using the cross product get the direction the player will jump off the wall.
            //Firstly, the cross product is an operation on two vectors that outputs a third vector that is perpendicular to the two input vectors.
            //In order to find the direction we want the player to jump in, we find the axis that we will rotate the wall normal around.
            //We want to rotate the wall normal since it serves as a good base direction to use.
            //So on the line below, we cross the wall_normal with the up vector get the the axis.
            Vector3 axis = Vector3.Cross(wall_normal, Vector3.up);
            //We then use the Quaternion.AngleAxis() function to rotate the wall_normal about the axis.
            //I think it is best to read the line below backwards, we rotate 'wall_normal' about the axis 'axis' by JUMP_ANGLE.
            Vector3 wall_jump_dir = Quaternion.AngleAxis(JUMP_ANGLE, axis) * wall_normal;

            //Move player with wall jump and update internal velocities.
            player.controller.Move(wall_jump_dir * JUMP_BOOST * Time.fixedDeltaTime);
            player.velocity_current = player.controller.velocity;
            player.velocity_xz = new Vector3(player.controller.velocity.x, 0, player.controller.velocity.z);
            player.velocity_y = player.controller.velocity.y;

            //Change states.
            state_machine.SwitchState(this, state_machine.state_air);
            return;
        }
        #endregion

        //Move
        player.controller.Move(player.velocity_current * Time.fixedDeltaTime);

        //Update velocities after the move.
        player.velocity_y = player.controller.velocity.y;

        #region Change to Ground State
        if (player.controller.isGrounded && player.velocity_y < 0.5f)
        {
            state_machine.SwitchState(this, state_machine.state_ground);
            return;
        }
            
        #endregion

        #region Change to Air State via Not Touching the Wall
        //Raycast behind the player to see if they have fallen off the wall.
        Ray ray = new Ray(player.transform.position, -player.direction_current.normalized);
        float dist = player.controller.radius + player.controller.skinWidth + 0.02f;
        if (!Physics.Raycast(ray, dist, 64)) //Layer mask is 64. This corrisponds to the world layer.
        {
            state_machine.SwitchState(this, state_machine.state_air);
            return;
        }
        #endregion
    }
}
