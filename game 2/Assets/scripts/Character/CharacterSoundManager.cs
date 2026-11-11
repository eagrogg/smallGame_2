using UnityEngine;

public class CharacterSoundManager : MonoBehaviour
{
    private AudioSource audioSource;

    protected virtual void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public void PlayRollSoundEffects()
    {
        audioSource.PlayOneShot(WorldSFXManager.instance.rollSFX);
    }
}
