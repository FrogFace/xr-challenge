using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

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
    [SerializeField] 
    private GameObject exitHint = null;
    [SerializeField]
    private GameObject exitConfirmationUI = null;
    [SerializeField]
    private GameObject levelCompletionUI = null;
    [SerializeField]
    private GameObject continueButton = null;
    [SerializeField]
    private GameObject cancelButton = null;
    [SerializeField]
    private GameObject completionExitButton = null;
    [SerializeField]
    private GameObject gameOverUI = null;
    [SerializeField]
    private GameObject gameOverContinueButton = null;

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

    public void OpenLevelCompleteMenu()
    {
        levelCompletionUI.SetActive(true);
        gameplayUI.SetActive(false);
        EventSystem.current.SetSelectedGameObject(completionExitButton);
    }

    public void OpenGameOverMenu()
    {
        gameplayUI.SetActive(false);
        gameOverUI.SetActive(true);
        EventSystem.current.SetSelectedGameObject(gameOverContinueButton);
    }

    public void ReloadScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void SetPauseUI(bool isPaused)
    {
        PauseUI.SetActive(isPaused);
        gameplayUI.SetActive(!isPaused);
        exitConfirmationUI.SetActive(false);

        if (isPaused) EventSystem.current.SetSelectedGameObject(continueButton);
    }

    public void SetExitHint(bool active)
    {
        exitHint.SetActive(active);
    }

    public void ExitWarning()
    {
        PauseUI.SetActive(false);
        exitConfirmationUI.SetActive(true);

        EventSystem.current.SetSelectedGameObject(cancelButton);
    }

    public void ExitToMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }
}
