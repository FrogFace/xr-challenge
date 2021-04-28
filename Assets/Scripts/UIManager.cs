using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField]
    private Text scoreText = null;
    [SerializeField]
    private Text starText = null;
    [SerializeField]
    private Slider HealthBar = null;
    [SerializeField]
    private GameManager gameManager = null;
    [SerializeField]
    private GameObject PauseUI = null; 
    [SerializeField]
    private GameObject gameplayUI = null;  

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

    public void SetPauseUI(bool isPaused)
    {
        PauseUI.SetActive(isPaused);
        gameplayUI.SetActive(!isPaused);
    }
}
