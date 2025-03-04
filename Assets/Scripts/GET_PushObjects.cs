using UnityEngine;
using System.Collections.Generic;

public class GET_PushObjects : MonoBehaviour
{
    [Header("Push Settings")]
    public float pushForce = 5f; // Force applied when pushing with "E"
    public float pushRange = 2f; // Maximum distance for pushing with "E"
    public float collisionPushStrength = 3f; // Strength of push when colliding
    public float raycastLength = 2f; // Adjustable raycast length in the inspector

    [Header("Player References")]
    public Camera playerCamera; // Reference to the player's camera

    private Renderer lastHighlightedRenderer;
    private Dictionary<Renderer, Color> originalColors = new Dictionary<Renderer, Color>();

    private void Update()
    {
        if (Input.GetKey(KeyCode.E)) // Hold 'E' to push objects
        {
            TryPushObject();
        }
        HighlightObject();
    }

    void TryPushObject()
    {
        if (playerCamera == null)
        {
            Debug.LogWarning("Player camera not assigned!");
            return;
        }

        RaycastHit hit;
        Vector3 forward = playerCamera.transform.forward; // Use camera's forward direction
        Vector3 origin = playerCamera.transform.position; // Start ray from camera position

        if (Physics.Raycast(origin, forward, out hit, raycastLength))
        {
            Renderer renderer = hit.collider.GetComponent<Renderer>();
            Rigidbody rb = hit.collider.attachedRigidbody;
            GET_ObjectSounds objectSounds = hit.collider.GetComponent<GET_ObjectSounds>(); // Get sound script

            if (rb != null && !rb.isKinematic)
            {
                Vector3 forceDirection = hit.point - origin;
                forceDirection.y = 0; // Keep force horizontal
                rb.AddForce(forceDirection.normalized * pushForce, ForceMode.Impulse);

                // Apply gravity only when pushed with "E"
                rb.useGravity = true;

                // Play push sound
                if (objectSounds != null)
                {
                    objectSounds.PlayPushSound();
                }
            }
            
            if (renderer != null && originalColors.ContainsKey(renderer))
            {
                renderer.material.SetColor("_BaseColor", originalColors[renderer]); // Reset to original color after push
            }
        }
    }

    void HighlightObject()
    {
        if (playerCamera == null) return;

        RaycastHit hit;
        Vector3 forward = playerCamera.transform.forward;
        Vector3 origin = playerCamera.transform.position;

        if (Physics.Raycast(origin, forward, out hit, raycastLength))
        {
            Renderer renderer = hit.collider.GetComponent<Renderer>();
            GET_ObjectSounds objectSounds = hit.collider.GetComponent<GET_ObjectSounds>(); // Get sound script

            if (renderer != null)
            {
                if (lastHighlightedRenderer != renderer)
                {
                    // Reset sound if highlighting a new object
                    if (lastHighlightedRenderer != null)
                    {
                        GET_ObjectSounds lastObjectSound = lastHighlightedRenderer.GetComponent<GET_ObjectSounds>();
                        if (lastObjectSound != null)
                        {
                            lastObjectSound.ResetRaycastSound();
                        }
                    }

                    ResetLastHighlighted();
                    StoreOriginalColor(renderer);
                    renderer.material.SetColor("_BaseColor", Color.yellow);
                    lastHighlightedRenderer = renderer;
                }

                // Play hover sound only once per interaction
                if (objectSounds != null)
                {
                    objectSounds.PlayRaycastHoverSound();
                }
            }
        }
        else
        {
            ResetLastHighlighted();
        }
    }

    private void ResetLastHighlighted()
    {
        if (lastHighlightedRenderer != null && originalColors.ContainsKey(lastHighlightedRenderer))
        {
            lastHighlightedRenderer.material.SetColor("_BaseColor", originalColors[lastHighlightedRenderer]);
            lastHighlightedRenderer = null;
        }
    }

    private void StoreOriginalColor(Renderer renderer)
    {
        if (!originalColors.ContainsKey(renderer))
        {
            originalColors[renderer] = renderer.material.GetColor("_BaseColor");
        }
    }

    private void OnDrawGizmos()
    {
        if (playerCamera != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawRay(playerCamera.transform.position, playerCamera.transform.forward * raycastLength);
        }
    }

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        Rigidbody rb = hit.collider.attachedRigidbody;
        Renderer renderer = hit.collider.GetComponent<Renderer>();
        GET_ObjectSounds objectSounds = hit.collider.GetComponent<GET_ObjectSounds>(); // Get sound script

        if (rb != null && !rb.isKinematic)
        {
            Vector3 forceDirection = hit.moveDirection; // Use player's movement direction
            forceDirection.y = 0; // Keep force horizontal to avoid tilting

            rb.AddForce(forceDirection * collisionPushStrength, ForceMode.Impulse);
        }
        
        if (renderer != null && originalColors.ContainsKey(renderer))
        {
            renderer.material.SetColor("_BaseColor", originalColors[renderer]); // Reset to original color after collision
        }

        // Play collision sound
        if (objectSounds != null)
        {
            objectSounds.PlayCollisionSound();
        }
    }
}
