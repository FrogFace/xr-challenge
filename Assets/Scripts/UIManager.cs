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

    /// <summary>
    /// updates the scoretext with the given score
    /// </summary>
    /// <param name="currentScore">The value to update the text with</param>
    public void UpdateScoreText(int currentScore)
    {
        scoreText.text = $"Gold: {currentScore}";
    }

    /// <summary>
    /// updates the health bar UI
    /// </summary>
    /// <param name="healthPercentage"> percentage of health, give as 0 - 1</param>
    public void UpdateHealthBar(float healthPercentage)
    {
        HealthBar.value = healthPercentage;
    }

    /// <summary>
    /// updates the star count UI
    /// </summary>
    /// <param name="currentStars"> the number of stars collected</param>
    /// <param name="totalStars"> the total number of stars</param>
    public void UpdateStarText(int currentStars, int totalStars)
    {
        starText.text = $"Stars: {currentStars} / {totalStars}";
    }

    /// <summary>
    /// Enables the level compelte menu
    /// </summary>
    public void OpenLevelCompleteMenu()
    {
        levelCompletionUI.SetActive(true);
        gameplayUI.SetActive(false);
        EventSystem.current.SetSelectedGameObject(completionExitButton);
    }

    /// <summary>
    /// enables the game over menu
    /// </summary>
    public void OpenGameOverMenu()
    {
        gameplayUI.SetActive(false);
        gameOverUI.SetActive(true);
        EventSystem.current.SetSelectedGameObject(gameOverContinueButton);
    }

    /// <summary>
    /// reloads the current scene
    /// </summary>
    public void ReloadScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    /// <summary>
    /// enables or disables the pause ui
    /// </summary>
    /// <param name="isPaused"><see langword="true"/>if enablign ui, false if disabling</param>
    public void SetPauseUI(bool isPaused)
    {
        PauseUI.SetActive(isPaused);
        gameplayUI.SetActive(!isPaused);
        exitConfirmationUI.SetActive(false);

        if (isPaused) EventSystem.current.SetSelectedGameObject(continueButton);
    }

    /// <summary>
    /// enables or disables exit hint
    /// </summary>
    /// <param name="active">true to enable, false to disable</param>
    public void SetExitHint(bool active)
    {
        exitHint.SetActive(active);
    }

    /// <summary>
    /// Enables the exit warning UI
    /// </summary>
    public void ExitWarning()
    {
        PauseUI.SetActive(false);
        exitConfirmationUI.SetActive(true);

        EventSystem.current.SetSelectedGameObject(cancelButton);
    }

    /// <summary>
    /// Loads the MainMenu scene
    /// </summary>
    public void ExitToMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }
}
