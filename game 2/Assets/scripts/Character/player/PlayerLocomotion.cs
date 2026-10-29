using UnityEngine;

public class PlayerLocomotion : CharacterLocomotion
{
    PlayerManager player;

    public float verticalMovement;
    public float horizontalMovement;
    public float moveAmount;

    private Vector3 moveDirection;
    private Vector3 targetRotationDir;
    [SerializeField] float walkSpeed = 2;
    [SerializeField] float runSpeed = 5;
    [SerializeField] float rotationSpeed = 15;

    protected override void Awake()
    {
        base.Awake();

        player = GetComponent<PlayerManager>();
    }

    public void HandleMovement()
    {
        HandleGroundedMovement();
        HandleRotation();
    }

    private void GetVerticalAndHorizontalInputs()
    {
        verticalMovement = PlayerInputManager.instance.verticalInput;
        horizontalMovement = PlayerInputManager.instance.horizontalInput;

        //clamp em
    }

    private void HandleGroundedMovement()
    {
        GetVerticalAndHorizontalInputs();
        //move direction is based on camera's perspective
        moveDirection = PlayerCamera.instance.transform.forward * verticalMovement;
        moveDirection = moveDirection + PlayerCamera.instance.transform.right * horizontalMovement;
        moveDirection.Normalize();
        moveDirection.y = 0;

        if (PlayerInputManager.instance.moveAmount > 0.5f)
        {
            //run
            player.characterController.Move(moveDirection * runSpeed * Time.deltaTime);
        }
        else if (PlayerInputManager.instance.moveAmount <= 0.5f)
        {
            //walk
            player.characterController.Move(moveDirection * walkSpeed * Time.deltaTime);
        }
    }

    private void HandleRotation()
    {
        targetRotationDir = Vector3.zero;
        targetRotationDir = PlayerCamera.instance.camObject.transform.forward * verticalMovement;
        targetRotationDir = targetRotationDir + PlayerCamera.instance.camObject.transform.right * horizontalMovement;
        targetRotationDir.Normalize();
        targetRotationDir.y = 0;

        if (targetRotationDir == Vector3.zero)
        {
            targetRotationDir = transform.forward;
        }

        Quaternion newRotation = Quaternion.LookRotation(targetRotationDir);
        Quaternion targetRotation = Quaternion.Slerp(transform.rotation, newRotation, rotationSpeed * Time.deltaTime);
        transform.rotation = targetRotation;
    }
}
