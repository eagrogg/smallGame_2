using UnityEngine;
using UnityEngine.InputSystem;


public class playerMovement : MonoBehaviour
{
    //components
    private Rigidbody rb;
    
    //movement variables
    private InputAction move_action;
    private InputAction roll_action;
    private Vector3 direction = Vector3.zero;
    private Vector3 velocity = Vector3.zero;
    public float speed = 5f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //assign rigidbody component
        rb = GetComponent<Rigidbody>();

        //get movement actions
        move_action = InputSystem.actions.FindAction("move");
        roll_action = InputSystem.actions.FindAction("roll");
    }

    // Update is called once per frame
    void Update()
    {
        direction.x = move_action.ReadValue<Vector2>().x;
        direction.z = move_action.ReadValue<Vector2>().y;
    }

    void FixedUpdate()
    {
        //velocity vector
        velocity = direction * speed;
        velocity *= Time.fixedDeltaTime;
        //move
        rb.MovePosition(rb.position + velocity);

    }
}
