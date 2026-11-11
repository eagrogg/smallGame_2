using UnityEngine;

public class PlayerLocomotion : CharacterLocomotion
{
    PlayerManager player;

    [HideInInspector] public float verticalMovement;
    [HideInInspector] public float horizontalMovement;
    [HideInInspector] public float moveAmount;

    [Header("Movement Settings")]
    private Vector3 moveDirection;
    private Vector3 targetRotationDir;
    [SerializeField] float walkSpeed = 2;
    [SerializeField] float runSpeed = 5;
    [SerializeField] float rotationSpeed = 15;

    [Header("Dodge")]
    private Vector3 rollDirection;

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
        if (!player.canMove)
            return;
        
        GetVerticalAndHorizontalInputs();
        //move direction is based on camera's perspective
        moveDirection = PlayerCamera.instance.transform.forward * verticalMovement;
        moveDirection = moveDirection + PlayerCamera.instance.transform.right * horizontalMovement;
        moveDirection.y = 0;
        moveDirection.Normalize();

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
        if (!player.canRotate)
            return;

        targetRotationDir = Vector3.zero;
        targetRotationDir = PlayerCamera.instance.camObject.transform.forward * verticalMovement;
        targetRotationDir = targetRotationDir + PlayerCamera.instance.camObject.transform.right * horizontalMovement;
        targetRotationDir.y = 0;
        targetRotationDir.Normalize();
        

        if (targetRotationDir == Vector3.zero)
        {
            targetRotationDir = transform.forward;
        }

        Quaternion newRotation = Quaternion.LookRotation(targetRotationDir);
        Quaternion targetRotation = Quaternion.Slerp(transform.rotation, newRotation, rotationSpeed * Time.deltaTime);
        transform.rotation = targetRotation;
    }

    public void AttemptToPerformDodge()
    {
        if (player.isPerformingAction)
            return;

        if (PlayerInputManager.instance.moveAmount > 0)
        {
            rollDirection = PlayerCamera.instance.camObject.transform.forward * PlayerInputManager.instance.verticalInput;
            rollDirection += PlayerCamera.instance.camObject.transform.right * PlayerInputManager.instance.horizontalInput;

            rollDirection.y = 0;
            rollDirection.Normalize();
            Quaternion playerRotation = Quaternion.LookRotation(rollDirection);
            player.transform.rotation = playerRotation;
            //roll if dodge input is pressed while moving

            player.playerAnimatorManager.PlayTargetActionAnimation("Roll_Forward_01", true);
        }
        else
        {
            player.playerAnimatorManager.PlayTargetActionAnimation("Back_Step_01", true);
            //backstep if stationary
        }
    }
}
