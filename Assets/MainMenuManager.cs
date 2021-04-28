using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class MainMenuManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField]
    private GameObject mainMenu = null;
    [SerializeField]
    private GameObject guideUI = null;
    [SerializeField]
    private GameObject aboutUI = null;
    [SerializeField]
    private GameObject mainMenuStartingButton = null;
    [SerializeField]
    private GameObject aboutMenuStartingButton = null;
    [SerializeField]
    private GameObject guideMenuStartingButton = null;


    // Start is called before the first frame update
    void Start()
    {
        Time.timeScale = 1;
    }
    public void EnableMainMenu()
    {
        mainMenu.SetActive(true);
        guideUI.SetActive(false);
        aboutUI.SetActive(false);

        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(mainMenuStartingButton);
    }

    public void EnableGuideMenu()
    {
        mainMenu.SetActive(false);
        guideUI.SetActive(true);
        aboutUI.SetActive(false);

        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(guideMenuStartingButton);
    }

    public void EnableAboutMenu()
    {
        mainMenu.SetActive(false);
        guideUI.SetActive(false);
        aboutUI.SetActive(true);

        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(aboutMenuStartingButton);
    }

    public void PlayGame()
    {
        SceneManager.LoadScene("Main");
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
