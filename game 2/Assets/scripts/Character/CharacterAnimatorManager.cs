using UnityEngine;

public class CharacterAnimatorManager : MonoBehaviour
{
    CharacterManager character;

    protected virtual void Awake()
    {
        character = GetComponent<CharacterManager>();
    }

    public void UpdateAnimatorMovementParameters(float horizontalValue, float verticalValue)
    {
        character.animator.SetFloat("Horizontal", horizontalValue, 0.1f, Time.deltaTime);
        character.animator.SetFloat("Vertical", verticalValue, 0.1f, Time.deltaTime);
    }
}
