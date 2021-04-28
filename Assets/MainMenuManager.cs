using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField]
    private GameObject mainMenu = null;
    [SerializeField]
    private GameObject guideUI = null;


    // Start is called before the first frame update
    void Start()
    {
        Time.timeScale = 1;
    }

    public void EnableGuideMenu()
    {
        mainMenu.SetActive(false);
        guideUI.SetActive(true);
    }

    public void EnableMainMenu()
    {
        mainMenu.SetActive(true);
        guideUI.SetActive(false);
    }

    public void PlayGame()
    {
        SceneManager.LoadScene("Main");
    }
}
