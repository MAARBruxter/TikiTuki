using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance;

    [Header("Panels")]
    [Tooltip("UI Game Over")]
    [SerializeField] private GameObject gameOverPanel;

    [Tooltip("UI Win Level")]
    [SerializeField] private GameObject winLevelPanel;

    [Tooltip("UI Act")]
    [SerializeField] private GameObject ActUI;

    [Header("UI Pause Menu")]
    [SerializeField] private GameObject pauseMenu;
    private bool isPaused = false;

    [Space]
    [Tooltip("Falling apple prefab")]
    public GameObject fallingApplePrefab;

    [Space]
    [Tooltip("Level time in seconds")]
    [Range(0f, 300f)]
    [SerializeField] private float levelTime;

    private float internalLevelTime;
    public float InternalLevelTime { get => internalLevelTime; set => internalLevelTime = value; }

    private PlayerHealth playerHealth;

    private void Awake()
    {
        // Singleton
        Instance = this;

        // Reset boss health
        EnemyController.ResetBossHealth();

        playerHealth = GameObject.FindGameObjectWithTag(GameConstants.PLAYERTAG_KEY).GetComponent<PlayerHealth>();

        // Disable panels
        gameOverPanel.SetActive(false);
        winLevelPanel.SetActive(false);

        Time.timeScale = 1f;

        // Initialize level time
        internalLevelTime = levelTime;
        LockCursor();
    }

    private void Update()
    {
        // Manage level time
        internalLevelTime -= Time.deltaTime;

        if (internalLevelTime <= 0.9f)
            GameOver();

        // Detect Escape or Start
        if (Keyboard.current.escapeKey.wasPressedThisFrame ||
            (Gamepad.current != null && Gamepad.current.startButton.wasPressedThisFrame))
        {
            TogglePause();
        }
    }

    // ---------- GAME STATES ---------- //

    /// <summary>
    /// Activates the Game Over Panel.
    /// </summary>
    [ContextMenu("Game Over")]
    public void GameOver()
    {
        ActUI.SetActive(false);
        gameOverPanel.SetActive(true);
        PauseGameLogic();
    }

    /// <summary>
    /// Acivates the win level Panel.
    /// </summary>
    [ContextMenu("Win Level")]
    public void WinLevel()
    {
        if (GameManager.Instance)
        {
            GameManager.Instance.TimePoints += (int)(internalLevelTime * 100);
            GameManager.Instance.FruitPoints += playerHealth.FruitCount * 500;
            GameManager.Instance.PlayerPoints = GameManager.Instance.TimePoints + GameManager.Instance.FruitPoints;
        }

        ActUI.SetActive(false);
        winLevelPanel.SetActive(true);
        PauseGameLogic();
    }

    /// <summary>
    /// Activates or deactivates the Pause panel.
    /// </summary>
    private void TogglePause()
    {
        if (!winLevelPanel.activeSelf && !gameOverPanel.activeSelf)
        {
            if (isPaused)
                ResumeGame();
            else
                PauseGame();
        }
    }

    /// <summary>
    /// Pauses the game and shows the pause panel.
    /// </summary>
    private void PauseGame()
    {
        isPaused = true;
        pauseMenu.SetActive(true);
        PauseGameLogic();
    }

    /// <summary>
    /// Resumes the game and hides the pause panel.
    /// </summary>
    public void ResumeGame()
    {
        isPaused = false;
        pauseMenu.SetActive(false);
        ResumeGameLogic();
    }

    /// <summary>
    /// Pauses the game.
    /// </summary>
    private void PauseGameLogic()
    {
        Time.timeScale = 0f;
        UnlockCursor();
    }

    /// <summary>
    /// Resumes the game.
    /// </summary>
    private void ResumeGameLogic()
    {
        Time.timeScale = 1f;
        LockCursor();
    }

    // ---------- SCENE MANAGEMENT ---------- //

    /// <summary>
    /// Loads the main menu screen.
    /// </summary>
    [ContextMenu("Load Main Menu Scene")]
    public void MainMenuScene()
    {
        CheckHighScore();

        GameManager.Instance.TimePoints = 0;
        GameManager.Instance.FruitPoints = 0;
        GameManager.Instance.PlayerPoints = 0;

        UnlockCursor();
        SceneManager.LoadScene(GameConstants.MAINMENU_LEVEL_NAME);
    }

    /// <summary>
    /// Reloads the current scene.
    /// </summary>
    [ContextMenu("Reload Current Scene")]
    public void ReloadScene()
    {
        CheckMaxAct();
        CheckHighScore();

        GameManager.Instance.TimePoints = 0;
        GameManager.Instance.FruitPoints = 0;
        GameManager.Instance.PlayerPoints = 0;

        LockCursor();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    /// <summary>
    /// Loads the next level.
    /// </summary>
    [ContextMenu("Load Next Scene")]
    public void NextLevel()
    {
        CheckMaxAct();

        int levelCount = SceneManager.sceneCountInBuildSettings - 1;

        if (SceneManager.GetActiveScene().buildIndex < levelCount)
        {
            LockCursor();
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        }
        else
        {
            CheckHighScore();

            GameManager.Instance.TimePoints = 0;
            GameManager.Instance.FruitPoints = 0;
            GameManager.Instance.PlayerPoints = 0;
        }
    }

    // ---------- PLAYERPREFS LOGIC ---------- //

    /// <summary>
    /// Checks the actual highscore and determines if must be changed.
    /// </summary>
    private void CheckHighScore()
    {
        if (PlayerPrefs.HasKey(GameConstants.HIGHSCORE_KEY))
        {
            int highScore = PlayerPrefs.GetInt(GameConstants.HIGHSCORE_KEY);
            if (GameManager.Instance.PlayerPoints > highScore)
                PlayerPrefs.SetInt(GameConstants.HIGHSCORE_KEY, GameManager.Instance.PlayerPoints);
        }
        else
        {
            PlayerPrefs.SetInt(GameConstants.HIGHSCORE_KEY, GameManager.Instance.PlayerPoints);
        }
    }

    /// <summary>
    /// Check what the Max Act is in the game using the highest number after "Act".
    /// </summary>
    public void CheckMaxAct()
    {
        int currentLevelIndex = int.Parse(SceneManager.GetActiveScene().name.Substring(3));

        if (!PlayerPrefs.HasKey(GameConstants.MAXACT_KEY))
        {
            PlayerPrefs.SetInt(GameConstants.MAXACT_KEY, currentLevelIndex + 1);
        }
        else if (PlayerPrefs.GetInt(GameConstants.MAXACT_KEY) < currentLevelIndex + 1)
        {
            PlayerPrefs.SetInt(GameConstants.MAXACT_KEY, currentLevelIndex + 1);
        }
    }

    // ---------- CURSOR UTILS ---------- //

    /// <summary>
    /// Hides and locks the cursor from the player.
    /// </summary>
    private void LockCursor()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    /// <summary>
    /// Shows and unlocks the cursor from the player.
    /// </summary>
    private void UnlockCursor()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
}
