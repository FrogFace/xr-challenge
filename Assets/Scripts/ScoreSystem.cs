using UnityEngine;
using UnityEngine.UI;

public class ScoreSystem : MonoBehaviour
{
    [SerializeField]
    private Text scoreText = null;
    [SerializeField]
    private int currentScore = 0;

    private void Start()
    {
        UpdateScoreText();
    }

    /// <summary>
    /// Add points to the total score.
    /// </summary>
    /// <param name="addedScore"> The value to be added to total score</param>
    public void AddScore(int addedScore)
    {
        currentScore += addedScore;
        UpdateScoreText();
    }

    private void UpdateScoreText()
    {
        scoreText.text = $"SCORE: {currentScore}";
    }
}
