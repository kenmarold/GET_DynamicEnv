using UnityEngine;

public class GET_ObjectSounds : MonoBehaviour
{
    [Header("Sound Clips")]
    public AudioClip collisionSound;  // Sound when player collides
    public AudioClip raycastHoverSound; // Sound when raycast highlights
    public AudioClip pushSound; // Sound when pushed with "E"

    private AudioSource audioSource;
    private bool hasPlayedRaycastSound = false; // Ensures hover sound plays once per object interaction
    private bool isCollided = false;  // Prevents repeated collision sound

    private void Start()
    {
        // Ensure an AudioSource component is attached to the object
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.spatialBlend = 1.0f; // Make it 3D sound
        audioSource.playOnAwake = false;
    }

    public void PlayCollisionSound()
    {
        if (!isCollided && collisionSound != null)
        {
            audioSource.PlayOneShot(collisionSound);
            isCollided = true;
            Invoke(nameof(ResetCollision), 0.5f); // Prevents spam
        }
    }

    public void PlayRaycastHoverSound()
    {
        if (!hasPlayedRaycastSound && raycastHoverSound != null)
        {
            audioSource.PlayOneShot(raycastHoverSound);
            hasPlayedRaycastSound = true;
        }
    }

    public void PlayPushSound()
    {
        if (pushSound != null)
        {
            audioSource.PlayOneShot(pushSound);
        }
    }

    public void ResetRaycastSound()
    {
        hasPlayedRaycastSound = false;
    }

    private void ResetCollision()
    {
        isCollided = false;
    }
}
