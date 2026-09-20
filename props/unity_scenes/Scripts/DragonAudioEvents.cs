using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class DragonAudioEvents : MonoBehaviour
{
    private AudioSource audioSource;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    // The Animation Event will call this method exactly when we tell it to
    public void PlayRoarSound()
    {
        if (audioSource != null && audioSource.clip != null)
        {
            audioSource.Play();
        }
    }
}