using UnityEngine;

//Script that controls the switching between different player states.
public class StateMachine : MonoBehaviour
{
    //The current state the state machine is in.
    private PlayerState state_current;
    //The previous state the palyer was in.
    //[HideInInspector] public PlayerState state_previous;

    //States for the player.
    [HideInInspector] public PlayerStateIdle state_idle = new PlayerStateIdle();
    [HideInInspector] public PlayerStateGround state_ground = new PlayerStateGround();
    [HideInInspector] public PlayerStateAir state_air = new PlayerStateAir();
    [HideInInspector] public PlayerStateWallSlide state_wall_slide = new PlayerStateWallSlide();
    //[HideInInspector] public PlayerStateGroundSlide state_ground_slide = new PlayerStateGroundSlide();

    //Reference to player.
    private Player player;

    //Debug
    //public TMP_Text debug_text;

    // Start is called before the first frame update
    void Start()
    {
        //Get Components
        player = GetComponent<Player>();

        //Set the starting states for the player.
        state_current = state_idle;
        //state_previous = state_current;

        //Enter the current state.
        state_current.Enter(this, player);
    }

    // Update is called once per frame
    void Update()
    {
        //return;
        state_current.Update(this, player);
    }

    private void FixedUpdate()
    {
        //return;
        state_current.FixedUpdate(this, player);
    }

    //OnControllerColliderHit is called when the controller hits a collider while performing a Move.
    //If we hit multiple colliders at the same time, the function will for each collider that was hit
    //The hit information returns information about the collision.
    void OnControllerColliderHit(ControllerColliderHit hit)
    {
        state_current.OnControllerColliderHit(hit, this, player);
    }

    public void SwitchState(PlayerState current_state, PlayerState new_state)
    {
        //Exit the current state.
        current_state.Exit(this, player);

        //Set the previous state to the current state.
        //state_previous = current_state;

        //Change the current state to the new state.
        state_current = new_state;

        //Enter the new state.
        state_current.Enter(this, player);
    }

}
