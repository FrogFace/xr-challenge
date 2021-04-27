using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [SerializeField]
    private Text scoreText = null;
    [SerializeField]
    private Text starText = null;
    [SerializeField]
    private Slider HealthBar = null;
    [SerializeField]
    private GameManager gameManager = null; 

    public void UpdateScoreText(int currentScore)
    {
        scoreText.text = $"Gold: {currentScore}";
    }

    public void UpdateHealthBar(float healthPercentage)
    {
        HealthBar.value = healthPercentage;
    }

    public void UpdateStarText(int currentStars, int totalStars)
    {
        starText.text = $"Stars: {currentStars} / {totalStars}";
    }
}
