using UnityEngine;

public class CharacterManager : MonoBehaviour
{
    public CharacterController characterController;
    [HideInInspector]public Animator animator;

    protected virtual void Awake()
    {
        DontDestroyOnLoad(this);

        characterController = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
    }

    protected virtual void Update()
    {

    }

    protected virtual void LateUpdate()
    {

    }
}
