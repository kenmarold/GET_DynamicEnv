using UnityEngine;
using TMPro;
using System.Collections;

public class PlayerScoreManager : MonoBehaviour
{
    public int score = 0;
    public TMP_Text scoreText;
    private bool canAddPoints = true;
    private bool canLosePoints = true;
    public float scoreCooldown = 1f;

    void Start()
    {
        UpdateScoreText();
    }

void OnCollisionEnter(Collision collision)
{
    Debug.Log($"💥 Collided with: {collision.gameObject.name} | Tag: {collision.gameObject.tag}");

    if (collision.gameObject.CompareTag("addPoints"))
    {
        score += 10;
        Debug.Log("✅ Score Increased: " + score);
        UpdateScoreText();
    }
    else if (collision.gameObject.CompareTag("losePoints"))
    {
        score -= 10;
        Debug.Log("❌ Score Decreased: " + score);
        UpdateScoreText();
    }
}


    void UpdateScoreText()
    {
        if (scoreText != null)
        {
            scoreText.text = "Score: " + score;
            Debug.Log("📊 Score UI Updated: " + score);
        }
        else
        {
            Debug.LogError("ERROR: ScoreText UI is not assigned!");
        }
    }
}
