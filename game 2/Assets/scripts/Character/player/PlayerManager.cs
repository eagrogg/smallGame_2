using UnityEngine;

public class PlayerManager : CharacterManager
{
    PlayerLocomotion playerLocomotion;

    protected override void Awake()
    {
        base.Awake();

        PlayerCamera.instance.player = this;

        playerLocomotion = GetComponent<PlayerLocomotion>();
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
