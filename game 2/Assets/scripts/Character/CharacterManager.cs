using UnityEngine;

public class CharacterManager : MonoBehaviour
{
    protected virtual void Awake()
    {
        DontDestroyOnLoad(this);
    }

    protected virtual void Update()
    {

    }
}
