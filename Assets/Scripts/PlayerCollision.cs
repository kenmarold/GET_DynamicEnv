using UnityEngine;

public class PlayerCollision : MonoBehaviour
{
    private ScoreManager scoreManager;

    void Start()
    {
        scoreManager = FindObjectOfType<ScoreManager>(); // Find ScoreManager in the scene
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("collectible"))
        {
            if (scoreManager != null)
            {
                scoreManager.AddScore(10); // Increase score by 10
            }
            
            Destroy(other.gameObject); // Remove the collected object
        }
    }
}
