using UnityEngine;

public class PlayerManager : CharacterManager
{
    [HideInInspector]public PlayerLocomotion playerLocomotion;
    [HideInInspector]public PlayerAnimatorManager playerAnimatorManager;

    protected override void Awake()
    {
        base.Awake();

        PlayerCamera.instance.player = this;
        PlayerInputManager.instance.player = this;

        playerLocomotion = GetComponent<PlayerLocomotion>();
        playerAnimatorManager = GetComponent<PlayerAnimatorManager>();
    }

    protected override void Update()
    {
        base.Update();

        playerLocomotion.HandleMovement();
    }

    protected override void LateUpdate()
    {
        base.LateUpdate();

        PlayerCamera.instance.HandleCameraActions();
    }
}
