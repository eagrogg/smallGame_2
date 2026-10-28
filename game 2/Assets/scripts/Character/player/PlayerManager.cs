using UnityEngine;

public class PlayerManager : CharacterManager
{
    PlayerLocomotion playerLocomotion;

    protected override void Awake()
    {
        base.Awake();

        playerLocomotion = GetComponent<PlayerLocomotion>();
    }

    protected override void Update()
    {
        base.Update();

        playerLocomotion.HandleMovement();
    }
}
