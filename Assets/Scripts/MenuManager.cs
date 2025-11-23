using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    [SerializeField] private GameObject mainMenu;

    private void Start()
    {
        //First time we save max level as 1
        if (!PlayerPrefs.HasKey(GameConstants.MAXACT_KEY))
        {
            PlayerPrefs.SetInt(GameConstants.MAXACT_KEY, 1);
        }
    }

    /// <summary>
    /// Opens a menu depending on the object included.
    /// </summary>
    /// <param name="menuName"></param>
    public void OpenMenu(GameObject menuName)
    {
        mainMenu.SetActive(false);
        menuName.SetActive(true);
    }

    /// <summary>
    /// Closes the current menu and goes back to the main menu.
    /// </summary>
    /// <param name="menuName"></param>
    public void GoBackToMainMenu(GameObject menuName)
    {
        menuName.SetActive(false);
        mainMenu.SetActive(true);
    }

    /// <summary>
    /// Loads the Select Act scene.
    /// </summary>

    [ContextMenu("Load Select Act Scene")]
    public void SelectAct()
    {
        //Go to the next Scene
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
    public void QuitGame()
    {
        //Quit the application
        Application.Quit();
    }

}
