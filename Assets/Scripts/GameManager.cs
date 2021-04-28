using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField]
    private Pickup[] pickupArray;
    [SerializeField]
    private UIManager uiManager = null;
    [SerializeField]
    private FinishZone finishZone = null;

    public int currentScore { get; private set; } = 0;

    public bool isPaused { get; private set; } = false;

    private bool allowPause = true;

    private void Start()
    {
        //subscribe to all of the star pickup events
        foreach (Pickup star in pickupArray) star.OnPickUp += FinishUnlockCheck;

        uiManager.UpdateStarText(0, pickupArray.Length);
    }

    private void Update()
    {
        if (Input.GetButtonDown("Pause") && allowPause)
        {
            isPaused = !isPaused;
            uiManager.SetPauseUI(isPaused);
            Time.timeScale = isPaused ? 0 : 1;
        }
    }

    public void CompleteLevel()
    {
        //pause game
        Time.timeScale = 0;

        //open ui
        uiManager.OpenLevelCompleteMenu();

        //disable pausing
        allowPause = false;
    }

    public void UnpauseGame()
    {
        isPaused = false;
        Time.timeScale = 1;
        uiManager.SetPauseUI(false);
    }

    /// <summary>
    /// Add points to the total score.
    /// </summary>
    /// <param name="addedScore"> The value to be added to total score</param>
    public void AddScore(int addedScore)
    {
        currentScore += addedScore;
        uiManager.UpdateScoreText(currentScore);
    }

    /// <summary>
    /// Check if all stars have been collected.
    /// Unlocks door if all have been collected.
    /// </summary>
    /// <param name="obj"></param>
    private void FinishUnlockCheck(Pickup obj)
    {
        //loop through all stars and count each collected
        int totalCollected = 0;
        for (int i = 0; i < pickupArray.Length; i++)
        {
            if (pickupArray[i].IsCollected) totalCollected++;
        }

        //if all stars are collected open door
        if (totalCollected == pickupArray.Length) finishZone.UnlockExit();

        //update UI
        uiManager.UpdateStarText(totalCollected, pickupArray.Length);
    }
}
