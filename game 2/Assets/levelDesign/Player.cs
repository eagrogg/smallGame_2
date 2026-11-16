using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    //Consts variable that should never change throughout code. They are constant.
    //These variables belong to the class. In order to access them we would need ot say Player.SLOPE_WALKABLE.
    //We usually denote them will all capital letters.
    public const float SLOPE_WALKABLE = 45f; //Angle of a slope that can be walked on by the player.
    public const float SLOPE_SLIDEABLE = 85f; //Angle of a slope that the player can slide on.

    //Inputs
    [HideInInspector] public InputAction move_action;
    [HideInInspector] public InputAction jump_action;
    [HideInInspector] public InputAction crouch_action;

    //Components
    //The model that represents the player character.
    public GameObject model;
    [HideInInspector] public CharacterController controller;
    public Transform cam;

    //A curve showing the arc of the player's velocity in the air.
    //public AnimationCurve jump_curve;

    //Variables
    //The direction supplied by the users inputs.
    [HideInInspector] public Vector2 direction_input = Vector2.zero;
    //The final direction the player should be moving in.
    [HideInInspector] public Vector3 direction_final = Vector2.zero;
    //The direction the player is currently traveling in.
    [HideInInspector] public Vector3 direction_current = Vector3.zero;
    //Velocity along the xz plane.
    [HideInInspector] public Vector3 velocity_xz = Vector3.zero;
    //Velocity along the y plane.
    [HideInInspector] public float velocity_y = 0;
    //The combination of the xz and y velocities.
    [HideInInspector] public Vector3 velocity_current = Vector3.zero;

    //Variable that is used to tell if the player the the user has intiated a jump.
    [HideInInspector] public bool wants_to_jump = false;
    [HideInInspector] public float jump_timer = 0;
    private float jump_buffer_time = 0.1f; //NOTE: Doesnt work for values lower than 0.1. They are too small of a time and inputs will be dropped.

    [HideInInspector] public bool is_crouching;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //Hide Mouse Cursor
        Cursor.lockState = CursorLockMode.Locked;

        //Find Input Actions
        move_action = InputSystem.actions.FindAction("Move");
        jump_action = InputSystem.actions.FindAction("Jump");
        crouch_action = InputSystem.actions.FindAction("Sprint"); //Make crouch

        //Get Components
        controller = GetComponent<CharacterController>();

        controller.slopeLimit = SLOPE_WALKABLE;
    }

    // Update is called once per frame
    void Update()
    {
        #region Read Input
        direction_input = move_action.ReadValue<Vector2>();
        is_crouching = crouch_action.IsPressed();

        #region Jump Input
        //Jumping is a bit strange. I originally wanted jumping to work on the frame the user pressed the key but that didnt always trigger. So I went with buffering the jump input instead.
        //To buffer something means that the user can perform some input and the game will allow that input to be valid for a period of time, even if the user has stopped supplying the input.
        //First we check to see if the jump_action was performed and the jump time is <= 0.
        if (jump_action.WasPerformedThisFrame() && jump_timer <= 0)
        {
            //If so, we tell the player that the user wants to jump and start the time.
            wants_to_jump = true;
            jump_timer = jump_buffer_time;
        }
        //We count the timer down.
        if (jump_timer > 0)
        {
            jump_timer -= Time.fixedDeltaTime;
        }
        else
        {
            //Then once the time has, we set wants to jump to false.
            wants_to_jump = false;
        }
        //The benfit to doing jumping this way is that there is a window of time where wants_to_jump will be accepted by the player. So jumping is no longer frame perfect.
        //I do wish there was a way to do exactly what I wanted but and it might be possible with the new input system but im still learning all the ins and outs of it.
        #endregion

        #endregion
    }
}
